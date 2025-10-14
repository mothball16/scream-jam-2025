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
    public PackageGenerator PackageGenerator;
    public GameManager GameManager;
    private List<IDisposable> _connections;
    public Package activePackage;
    void Start()
    {
        int day = 1;
        packages = new Queue<Package>();
        switch (day)
        {
            case 1:
                packages.Enqueue(new Package(true,PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(),PackageGenerator.GenerateGoodRemark(),PackageGenerator.GenerateID(),PackageGenerator.GenerateGoodShipper()));
                packages.Enqueue(new Package(false, PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(), PackageGenerator.GenerateGoodRemark(), PackageGenerator.GenerateID(), PackageGenerator.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(), PackageGenerator.GenerateGoodRemark(), PackageGenerator.GenerateID(), PackageGenerator.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(), PackageGenerator.GenerateGoodRemark(), PackageGenerator.GenerateID(), PackageGenerator.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(), PackageGenerator.GenerateGoodRemark(), PackageGenerator.GenerateID(), PackageGenerator.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(), PackageGenerator.GenerateGoodRemark(), PackageGenerator.GenerateID(), PackageGenerator.GenerateGoodShipper()));
                packages.Enqueue(new Package(true, PackageGenerator.GenerateGoodWeightPair(), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GenerateGoodAddress(day), PackageGenerator.GetCurrentDate(day).ToString(), PackageGenerator.GenerateGoodRemark(), PackageGenerator.GenerateID(), PackageGenerator.GenerateGoodShipper()));

                _packagesLeft = packages.Count;
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }

    }

    private void OnEnable()
    {
        _connections = new()
        {
            EventBus.Subscribe<DecisionMadeEvent>(OnDecisionMade)
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
        }
        else
        {
            Debug.Log("Good Job");
        }
    }

    private void CheckPackage()
    {
        if(_packagesLeft > 0 && CountObjectsWithTag("Package") == 0)
        {
            activePackage = packages.Dequeue();
            PackageGenerator.SpawnPackage(activePackage);
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
        int day = 1;
        switch (day)
        {
            case 1:
                break;
            case 2:
                break;
            case 3: 
                break;
        }
    }
}
