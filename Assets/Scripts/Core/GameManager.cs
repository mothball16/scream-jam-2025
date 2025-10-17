using Assets.Scripts;
using Assets.Scripts.Util;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Days
{
    DayOne = 1,
    DayTwo = 2,
    DayThree = 3,
    DayFour = 4,
    BombExploded,
    FiredForSuckingAtJob,
    GameEnd,
}

public enum StoryFlags
{
    FailedFirstPackage,
    EnablePickup,
    AcknowledgedBombPackage,
}

public class GameManager : MonoSingleton<GameManager>
{
    public Days CurrentDay;
    private Dictionary<StoryFlags, bool> _flags;
    

    public override void Init()
    {
        _flags = new() { };
    }

    public void LoadLevel()
        => LoadLevel(CurrentDay);

    public void LoadLevel(Days day)
    {
        CurrentDay = day;
        Debug.Log(day);
        DOTween.KillAll();
        SceneManager.LoadScene(SceneNames.GameScene);
    }

    public void SetFlag(StoryFlags flag, bool value = true)
    {
        _flags[flag] = value;
    }

    public bool GetFlag(StoryFlags flag)
    {
        if(!_flags.TryGetValue(flag, out bool value)) 
            return false;
        return value;
    }
}