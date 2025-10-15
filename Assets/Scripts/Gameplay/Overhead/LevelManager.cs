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
    public GameManager GameManager = GameManager.Instance;
    private List<IDisposable> _connections;
    public Package activePackage;
    public int violations;
    public int maxViolations;

    void Start()
    {
    }

    private void OnLoadLevel(LoadLevelEvent e)
    {

        packages = new Queue<Package>();
        violations = 0;
        int day = e.day;
        switch (e.day)
        {
            case 1:
                maxViolations = 2;
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateBadShipper()));

                _packagesLeft = packages.Count;
                break;
            case 2:
                Debug.Log("help me");
                maxViolations = 1;
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateBadRegionAddress(day), pg.GenerateBadZipAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateID(), pg.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateBadZipAddress(day), pg.GenerateBadRegionAddress(day), pg.GetBadDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateID(), pg.GenerateBadShipper()));
                _packagesLeft = packages.Count;
                break;
            case 3:
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
                break;
        }

    }

    private void OnEnable()
    {
        _connections = new()
        {
            EventBus.Subscribe<DecisionMadeEvent>(OnDecisionMade),
            EventBus.Subscribe<LoadLevelEvent>(OnLoadLevel)
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
        int day = GameManager.Instance.currentDay;
        switch (day)
        {
            case 1:
                GameManager.Instance.LoadLevel(2);
                break;
            case 2:
                GameManager.Instance.LoadLevel(3);
                break;
            case 3:
                GameManager.Instance.LoadLevel(4);
                break;
        }
    }
}
