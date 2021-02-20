using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text scoreText;        //контейнер интерфейса под количество попаданий
    [SerializeField] private Text stageText;        //контейнер интерфейса под максимально достигнутый уровень
    [SerializeField] private Text appleText;        //контейнер интерфейса под яблоки
    [SerializeField] private Toggle toggleVibration;//Кнопка включения/выключения вибрации в настройках

    private int hitBestScore = 0;                   //лучший результат попаданий на начало игры
    private int hitInLastGame = 0;                  //лучший результат попаданий в последней сессии
    private int hitResultScore;                     //итоговый максимальный результат который и станет лучшим результатом
    private int stageBestScore = 0;                 //максимально достигнутый уровень на начало игры
    private int stageInLastGame = 0;                //максимально достигнутый уровень в последней сессии
    private int stageResult;                        //итоговый максимальный уровень
    private int apple = 0;                          //количество яблочек
    private int vibrationOption;                    //настройка - включена ли вибрация

    private void Awake()
    {
        if (PlayerPrefs.HasKey("BestScore")) hitBestScore = PlayerPrefs.GetInt("BestScore");    //проверяем наличие записи, в случае успеха записываем в переменную
        if (PlayerPrefs.HasKey("HitCount")) hitInLastGame = PlayerPrefs.GetInt("HitCount");

        if (hitBestScore >= hitInLastGame ) hitResultScore = hitBestScore;                      //Проверяем какой из результатов больше
        else if (hitBestScore < hitInLastGame) hitResultScore = hitInLastGame;
            
        PlayerPrefs.SetInt("BestScore", hitResultScore);                                       //Записываем наибольшее значение

        if (PlayerPrefs.HasKey("BestStage")) stageBestScore = PlayerPrefs.GetInt("BestStage"); //проверяем наличие записи, в случае успеха записываем в переменную
        if (PlayerPrefs.HasKey("Stage")) stageInLastGame = PlayerPrefs.GetInt("Stage");

        if (stageBestScore >= stageInLastGame) stageResult = stageBestScore;                    //Проверяем какой из результатов больше
        else if(stageBestScore < stageInLastGame) stageResult = stageInLastGame;

        PlayerPrefs.SetInt("BestStage", stageResult);                                           //Записываем наибольшее значение

        if (PlayerPrefs.HasKey("Apple")) apple = PlayerPrefs.GetInt("Apple");                   //проверяем наличие записи, в случае успеха записываем в переменную

        if (PlayerPrefs.HasKey("Vibration")) vibrationOption = PlayerPrefs.GetInt("Vibration");
        else vibrationOption = 1;

        PlayerPrefs.SetInt("Vibration", vibrationOption);                                       //Записываем значение опции вибрация
    }

    void Start()
    {
        scoreText.text = "SCORE " + hitResultScore;     //Выводим лучший результат попаданий
        stageText.text = "STAGE " + stageResult;        //Выводим максимально достигнутый уровень
        appleText.text = "" + apple;                    //Выводим накопленное количество яблок
    }


    /// <summary>
    /// Переходим на главную сцену, начинаем игру
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    ///Метод записывает значени которое говорит включить или выключить вибрацию в игре 
    /// </summary>
    public void VibrationSettings()
    {
        if (toggleVibration.isOn)
            vibrationOption = 1;

        else if (!toggleVibration.isOn)
            vibrationOption = 0;

        PlayerPrefs.SetInt("Vibration", vibrationOption);
    }
}
