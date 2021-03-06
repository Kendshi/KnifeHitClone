using System.Collections;
using UnityEngine;

public class PushKnife : MonoBehaviour
{
    public static PushKnife singltone;                      //Синглтон

    [HideInInspector] public bool dontThrow;                //Запрещает бросок ножа

    private GameObject knife;                               //текущий нож который можно метнуть
    private Rigidbody2D rb;
    [SerializeField] private float power;                   //Сила с которой метается нож
    [SerializeField] private GameObject prefabKnife;        //префаб ножа для метания
    [SerializeField] private Transform KnifeRespawnPoint;   //Позиция на которой должен появиться новый нож
    
    private void Awake()
    {
        singltone = this;
    }

    private void Start()
    {
        dontThrow = false;                                 //устанавливаем значение по умолчанию
        knife = GameObject.FindGameObjectWithTag("Knife"); //Находим актуальный метательный нож на сцене
    }

    private void Update()
    {
        if (knife == null && !dontThrow) //если актуальный нож удалился
        {   //создаем новый нож
            knife = Instantiate(prefabKnife, KnifeRespawnPoint.position, Quaternion.identity);
            StartCoroutine(StopThrow(GameManager.manager.timeBetweenThrows)); // устанавливаем интервал между бросками ножа
        }
    }

    private void OnMouseDown()
    { // Разрешаем бросок, если интервал выдержан и на сцене существует бревно
            if (!dontThrow && GameManager.manager.logOnScene != null)
            {
                rb = knife.GetComponent<Rigidbody2D>();
                rb.AddForce(Vector2.up * power, ForceMode2D.Impulse);
            }
    }

    /// <summary>
    /// Задает интервал между бросками, время настаивается
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator StopThrow(float time)
    {
        dontThrow = true;
        yield return new WaitForSeconds(time);
        dontThrow = false;
    }
}
