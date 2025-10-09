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
    [SerializeField] private int _packagesLeft;
    [SerializeField] private Transform _spawn;
    public GameObject[] _packages;

    private void SpawnPackage()
    {
        GameObject package = Instantiate(
                _packages[0],                                        //Prefab Chosen 
                _spawn.position,                                     //Position of Spawn 
                Quaternion.identity);

        var info = package.AddComponent<PackageInfo>();
        package.GetComponent<PackageInfo>().data = new() {
            { typeof(Weight), new Weight { Value = 8, DisplayValue = 7 } },
            { typeof(From), new From { DisplayValue = "123 street\nunit 1\nnew york NY\n10304"} },
            { typeof(ShipTo), new ShipTo { DisplayValue = "123 street\nunit 1\nnew york NY\n10304"} },
            { typeof(ShippingDate), new ShippingDate { Value = Convert.ToString(PackageAttributeConstraints.DayOneDate + 1)} },
            { typeof(Remarks), new Remarks { DisplayValue = "pass my shit"} },
            { typeof(OrderID), new OrderID{ DisplayValue = 124449284 } }
        };


        EventBus.Publish(new PackageSpawnedEvent(package));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPackage();
    }

    public int GetRemainingPackages() => _packagesLeft;

    private void CheckPackage()
    {
        if(_packagesLeft > 0 && CountObjectsWithTag("Package") == 0)
        {
            SpawnRandomPackage();
            _packagesLeft--;
        }
    }
    public int CountObjectsWithTag(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        return taggedObjects.Length;
    }

    private void SpawnRandomPackage()
    {
        GameObject package = Instantiate(
                _packages[0],                                        //Prefab Chosen 
                _spawn.position,                                     //Position of Spawn 
                Quaternion.identity);

        int chance = Random.Range(1, 101);
        int weight = Random.Range(PAC.WeightLimits[0], PAC.WeightLimits[1]+1);
        int displayWeight = weight;

        if(chance <= 25)
        {
            displayWeight += (int) Random.Range(-PAC.WeightMaxDeviationAmount, PAC.WeightMaxDeviationAmount);
        }
        
        int addressFromNum = Random.Range(1, 1000);
        int addressToNum = Random.Range(1, 1000);

        var info = package.AddComponent<PackageInfo>();
        package.GetComponent<PackageInfo>().data = new() {
            { typeof(Weight), new Weight { Value = weight, DisplayValue = displayWeight } },
            { typeof(From), new From { DisplayValue = "123 street\nunit 1\nnew york NY\n10304"} },
            { typeof(ShipTo), new ShipTo { DisplayValue = "123 street\nunit 1\nnew york NY\n10304"} },
            { typeof(ShippingDate), new ShippingDate { Value = Convert.ToString(PackageAttributeConstraints.DayOneDate + 1)} },
            { typeof(Remarks), new Remarks { DisplayValue = "pass my shit"} },
            { typeof(OrderID), new OrderID{ DisplayValue = 124449284 } }
        };

        EventBus.Publish(new PackageSpawnedEvent(package));
    }
    
}
