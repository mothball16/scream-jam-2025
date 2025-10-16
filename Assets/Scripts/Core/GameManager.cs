using Assets.Scripts;
using Assets.Scripts.Util;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Days
{
    DayOne = 1,
    DayTwo = 2,
    DayThree = 3,
    DayFour = 4,
    FiredForSuckingAtJob
}


public class GameManager : MonoSingleton<GameManager>
{
    public Days CurrentDay;
    

    public override void Init()
    {

    }

    public void LoadLevel(Days day)
    {
        CurrentDay = day;
        Debug.Log(day);
        DOTween.KillAll();
        SceneManager.LoadScene(SceneNames.GameScene);
    }
}