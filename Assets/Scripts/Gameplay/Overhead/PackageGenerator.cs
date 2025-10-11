using System;
using Assets.Scripts.Gameplay.Attributes;
using UnityEngine;

/// <summary>
/// The purpose of this script is to create the packages and set up a way to order/script the packages on days
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
    /// This method spawns the package as scripted (nothing is random)
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="dWeight"></param>
    /// <param name="from"></param>
    /// <param name="shipTo"></param>
    /// <param name="date"></param>
    /// <param name="remark"></param>
    /// <param name="id"></param>
    public void SpawnScriptedPackage(int weight, int dWeight, string from, string shipTo, string date, string remark, int id)
    {
        GameObject package = Instantiate(
                _packages[0],                                        //Prefab Chosen 
                _spawn.position,                                     //Position of Spawn 
                Quaternion.identity);

        var info = package.AddComponent<PackageInfo>();
        package.GetComponent<PackageInfo>().data = new() {
            { typeof(Weight), new Weight { Value = weight, DisplayValue = dWeight } },
            { typeof(From), new From { DisplayValue = from} },
            { typeof(ShipTo), new ShipTo { DisplayValue = shipTo} },
            { typeof(ShippingDate), new ShippingDate { Value = date} },
            { typeof(Remarks), new Remarks { DisplayValue = remark} },
            { typeof(OrderID), new OrderID{ DisplayValue = id } }
        };
        EventBus.Publish(new PackageSpawnedEvent(package));
    }

}
    
