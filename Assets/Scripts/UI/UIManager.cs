using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI and stuff. Wow.
/// </summary>
public class UIManager : MonoBehaviour
{
    private List<IDisposable> _disposables;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Canvas _screen;
    private Text _state;
    private List<string> _citations;
    private void Awake()
    {
        _state = _screen.transform.Find("State").GetComponent<Text>();
        _citations = new();
    }
    private void OnEnable()
    {
        _disposables = new()
        {
            EventBus.Subscribe<CorrectChoiceEvent>(OnCorrectChoice),
            EventBus.Subscribe<IncorrectChoiceEvent>(OnIncorrectChoice)
        };
    }

    private void OnDisable()
    {
        _disposables.ForEach(disposable => disposable.Dispose());
        _disposables.Clear();
    }

    private void OnCorrectChoice(CorrectChoiceEvent e)
    {
        UpdateGameState();

    }

    private void OnIncorrectChoice(IncorrectChoiceEvent e)
    {
        _citations.AddRange(e.Citations);
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        string text = $"You have {_levelManager.GetRemainingPackages()} packages left to process. Hurry!";
        foreach(var c in _citations)
        {
            text += "\n" + c;
        }
        _state.text = text;
    }

    private void Update()
    {
        
    }
}
