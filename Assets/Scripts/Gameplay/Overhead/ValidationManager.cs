using System.Collections.Generic;
using System;
using UnityEngine;
using Assets.Scripts.Gameplay.Attributes;
using System.Linq;

/// <summary>
/// Determines whether a processed package met criteria.
/// </summary>
public class ValidationManager : MonoBehaviour
{
    private List<IDisposable> _disposables;

    private void OnEnable()
    {

        _disposables = new()
        {
            EventBus.Subscribe<PackageProcessedEvent>(OnPackageProcessed)
        };
    }

    // shitty package validaotr
    private void OnPackageProcessed(PackageProcessedEvent e)
    {
        int citationWeight = 0;
        List<string> citations = new();
        var data = e.Info.data;
        Weight weight = default;
        Shipper shipper = default;
        From from = default;
        ShipTo shipTo = default;
        OrderID orderID = default;
        Remarks remarks = default;
        ShippingDate shippingDate = default;

        if(data.TryGetValue(typeof(Weight), out var a))
        {
            weight = a as Weight;
            if (weight.DisplayValue != weight.Value)
            {
                citations.Add("PACKAGE WEIGHT DOES NOT MATCH PACKAGE LABEL");
                citationWeight += 1;
            }
        }

        if (data.TryGetValue(typeof(Shipper), out var b))
        {
            shipper = b as Shipper;
            if (PackageAttributeConstraints.ValidShippers.Contains(shipper.Value))
            {
                citations.Add("FORGED/INVALID SHIPPING COMPANY");
                citationWeight += 1;
            }
        }

        if (data.TryGetValue(typeof(From), out var c))
        {
            from = c as From;
            if (PackageAttributeConstraints.BannedShipFromOnThirdDay.Contains(from.Value))
            {
                citations.Add("BLACKLISTED SENDER");
                citationWeight += 2;
            }
            else if (!from.IsValid)
            {
                citations.Add("INVALID ZIP-CODE");
                citationWeight += 1;
            }
        }

        if (data.TryGetValue(typeof(ShipTo), out var d))
        {
            shipTo = d as ShipTo;
            if (PackageAttributeConstraints.BannedShipToOnSecondDay.Contains(shipTo.Value))
            {
                citations.Add("BLACKLISTED RECIPIENT");
                citationWeight += 2;
            }
            else if (!shipTo.IsValid)
            {
                citations.Add("INVALID ZIP-CODE");
                citationWeight += 1;
            }
        }

        if (data.TryGetValue(typeof(OrderID), out var f) && data.TryGetValue(typeof(Shipper), out var s))
        {
            orderID = f as OrderID;
            // WHY ISNT THE OTEHR VAR WOKING
            var ship = s as Shipper;
            if (ship.Value == "Amazon" && orderID.DisplayValue % 2 == 1)
            {
                citations.Add("INVALID AMAZOA ORDER ID");
                citationWeight += 1;
            } 
            else if(ship.Value == "WeBay" && (orderID.DisplayValue < 100000 || orderID.DisplayValue > 999999))
            {
                citations.Add("INVALID WEBAY ORDER ID");
                citationWeight += 1;
            }
            else if(ship.Value == "Fexed" && (orderID.DisplayValue % 10 != 4))
            {
                citations.Add("INVALID FEXED ORDER ID");
                citationWeight += 1;
            }
        }

        if (data.TryGetValue(typeof(Remarks), out var g))
        {
            remarks = g as Remarks;
            if (PackageAttributeConstraints.ThreateningRemarks.Contains(remarks.Value))
            {
                citations.Add("THREATENING/SUSPICIOUS PACKAGE REMARKS");
                citationWeight += 2;
            }
        }

        if (data.TryGetValue(typeof(ShippingDate), out var h))
        {
            shippingDate = h as ShippingDate;
            int days = Convert.ToInt32(shippingDate.Value);
            if (PackageAttributeConstraints.DayOneDate < days)
            {
                citations.Add("INVALID SHIPPING DATE");
                citationWeight += 1;
            }
        }

        Destroy(e.Info);


        if(citationWeight > 0)
        {
            EventBus.Publish(new IncorrectChoiceEvent(citationWeight, citations));
        } else
        {
            EventBus.Publish(new CorrectChoiceEvent());
        }
    }

    private void OnDisable()
    {
        _disposables.ForEach(disposable => disposable.Dispose());
        _disposables.Clear();
    }
}
