using UnityEngine;

public class KnifeBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject Knife;                  //Префаб ножа который появляется в бревне
    [SerializeField] private GameObject prefabAnimationFail;    //анимированный нож который появляется в случае попадания в другой нож
    [SerializeField] private GameObject halfApple;              //анимированные половинки яблока которые появляются на месте яблока в бревне.
    [SerializeField] private GameObject splintersParticles;     //стружка появляющаяся на месте попадания ножа в бревно

    private int vibrationOption;                                //переменная показывающая нужно включать вибрацию или нет

    private void Start()
    {
        vibrationOption = PlayerPrefs.GetInt("Vibration");      //Получаем значение опции вибрация
        Vibration.Init();                                       //инициализируем скрипт с методами вибрации
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { //События происходящие если нож попал в бревно
        if (collision.gameObject.CompareTag("Log"))
        {
            if(vibrationOption >= 1)
            Vibration.VibratePop();                             //включаем вибрацию если нужно
            PlayerPrefs.SetInt("HitCount", GameManager.manager.hitCount);
            PlayerPrefs.SetInt("Stage", GameManager.manager.stage);
            PlayerPrefs.SetInt("Apple", GameManager.manager.apple);
            GameManager.manager.hitCount += 1;                  //увеличиваем счетчик попаданий
            GameManager.manager.currentIcon -= 1;               //передвигает позицию текущей иконки доступного ножа
            GameManager.manager.ChangeIconState();              //изменяе состояние иконок доступных ножей
            ContactPoint2D contact = collision.contacts[0];     //Получаем первую точку контакта ножа с бревном
            Instantiate(Knife, contact.point, Quaternion.identity, collision.gameObject.transform); // в этой точке создаем нож
            Instantiate(splintersParticles, contact.point - new Vector2(0f, 0.2f), Quaternion.Euler(180f, 0f, 0f)); // чуть ниже этой точки создаем партиклы стружки из дерева
            SoundManager.singltone.sounds[0].Play();            //включаем звук попадания ножа
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("OldKnife"))
        {//События происходящие при попадании в другой нож
            InGameMenu.singltone.StopGame();            //Включаем меню в игре
            if(vibrationOption == 1)
            Vibration.VibratePeek();                    //включаем вибрацию если она разрешена
            Instantiate(prefabAnimationFail, transform.position, Quaternion.identity);  //создаем анимированный нож который вращаясь улетает в даль
            SoundManager.singltone.sounds[1].Play();    //Проигрываем звук попадания
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Apple"))
        {// Если попали в яблоко
            GameObject halfs = Instantiate(halfApple, gameObject.transform.position, Quaternion.identity); //Создаем анимированные половинки яблок падающие вниз 
            Destroy(other.gameObject);
            Destroy(halfs, 1f);
            GameManager.manager.apple += 2;     //Увеличиваем счетчик яблок
        }
    }
}
