using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using PAC = PackageAttributeConstraints;
/// <summary>
/// The purpose of this script is to create the packages and set up a way to order/script the packages on days
/// 
/// </summary>
public class PackageGenerator : MonoBehaviour
{

    [SerializeField] private int _packagesLeft;
    [SerializeField] private Transform _spawn;
    public GameObject[] _packages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This method spawns the package as the inputs allow
    /// </summary>
    public void SpawnPackage(Package package)
    {
        GameObject packageObject = Instantiate(
                _packages[0],                                        //Prefab Chosen 
                _spawn.position,                                     //Position of Spawn 
                Quaternion.identity);

        var info = packageObject.AddComponent<PackageInfo>();
        packageObject.GetComponent<PackageInfo>().data = new() {
            { typeof(Weight), new Weight { Value = package.Weight, DisplayValue = package.DisplayWeight } },
            { typeof(From), new From { DisplayValue = package.From} },
            { typeof(ShipTo), new ShipTo { DisplayValue = package.ShipTo} },
            { typeof(ShippingDate), new ShippingDate { Value = package.Date} },
            { typeof(Remarks), new Remarks { DisplayValue = package.Remark} },
            { typeof(OrderID), new OrderID{ DisplayValue = package.ID } },
            { typeof(Shipper), new Shipper{ DisplayValue = package.Shipper } }
        };
        EventBus.Publish(new PackageSpawnedEvent(packageObject));
    }
    /// <summary>
    /// Generates a random weightPair (ActualWeight,DisplayWeight)
    /// </summary>
    /// <returns>Vector2 weightPair (ActualWeight,DisplayWeight)</returns>
    public Vector2 GenerateRandomWeight()
    {
        int weight = (int)Random.Range(PAC.WeightLimits[0], PAC.WeightLimits[1] + 1);
        Vector2 weightPair = new Vector2(weight, weight);
        float roll = Random.Range(0f, 1f);
        if (roll < 0.25)
        {
            if (roll < 0.13)
            {
                weightPair.y += Random.Range(1, PAC.WeightMaxDeviationAmount + 1);
            }
            else
            {
                weightPair.y -= Random.Range(1, PAC.WeightMaxDeviationAmount + 1);
            }
        }
        return weightPair;
    }
    /// <summary>
    /// Generates a random weightPair (ActualWeight,DisplayWeight)
    /// </summary>
    /// <param name="num">The weight value you want</param>
    /// <returns>Vector2 weightPair (ActualWeight,DisplayWeight)</returns>
    public Vector2 GenerateRandomWeight(int num)
    {
        int weight = num;
        Vector2 weightPair = new Vector2(weight, weight);
        float roll = Random.Range(0f, 1f);
        if (roll < 0.25)
        {
            if (roll < 0.13)
            {
                weightPair.y += Random.Range(1, PAC.WeightMaxDeviationAmount + 1);
            }
            else
            {
                weightPair.y -= Random.Range(1, PAC.WeightMaxDeviationAmount + 1);
            }
        }
        return weightPair;
    }
}
    
