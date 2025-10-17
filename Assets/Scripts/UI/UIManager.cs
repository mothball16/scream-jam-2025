using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Assets.Scripts.Events;
using Assets.Scripts.Util;

public enum ChatColors
{
    Default,
    Angry,
    Disappointed
}

/// <summary>
/// Handles UI and stuff. Wow.
/// </summary>
public class UIManager : MonoBehaviour
{
    private static Dictionary<ChatColors, Color> _chatColors = new()
    {
        {ChatColors.Default, Color.white},
        {ChatColors.Angry, new Color(255/255f,100/255f,100/255f)},
        {ChatColors.Disappointed, Color.yellow }
    };


    private List<IDisposable> _disposables;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Canvas _screen;
    [SerializeField] private Sprite[] _manualPages;
    private Transform _dialogueBox, _dialogueTemplate, _manual;
    private Text _state;
    private Image _fade;
    private List<string> _citations;
    private void Awake()
    {
        _state = _screen.transform.Find("State").GetComponent<Text>();
        _dialogueBox = _screen.transform.Find("Dialogue");
        _dialogueTemplate = _dialogueBox.Find("Template");
        _fade = _screen.transform.Find("FadePanel").GetComponent<Image>();
        _manual = _screen.transform.Find("Manual");
        _citations = new();
    }
    private void OnEnable()
    {
        _disposables = new()
        {
            EventBus.Subscribe<CorrectChoiceEvent>(OnCorrectChoice),
            EventBus.Subscribe<IncorrectChoiceEvent>(OnIncorrectChoice),
            EventBus.Subscribe<RequestDialogueEvent>(OnRequestDialogue),
            EventBus.Subscribe<FadeToEvent>(OnFadeTo),
            EventBus.Subscribe<ManualFlippedEvent>(OnManualFlipped),
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
        tempText.text = e.Message;
        tempText.color = _chatColors[e.Color];
        DOTween.Sequence()
            .Append(tempText.DOFade(1, 2).From(0))
            .AppendInterval(e.Time)
            .Append(tempText.DOFade(0, 2))
            .OnComplete(() => {
                if (temp != null && temp.gameObject != null)
                    Destroy(temp.gameObject);
            });
    }

    private void OnManualFlipped(ManualFlippedEvent e)
    {
        var rect = _manual.GetComponent<RectTransform>();
        rect.DOAnchorPos(new Vector3(0, -800, 0), 0.5f);

        if (e.Page == 1)
        {
            rect.DOAnchorPos(new Vector3(0, 0, 0), 0.5f);
            _manual.GetComponent<Image>().sprite = _manualPages[e.Page - 1];
        }
        else if (e.Page != 0)
        {
            Utils.Defer(0.5f, () =>
            {
                _manual.GetComponent<Image>().sprite = _manualPages[e.Page - 1];
                rect.DOAnchorPos(new Vector3(0, 0, 0), 0.5f);
            });
        }
        else
        {

        }
        

    }

    private void OnFadeTo(FadeToEvent e)
    {
        var fIn = e.Direction == FadeDirection.In;
        _fade.color = e.FadeColor;
        _fade.DOFade(fIn ? 1f : 0f, e.TweenTime).From(fIn ? 0f : 1f).SetEase(Ease.InCubic);
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
