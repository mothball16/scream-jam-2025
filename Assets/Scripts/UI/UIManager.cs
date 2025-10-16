using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// Handles UI and stuff. Wow.
/// </summary>
public class UIManager : MonoBehaviour
{
    private List<IDisposable> _disposables;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Canvas _screen;
    private Transform _dialogueBox, _dialogueTemplate;
    private Text _state;
    private List<string> _citations;
    private void Awake()
    {
        _state = _screen.transform.Find("State").GetComponent<Text>();
        _dialogueBox = _screen.transform.Find("Dialogue");
        _dialogueTemplate = _dialogueTemplate.Find("Template");
        _citations = new();
    }
    private void OnEnable()
    {
        _disposables = new()
        {
            EventBus.Subscribe<CorrectChoiceEvent>(OnCorrectChoice),
            EventBus.Subscribe<IncorrectChoiceEvent>(OnIncorrectChoice),
            EventBus.Subscribe<RequestDialogueEvent>(OnRequestDialogue)
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

    private void OnRequestDialogue(RequestDialogueEvent e)
    {

        var temp = Instantiate(_dialogueTemplate);
        temp.gameObject.SetActive(true);
        temp.SetParent(_dialogueBox, false);
        var tempText = temp.GetComponent<Text>();
        tempText.text = e.msg;
        DOTween.Sequence()
            .Append(tempText.DOFade(1, 2).From(0))
            .AppendInterval(e.time)
            .Append(tempText.DOFade(0, 2)).Play();
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
