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
    void Start()
    {
        packages = new Queue<Package>();
        switch (1)
        {
            case 1:
                Vector2 weights = PackageGenerator.GenerateRandomWeight(5);
                packages.Enqueue(new Package(true,(int)weights.x, (int)weights.y, "Ben","Joe",PAC.DayOneDate.ToString(),"I sure hope nothing bad happens today",304948,"Amazon"));
                packages.Enqueue(new Package(true,8, 8, "Tom", "Tim", (PAC.DayOneDate+3).ToString(), "Why is the date a string but its a number", 4892857, "Fexed"));
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
    }

    private void CheckPackage()
    {
        if(_packagesLeft > 0 && CountObjectsWithTag("Package") == 0)
        {
            PackageGenerator.SpawnPackage(packages.Dequeue());
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
        switch (1)
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
