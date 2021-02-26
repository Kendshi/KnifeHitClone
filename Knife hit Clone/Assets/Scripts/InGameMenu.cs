using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public static InGameMenu singltone;         //Синглтон

    public bool stopRotation;                   //Переменная останавлявающая вращение бревна

    private Animator animator;

    [SerializeField] private GameObject Menu;   //объект со всем содержанием внутреигрового меню
    [SerializeField] private Text stageText;    //отображение текущего уровня
    [SerializeField] private Text hitScoreText; //отображение текущего счетчика попаданий

    private void Awake()
    {
        stopRotation = false;
        singltone = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StopGame()
    {
        stageText.text = "STAGE " + GameManager.manager.stage;
        hitScoreText.text = "" + GameManager.manager.hitCount;
        //Сохраняем все нужные данные
        PlayerPrefs.SetInt("Stage", GameManager.manager.stage);
        PlayerPrefs.SetInt("HitCount", GameManager.manager.hitCount);
        PlayerPrefs.SetInt("Apple", GameManager.manager.apple);
        PlayerPrefs.Save();
        PushKnife.singltone.dontThrow = true; //Пока меню активно запрещаем броски ножей
        StartCoroutine(LateActiveMenu());     //Делаем красивый эффект постепенного появления меню совокупно с анимацией  
        animator.SetBool("StopGame", true);
        stopRotation = true;                  //Останавливаем вращение бревна
    }

    /// <summary>
    /// Перезагрузка уровня
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(1,LoadSceneMode.Single);
    }

    /// <summary>
    /// Продолжить игру
    /// </summary>
    public void ContineGame()
    {
        Menu.SetActive(false);
        animator.SetBool("StopGame", false);
        stopRotation = false;
        PushKnife.singltone.dontThrow = false;
    }

    IEnumerator LateActiveMenu()
    {
        yield return new WaitForSeconds(0.6f);
        Menu.SetActive(true);
    }
    /// <summary>
    /// Вернуться в главное меню
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
