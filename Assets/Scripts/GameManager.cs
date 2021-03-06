using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;                      //Синглтон

    [HideInInspector] public int hitCount;                  //счетчик попаданий
    [HideInInspector] public int currentIcon;               //Текущая картинка доступного ножа
    [HideInInspector] public int apple = 0;                 //Количество собранных половинок яблок
    [HideInInspector] public int stage = 1;                 //текущий уровень
    [HideInInspector] public List<GameObject> knifsIcons;   //список иконок доступных ножей
    [HideInInspector] public GameObject logOnScene;         //текущее бревно на сцене, нужно для PushKnife, чтобы нельзя было бросить нож пока бревно отсутсвует
    [Tooltip("Скорость вращения бревна")]
    public float logSpeedRotation;                          //Скорость вращения бревна
    public event Action WinLevel;                           //Событие успешного прохождения уровня
    [Tooltip("задержка между бросками ножа")]
    public float timeBetweenThrows;                         //время между бросками
    [Tooltip("минимальное время вращения бревна до остановки")]
    public float rotationTimeMIN;
    [Tooltip("максимальное время вращения бревна до остановки")]
    public float rotetionTimeMAX;
    [Tooltip("Минимальное время остановки")]
    public float stopTimeMIN;
    [Tooltip("Максимальное время остановки")]
    public float stopTimeMAX;

    private int knifesAmount;                               //Количество доступных ножей на начало уровня
    private Vector4 orangeColor;                            //Ораньжевый цвет
    private int currentSphere;                              //Текущая сфера

    [SerializeField] private Text stageText;                //Текст выводящий текущий уровень
    [SerializeField] private Text hitCountText;             //Текст выводящий количество попаданий в бревно
    [SerializeField] private Text appleCountText;           //Текст выводящий количество поряженных яблок в половинках
    [SerializeField] private GameObject knifeAmountIcon;    //Префаб иконки доступного ножа
    [SerializeField] private GameObject knifsAmountPanel;   //Панель где располагаются иконки доступных ножей
    [SerializeField] private GameObject logPrefab;          //Префаб бревна
    [SerializeField] private Transform logRespawnPosition;  //точка где создаются новые брёвна
    [SerializeField] private GameObject SpherePanel;        //панель с шариками, показывают текущий этап игры
    [SerializeField] private List<Image> sphereIcons = new List<Image>(); //список иконок шариков
    
    private void Awake()
    {
        knifsIcons = new List<GameObject>(); 
        manager = this;
        if (PlayerPrefs.HasKey("Apple")) apple = PlayerPrefs.GetInt("Apple"); //если до этого заработали яблок то устанавливаем это значение
    }

    void Start()
    {//устанавливаем стандартное значение для переменных
        stage = 1;
        hitCount = 0;
        currentSphere = 0;
        knifesAmount = 8;
        FillingKnifesIcons();               //Заполняем панель доступных ножей
        currentIcon = knifsIcons.Count;     //устанавливаем счетчик ножей на самый верхний
        orangeColor = new Vector4(0.94f, 0.59f, 0.08f, 1f); //Задаем значение ораньжевого цвета
        logOnScene = GameObject.FindGameObjectWithTag("Log");   //находим текущее бревно
    }

    void Update()
    {
        hitCountText.text = "  " + hitCount;                //обновляем счетчик попаданий, выводим на экран
        appleCountText.text = "" + apple;                   //обновляем счетчик половинок яблок

        if (stage % 5 != 0)
        {//Если не босс то номер уровня белого цвета
            stageText.color = Color.white;
            stageText.text = "STAGE " + stage;                  // Показываем текущий уровень
        }
        else if (stage % 5 == 0)
        {//Если уровень с боссом, то меняем цвет текста на красный и сам текст меняем
            stageText.color = Color.red;
            stageText.text = "BOSS: LEMON";
        }
       
        if (currentIcon == 0)
        {//Если все ножи закончились то Победа
            WinLevel();                                     //запускаем событие победы на уровне
            DeleteOldKnifsIcon();                           //Удаляем иконки ножей с интерфейса
            knifsIcons.RemoveRange(0, knifsIcons.Count);    //удаляем все элементы списка иконок доступных ножей
            knifesAmount = UnityEngine.Random.Range(8, 11); //Задаем новое значение количества доступных ножей
            stage += 1;                                     //Повышаем значение уровня
            FillingKnifesIcons();                           //Заполняем интерфейс новыми иконками ножей и заполняем список
            currentIcon = knifesAmount;                     //устанавливаем счетчик ножей на самый верхний
            if (stage % 5 != 0)
            {//Перемещаем счетчик текущей сферы вправо
                currentSphere += 1;
            }
            
            if (currentSphere > 3)
            {//Когда доходим до последней сферы, начинаем заново
                currentSphere = 0;
                AllSphereWhite();                           //Красим все сферы в белый
            }
            sphereIcons[currentSphere].color = orangeColor; //Меняем цвет текущей сферы на ораньжевый
            StartCoroutine(CreateNewLog());                 // Создаем новое бревно
        }
    }

    /// <summary>
    /// создание иконок доступных ножей и добавление их в список
    /// </summary>
    private void FillingKnifesIcons()
    {
        for (int i = 0; i < knifesAmount; i++)
        {
           GameObject icon = Instantiate(knifeAmountIcon, knifsAmountPanel.transform);
           knifsIcons.Add(icon);
        }
    }

    /// <summary>
    /// Изменяет состояние иконок доступных ножей
    /// </summary>
    public void ChangeIconState()
    {
        GameObject whiteIcon = knifsIcons[currentIcon].transform.GetChild(0).gameObject;
        Image currentKnife = whiteIcon.GetComponent<Image>();
        currentKnife.fillAmount = 0;
    }

    /// <summary>
    /// Удаляет объекты интерфейса - доступные ножи
    /// </summary>
    private void DeleteOldKnifsIcon()
    {
        for (int i = 0; i < knifsIcons.Count; i++)
        {
            Destroy(knifsIcons[i]);
        }
    }

    /// <summary>
    /// Делает все уровневые сферы белыми
    /// </summary>
    private void AllSphereWhite()
    {
        for (int i = 0; i < sphereIcons.Count; i++)
        {
            sphereIcons[i].color = Color.white;
        }
    }

    /// <summary>
    /// Создание нового бревна
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateNewLog()
    {
        yield return new WaitForSeconds(0.8f);
        logOnScene = Instantiate(logPrefab,logRespawnPosition.position, Quaternion.identity);
    }
}
