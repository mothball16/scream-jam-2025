using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using PAC = PackageAttributeConstraints;
using Assets.Scripts.Util;
using Assets.Scripts.Events;
/// <summary>
/// Handles the spawning of packages and the general game flow.
/// </summary>
public class LevelManager : MonoBehaviour
{
    private int _packagesLeft;
    public Queue<Package> packages;
    private List<IDisposable> _connections;
    private Package _activePackage;
    private GameObject _activePackageModel;
    private bool _running;
    private InputSystem_Actions _actions;

    private int _currentPage, _availablePages;

    public int violations;
    public int maxViolations;

    void Start()
    {
        if (GameManager.Inst == null)
        {
            Utils.Talk(new("(SYSTEM) GameManager not initialized. Run from the bootstrapper idiot", Color: ChatColors.Angry));
            StartDay(Days.DayOne);
        }
        else
            StartDay(GameManager.Inst.CurrentDay);
    }

    private void StartDay(Days day)
    {
        Utils.Fade(new(Color.black, 1f, FadeDirection.Out));

        var pg = PackageGenerator.Inst;
        packages = new Queue<Package>();
        violations = 0;
        _packagesLeft = -2;
        _running = true;
        _availablePages = 0;
        _currentPage = 0;
        switch (day)
        {
            case Days.DayOne:
                maxViolations = 2;
                _availablePages = 2;
                //so we dont go to gameend during the dialogue sequence
                
                Utils.Talk(new("*yawn* What up rookie."));
                Utils.Defer(3, () => {
                    Utils.Talk(new("Welcome to your new job! Take a look around. <i>(MOUSE to look. A/D to turn.)</i>", 3));
                });
                Utils.Defer(6, () => {
                    Utils.TalkDeferred(3, new("Your responsibility is to examine these packages, tossing out sketchy ones."));
                    Utils.TalkDeferred(8, new("You have a scale and a manual to get started"));
                    Utils.TalkDeferred(12, new("<i>F to pull up the manual.</i>"));
                    packages.Enqueue(
                       new Package(true,
                       pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day),
                       pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper(),
                       OnSpawnedCallback: (obj) =>
                       {
                           Utils.Talk(new("Oh, and here's the first package."));
                           Utils.TalkDeferred(18, new("Take your time before you make your decision"));
                           Utils.TalkDeferred(21, new("You DON'T want to make the wrong one", Color: ChatColors.Angry));
                           Utils.TalkDeferred(24, new("btw use your mouse to examine"));
                           Utils.Defer(24, () =>  GameManager.Inst.SetFlag(StoryFlags.EnablePickup) );
                           Utils.TalkDeferred(30, new("Approve by placing the package on the next conveyer otherwise, deny with E"));
                           //Utils.Talk(new("test, making sure OnSpawnedCallback works"));
                           //this puts the package ontop of the conveyor window lol
                           obj.transform.position += new Vector3(0, 0, 0);
                       },
                       OnProcessedCallback: (obj, accepted) => {
                           Utils.Talk(accepted
                               ? new("Good work. You're doing better than the last guy already.")
                               : new("...yikes... who the hell hired you?", Color: ChatColors.Angry)
                               );
                           if (!accepted) GameManager.Inst.SetFlag(StoryFlags.FailedFirstPackage);
                       }));
                    packages.Enqueue(new Package(false,
                        pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day),
                        pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper(),
                        OnSpawnedCallback: (obj) =>
                        {
                            if (GameManager.Inst.GetFlag(StoryFlags.FailedFirstPackage))
                            {
                                Utils.TalkDeferred(3, new("...Try not to mess up this time.", Color: ChatColors.Disappointed));
                            }
                                
                        
                        },
                       OnProcessedCallback: (obj, accepted) => {
                           Utils.Talk(!accepted
                               ? new("Nice job using your scale.")
                               : new("how'd you miss the check of the only other action you have.", Color: ChatColors.Disappointed)
                               );
                           if (!accepted) GameManager.Inst.SetFlag(StoryFlags.FailedFirstPackage);
                       }));
                    Debug.Log(packages.Peek().WeightPair);
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper(),
                        OnSpawnedCallback: (obj) =>
                        {
                            Utils.TalkDeferred(3, new("listen pal,"));
                            Utils.TalkDeferred(5, new("just stay focused and you'll be fine."));
                            Utils.TalkDeferred(8, new("I'll check in with ya tomorrow. *click*"));

                        }));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                    _packagesLeft = packages.Count;
                });
                break;
            case Days.DayTwo:
                maxViolations = 1;
                _availablePages = 3;
                Utils.Talk(new("...You seen the news today?", Color: ChatColors.Angry));
                Utils.Defer(3, () =>
                {
                    Utils.Talk(new("<i>You messed up. Bad.</i>", Color: ChatColors.Angry));
                    packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateBadRegionAddress(day), pg.GenerateBadZipAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateBadZipAddress(day), pg.GenerateBadRegionAddress(day), pg.GetBadDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                    _packagesLeft = packages.Count;
                });
                
                break;
            case Days.DayThree:
                maxViolations = 0;
                _availablePages = 4;
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                _packagesLeft = packages.Count;
                break;
            default:
                Debug.LogError(day.ToString());
                break;
        }

    }

    private void Awake()
    {
        _actions = new();
    }

    private void OnEnable()
    {
        _connections = new()
        {
            EventBus.Subscribe<DecisionMadeEvent>(OnDecisionMade),
        };
        _actions.Player.Enable();
        _actions.Player.Manual.started += OnMaunalFlip;

    }

    private void OnDisable()
    {
        _connections.ForEach(x => x.Dispose());
        _connections.Clear();
        _actions.Player.Disable();
        _actions.Player.Manual.started -= OnMaunalFlip;
    }

    private void OnMaunalFlip(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _currentPage = (_currentPage + 1) % _availablePages;
        EventBus.Publish<ManualFlippedEvent>(new(_currentPage));
    }

    // Update is called once per frame
    void Update()
    {
        CheckPackage();
    }

    public int GetRemainingPackages() => _packagesLeft;

    private void OnDecisionMade(DecisionMadeEvent msg)
    {
        if (_activePackage == null || _activePackageModel == null)
        {
            Utils.Talk(new("how the hell did you call this? there is no package to make a decision on", Color: ChatColors.Angry));
            return;
        }

        if (!_activePackageModel.TryGetComponent<PackageInfo>(out var info))
            Debug.LogError("no package info on the active package model");

        Utils.Talk(new("<i>(processing)</i>", 0));
        info.Processed = true;

        if (msg.Accepted != _activePackage.Valid)
        {
            Debug.Log("Wrong Decesion");
            violations++;
            if(violations > maxViolations)
            {
                //Pop up Game Over message propmting restart
            }
        }
        else
        {
            Debug.Log("Good Job");
        }

        if (_activePackage.OnProcessedCallback is not null)
            _activePackage.OnProcessedCallback(_activePackageModel, msg.Accepted);

        Destroy(_activePackageModel, 1);
        _activePackage = null;
        _activePackageModel = null;
    }

    private void CheckPackage()
    {
        if (_packagesLeft == -1)
        {
            EndDay();
        } else if(_packagesLeft == -2)
        {
            //do nothin. this is prior to game start
        } else if (_packagesLeft >= 0 && CountObjectsWithTag("Package") == 0)
        {
            if (_packagesLeft != 0)
            {
                _activePackage = packages.Dequeue();
                _activePackageModel = PackageGenerator.Inst.SpawnPackage(_activePackage);
                if (_activePackage.OnSpawnedCallback is not null)
                    _activePackage.OnSpawnedCallback(_activePackageModel);
            }
            _packagesLeft--;
        }

    }
    public int CountObjectsWithTag(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        return taggedObjects.Length;
    }

    public void EndDay()
    {
        if (!_running) return;
        _running = false;
        Days day = GameManager.Inst.CurrentDay;

        switch (day)
        {
            case Days.DayOne:
                GameManager.Inst.LoadLevel(Days.DayTwo);
                break;
            case Days.DayTwo:
                GameManager.Inst.LoadLevel(Days.DayThree);
                break;
            case Days.DayThree:
                GameManager.Inst.LoadLevel(Days.DayFour);
                break;
            default:
                Debug.LogError(day.ToString());
                GameManager.Inst.LoadLevel(Days.DayOne);
                break;
        }
    }
}
