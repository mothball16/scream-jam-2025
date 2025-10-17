using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using PAC = PackageAttributeConstraints;
using static UnityEngine.Analytics.IAnalytic;
using Assets.Scripts.Util;
using System.Linq;
/// <summary>
/// The purpose of this script is to create the packages and set up a way to order/script the packages on days
/// 
/// </summary>
public class PackageGenerator : MonoSingleton<PackageGenerator>
{
    public string shipper;
    [SerializeField] private Transform _spawn;
    public GameObject[] _packages;

    /// <summary>
    /// This method spawns the package as the inputs allow
    /// </summary>
    public GameObject SpawnPackage(Package package)
    {
        GameObject packageObject = Instantiate(
                _packages[0],                                        //Prefab Chosen 
                _spawn.position,                                     //Position of Spawn 
                Quaternion.identity);

        packageObject.AddComponent<PackageInfo>().data = new() {
            { typeof(Weight), new Weight { Value = package.WeightPair.x, DisplayValue = package.WeightPair.y } },
            { typeof(From), new From { DisplayValue = package.From} },
            { typeof(ShipTo), new ShipTo { DisplayValue = package.ShipTo} },
            { typeof(ShippingDate), new ShippingDate { Value = package.Date} },
            { typeof(Remarks), new Remarks { DisplayValue = package.Remark} },
            { typeof(OrderID), new OrderID{ DisplayValue = package.ID } },
            { typeof(Shipper), new Shipper{ DisplayValue = package.Shipper } }
        };
        EventBus.Publish(new PackageSpawnedEvent(packageObject));
        return packageObject;
    }

    public Vector2 GenerateGoodWeightPair()
    {
        int weight = (int)Random.Range(PAC.WeightLimits[0], PAC.WeightLimits[1] + 1);
        Vector2 weightPair = new Vector2(weight, weight);
        return weightPair;
    }

    public Vector2 GenerateBadWeightPair()
    {
        int weight = (int)Random.Range(PAC.WeightLimits[0], PAC.WeightLimits[1] + 1);
        Vector2 weightPair = new(weight, weight);
        float roll = Random.Range(0f, 1f);
        if (roll < 0.5)
        {
            weightPair.y += Random.Range(1, PAC.WeightMaxDeviationAmount + 1);
        }
        else
        {
            weightPair.y -= Random.Range(1, PAC.WeightMaxDeviationAmount + 1);
        }
        return weightPair;
    }

    public string GenerateGoodShipper()
    {
        int index;
        index = Random.Range(0, PAC.ValidShippers.Length);
        shipper = PAC.ValidShippers[index];
        return shipper;
    }

    public string GenerateBadShipper()
    {
        int index;
        index = Random.Range(0, PAC.InvalidShippers.Length);
        shipper = PAC.InvalidShippers[index];
        return shipper;
    }

    public string GenerateGoodAddress(Days day)
    {
        string address = "";
        address += Random.Range(PAC.AddressLineOneNumLimits[0], PAC.AddressLineOneNumLimits[1] + 1) + " ";
        address += PAC.AddressLineOneMid[Random.Range(0, PAC.AddressLineOneMid.Length)] + " ";
        address += PAC.AddressLineOneEnd[Random.Range(0, PAC.AddressLineOneEnd.Length)] + " \n";
        var regionIndex = day switch
        {
            Days.DayOne => Random.Range(0, PAC.Regions.Length),
            Days.DayTwo => Random.Range(0, PAC.Regions.Length - 1),
            Days.DayThree => Random.Range(0, PAC.Regions.Length - 2),
            _ => 0,
        };
        address += PAC.Regions[regionIndex] + " ";
        address += Random.Range((regionIndex) * 10000, (regionIndex + 1) * 10000);
        return address;
    }
    public string GenerateBadZipAddress(Days day)
    {
        string address = "";
        address += Random.Range(PAC.AddressLineOneNumLimits[0], PAC.AddressLineOneNumLimits[1] + 1) + " ";
        address += PAC.AddressLineOneMid[Random.Range(0, PAC.AddressLineOneMid.Length)] + " ";
        address += PAC.AddressLineOneEnd[Random.Range(0, PAC.AddressLineOneEnd.Length)] + " \n";
        var regionIndex = day switch
        {
            Days.DayOne => Random.Range(0, PAC.Regions.Length),
            Days.DayTwo => Random.Range(0, PAC.Regions.Length - 1),
            Days.DayThree => Random.Range(0, PAC.Regions.Length - 2),
            _ => 0,
        };
        address += PAC.Regions[regionIndex] + " ";
        address += Random.Range((regionIndex + 1) * 10000, (regionIndex + 5) * 10000);
        return address;
    }

    public string GenerateBadRegionAddress(Days day)
    {
        string address = "";
        address += Random.Range(PAC.AddressLineOneNumLimits[0], PAC.AddressLineOneNumLimits[1] + 1) + " ";
        address += PAC.AddressLineOneMid[Random.Range(0, PAC.AddressLineOneMid.Length)] + " ";
        address += PAC.AddressLineOneEnd[Random.Range(0, PAC.AddressLineOneEnd.Length)] + " \n";
        var regionIndex = day switch
        {
            Days.DayThree => Random.Range(PAC.Regions.Length - 2, PAC.Regions.Length),
            _ => PAC.Regions.Length - 1,
        };
        address += PAC.Regions[regionIndex] + " ";
        address += Random.Range((regionIndex + 2) * 10000, (regionIndex + 5) * 10000);
        return address;
    }

    public int GetCurrentDate(Days day)
    {
        return (int)day + PAC.DayOneDate;
    }

    public int GetBadDate(Days day)
    {
        int deviation = 0;
        while(deviation == 0)
        {
            deviation = Random.Range(-PAC.DateDeviationFromCurrent, PAC.DateDeviationFromCurrent+1);
        }
        return (int)day + deviation + PAC.DayOneDate;
        
    }

    public string GenerateGoodRemark()
    {
        int index = Random.Range(0, PAC.UselessRemarks.Length);
        string remark = ""; 
        if (GameManager.Inst.CurrentDay == Days.DayThree)
        {
            remark = PAC.UselessRemarks[index] + GeneratePhoneNumber();
        }
        else
        {
            remark = PAC.UselessRemarks[index];
        }
        return remark;
    }

    public string GenerateBadRemark()
    {
        int index = Random.Range(0, PAC.ThreateningRemarks.Length);
        string remark = "";
        if (GameManager.Inst.CurrentDay == Days.DayThree)
        {
            remark = PAC.ThreateningRemarks[index] + GeneratePhoneNumber();
        }
        else
        {
            remark = PAC.ThreateningRemarks[index];
        }
        return remark;
    }

    public string GenerateGoodID()
    {
        int index = Random.Range(0, 10);
        string id;
        switch(shipper)
        {
            case ("Amazon"):
                id = PAC.amazonGood[index];
                break;
            case ("WeBay"):
                id = PAC.weBayGood[index];
                break;
            case ("Fexed"):
                id = PAC.fexedGood[index];
                break;
            default:
                id = "" + Math.PI ;
                break;
        }
        return id;
    }

    public string GenerateBadID()
    {
        int index = Random.Range(0, 10);
        string id;
        switch (shipper)
        {
            case ("Amazon"):
                id = PAC.amazonBad[index];
                break;
            case ("WeBay"):
                id = PAC.weBayBad[index];
                break;
            case ("Fexed"):
                id = PAC.fexedBad[index];
                break;
            default:
                id = "" + Math.PI;
                break;
        }
        return id;
    }

    public string GeneratePhoneNumber()
    {
        string phoneNumber;
        phoneNumber = " " + Random.Range(100, 1000) + "-" + Random.Range(1000,10000);
        return phoneNumber;
    }

}
    
