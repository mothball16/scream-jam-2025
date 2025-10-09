using Assets.Scripts.Gameplay.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ScaleTool : MonoBehaviour
{
    private float _lastUpd = 5;
    [SerializeField] private Text _display;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PackageInfo>(out var info))
        {
            if (info.data.TryGetValue(typeof(Weight), out var attrib))
            {
                _display.text = $"{(attrib as Weight).Value} LBS";
            }
            else
            {
                _display.text = $"ERR";
            }

            if (_lastUpd > 5)
                EventBus.Publish(new ScalePressedEvent());
            
            _lastUpd = 0;

            
        }
    }

    private void Update()
    {
        _lastUpd += Time.deltaTime;
        if (_lastUpd > 5)
            _display.text = "0 LBS";
    }
}
