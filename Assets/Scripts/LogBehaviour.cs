using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogBehaviour : MonoBehaviour
{
    private Transform logTransform;
    private Animator animator;
    private int vibrationOption;                                //значение переменной говорит нужно ли воспроизводить вибрацию в игре
    [SerializeField] private AppleChance appleChance;           //экземпляр ScriptableObject с шансом появления яблока 
    [SerializeField] private List<Transform> respawnPoints = new List<Transform>(); //точки в которых могут спавниться яблоки или ножи
    [SerializeField] private GameObject applePrefab;            //Префаб яблока стоящего на бревне
    [SerializeField] private GameObject knifePrefab;            //Префаб ножа воткнутого в бревно
    [SerializeField] private GameObject logAnimationPrefab;     //3D бревно с анимацией разваливания на части
    [SerializeField] private GameObject logImage;               //Объект с изображением бревна
    [SerializeField] private GameObject lemonImage;             //объект с изображением лимона 
    [SerializeField] private GameObject lemonParticle;          //партиклы капель лимона при его уничтожении
    
    private bool increaseOn;                                    //переменная включает повышенние скорости вращения
    private bool decreaseOn;                                    //переменная включает понижение скорости вращения бревна
    private float currentSpeed;                                 //Дополнительная переменная скорости вращения, участвует в циклах вращения
    private bool ON = true;                                     //вспомогательная переменная, не дает запустить корутину больше 1 раза

    private void Start()
    {
        GameManager.manager.WinLevel += WinRound;               //Подписываемся на событие выйгранный уровень 
        logTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        SpawnApple();                                           //создаем яблоки на бревне
        SpawnKnifes();                                          //создаем ножи в бревне
        if (GameManager.manager.stage % 5 == 0)                 //Меняем спрайт бревна на спрайт лимона во время битвы с босом
        {
            logImage.SetActive(false);
            lemonImage.SetActive(true);
        }

        vibrationOption = PlayerPrefs.GetInt("Vibration");      //Получаем значение опции вибрации
        currentSpeed = 1;                                       //устанавливаем нормальное значение скорости вращения
    }

    private void Update()
    {
        if (GameManager.manager.stage > 1)
        { //Основной способ вращения бревна, максимальная скорость вращения задается в GameManager
            if (!InGameMenu.singltone.stopRotation)
                logTransform.Rotate(0f, 0f, (1f * currentSpeed) * GameManager.manager.logSpeedRotation * Time.deltaTime, Space.Self);
        }
        else { 
        if (!InGameMenu.singltone.stopRotation)//Метод вращения для первого уровня, вращение равномерное, без остановок
        logTransform.Rotate(0f, 0f, 1f * GameManager.manager.logSpeedRotation * Time.deltaTime, Space.Self);
        }

        if (GameManager.manager.stage >1 && ON)
        { // Для задания интервалов используется 2 корутины, время вращения до остановки задается случано из чисел в GameManager
            ON = false;
            StartCoroutine(RotationTime(Random.Range(GameManager.manager.rotationTimeMIN, GameManager.manager.rotetionTimeMAX)));
        }
        //увеличиваем скорость вращения
        if (increaseOn)
            IncreaseSpeed();
        //уменьшаем скорость вращения
        if (decreaseOn)
            DecreaseSpeed();
    }

    /// <summary>
    ///Если нож попадает в бревно, срабатывает анимация попадания 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Knife"))
            animator.SetTrigger("Hit");
    }

    /// <summary>
    /// В начале каждого уровня создаем яблоки на бревне
    /// </summary>
    private void SpawnApple()
    {
        for (int i = 0; i < 3; i++) // максимум можно создать 3 яблока
        {
            int totalChance = Random.Range(0, 101); // случайное число от 1 до 100 определяет появится яблоко или нет
           
            if (totalChance <= appleChance.Chance) // шанс появления яблока равен 25% задается внешне из экземпляра ScriptableObject  
            {// Если число totalChance входит в наш диапазон, то создаем яблоко на случайной позиции из списка.
                int point = Random.Range(0, respawnPoints.Count);
                Instantiate(applePrefab, respawnPoints[point].position, Quaternion.Euler(0f, 0f, respawnPoints[point].rotation.eulerAngles.z), gameObject.transform);
                respawnPoints.RemoveAt(point); // удаляем эту позицию из списка, чтобы на том же месте не появилось ничего другого.
            }
        }
    }

    /// <summary>
    /// Создаем ножи в бревне 
    /// </summary>
    private void SpawnKnifes()
    { // Создаем случайное количество ножей от 1 до 3
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            int point = Random.Range(0, respawnPoints.Count);
            Instantiate(knifePrefab, respawnPoints[point].position, Quaternion.Euler(0f, 0f, respawnPoints[point].rotation.eulerAngles.z + 180), gameObject.transform);
            respawnPoints.RemoveAt(point);
        }
    }

    /// <summary>
    /// Событие успешно пройденный уровень
    /// </summary>
    public void WinRound()
    { // если настройки позволяют включаем вибрацию
        if (vibrationOption == 1)
            Handheld.Vibrate();
        //Если не босс, то на месте бревна создаем префаб 3D разрушаемого бревна
        if (GameManager.manager.stage % 5 !=0)
        {
            GameObject log = Instantiate(logAnimationPrefab, gameObject.transform.position - new Vector3(0f, 0f, 0.2f), Quaternion.Euler(90f, 0f, 0f));
            Destroy(log, 1.2f);
        } // если босс, то в случае победы создаем префаб с партиклами каплями лимона
        else if (GameManager.manager.stage % 5 == 0)
        {
            Instantiate(lemonParticle, new Vector3(0f, 1.6f, -1f), Quaternion.identity);
        }
       
        GameManager.manager.WinLevel -= WinRound; // отписка от события
        Destroy(gameObject);
    }

    /// <summary>
    /// Постепенно увеличивает скорость вращения бревна до определенного предела 
    /// </summary>
    private void IncreaseSpeed()
    {
        if (currentSpeed <= 1.3f)
            currentSpeed += 0.01f;
    }
    /// <summary>
    /// Постепенно уменьшает скорость вращения бревна до определенного предела 
    /// </summary>
    private void DecreaseSpeed()
    {
        if (currentSpeed >= 0)
            currentSpeed -= 0.007f;
    }

    /// <summary>
    /// Определяет сколько будет вращатся бревно до остановки
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator RotationTime(float time)
    {
        yield return new WaitForSeconds(time);
        increaseOn = false;
        decreaseOn = true;
        StartCoroutine(StopTime(Random.Range(GameManager.manager.stopTimeMIN, GameManager.manager.stopTimeMAX)));
    }

    /// <summary>
    /// Определяет Сколько времени продлится остановка вращения бревна
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator StopTime(float time)
    {
        yield return new WaitForSeconds(time);
        decreaseOn = false;
        increaseOn = true;
        ON = true;
    }
}
