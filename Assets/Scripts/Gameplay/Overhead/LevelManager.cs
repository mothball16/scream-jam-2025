using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using PAC = PackageAttributeConstraints;
using Assets.Scripts.Util;
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
    public int violations;
    public int maxViolations;

    void Start()
    {
        if (GameManager.Inst == null)
        {
            Utils.Talk(new("(SYSTEM) GameManager not initialized. Day one will work, but the rest won't."));
            StartDay(Days.DayOne);
        }
        else
            StartDay(GameManager.Inst.CurrentDay);
    }

    private void StartDay(Days day)
    {
        var pg = PackageGenerator.Inst;
        packages = new Queue<Package>();
        violations = 0;
        _packagesLeft = -2;

        switch (day)
        {
            case Days.DayOne:
                maxViolations = 2;
                //so we dont go to gameend during the dialogue sequence
                Utils.Talk(new("*yawn* What up rookie."));
                Utils.TalkDeferred(new("Welcome to your new job. Take a look around. <i>(MOUSE to look. A/D to turn.)</i>",3), 3);
                Utils.Defer(6, () => {
                    Utils.Talk(new("Oh, and here's the first package. Check your manual to figure out what to do."));
                    packages.Enqueue(
                       new Package(true,
                       pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day),
                       pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper(),
                       OnSpawnedCallback: (obj) =>
                       {
                           Utils.Talk(new("test, making sure OnSpawnedCallback works"));
                           //this puts the package ontop of the conveyor window lol
                           obj.transform.position += new Vector3(0, 5, 0);
                       },
                       OnProcessedCallback: (obj, accepted) => {
                           Utils.Talk(accepted
                               ? new("Good work. You're doing better than the last guy already.")
                               : new("...yikes... who the hell hired you?", Color: ChatColors.Angry)
                               );
                       }));
                    packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                    _packagesLeft = packages.Count;
                });
               
                break;
            case Days.DayTwo:
                maxViolations = 1;
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateBadRegionAddress(day), pg.GenerateBadZipAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateBadZipAddress(day), pg.GenerateBadRegionAddress(day), pg.GetBadDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                _packagesLeft = packages.Count;
                break;
            case Days.DayThree:
                maxViolations = 0;
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

    private void OnEnable()
    {
        _connections = new()
        {
            EventBus.Subscribe<DecisionMadeEvent>(OnDecisionMade),
        };
    }

    private void OnDisable()
    {
        _connections.ForEach(x => x.Dispose());
        _connections.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPackage();
    }

    public int GetRemainingPackages() => _packagesLeft;

    private void OnDecisionMade(DecisionMadeEvent msg)
    {
        Utils.Talk(new("(processing)"));
        Debug.Log(msg.Accepted);
        if(msg.Accepted != _activePackage.Valid)
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
