using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using PAC = PackageAttributeConstraints;
/// <summary>
/// Handles the spawning of packages and the general game flow.
/// </summary>
public class LevelManager : MonoBehaviour
{
    private int _packagesLeft;
    public Queue<Package> packages;
    public PackageGenerator pg = PackageGenerator.Instance;
    private List<IDisposable> _connections;
    public Package activePackage;
    public int violations;
    public int maxViolations;

    void Start()
    {
        StartDay(GameManager.Inst.CurrentDay);
    }

    private void StartDay(Days day)
    {
        packages = new Queue<Package>();
        violations = 0;

        switch (day)
        {
            case Days.DayOne:
                maxViolations = 2;

                //packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                //packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                //packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                //packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                //packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateBadShipper()));

                _packagesLeft = packages.Count;
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
        Debug.Log(msg.Accepted);
        if(msg.Accepted != activePackage.Valid)
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
    }

    private void CheckPackage()
    {
        if (_packagesLeft == -1)
        {
            EndDay();
        }
        else if (_packagesLeft >= 0 && CountObjectsWithTag("Package") == 0)
        {
            if (_packagesLeft !=0)
            {
                activePackage = packages.Dequeue();
                pg.SpawnPackage(activePackage);
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
