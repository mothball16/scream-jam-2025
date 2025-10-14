using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Assets.Enums;
using UnityEngine.UI;
using System.Linq;



[System.Serializable]
public class SpriteLink
{
    public string Name;
    public Sprite Image;
}

/// <summary>
/// Handles the rendering of package attributes and other related stuff.
/// </summary>
public class PackageDecorator : MonoBehaviour
{
    private List<IDisposable> _disposables;
    [SerializeField] private List<SpriteLink> _sprites;

    private Dictionary<AttributeHandler, Action<Transform, IPackageAttribute>> _handlers;
    private void Awake()
    {
         _handlers = new()
        {
            {
                AttributeHandler.NumToString, (obj, attrib)
                => RenderFloatToString(obj.GetComponent<Text>(),
                    attrib as IPackageAttribute<float, ToStringParams>)
            },
            {
                AttributeHandler.String, (obj, attrib)
                => RenderString(obj.GetComponent<Text>(),
                    attrib as IPackageAttribute<string, ToStringParams>)
            },
            {
                AttributeHandler.StringToImage, (obj, attrib)
                => RenderImage(obj.GetComponent<Image>(),
                    attrib as IPackageAttribute<string, ToImageParams>)
            }
        };

    }

    private void OnEnable()
    {
        _disposables = new()
        {
            EventBus.Subscribe<PackageSpawnedEvent>(OnPackageSpawned)
        };
    }

    private void OnDisable() 
        => _disposables.ForEach(s =>  s.Dispose());

    /// <summary>
    /// check and call if renderer for attribute handler type exists
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="attrib"></param>
    private void RenderLabel(Transform obj, IPackageAttribute attrib)
    {
        if(!_handlers.TryGetValue(attrib.Handler, out var handler))
            throw new KeyNotFoundException();
        handler.Invoke(obj, attrib);
    }


    private void OnPackageSpawned(PackageSpawnedEvent e)
    {
        var canvas = e.Package.transform.Find("Canvas");
        if (!e.Package.TryGetComponent<PackageInfo>(out var info) 
            || canvas == null)
            throw new KeyNotFoundException();

        foreach (var kvp in info.data)
        {
            var label = canvas.transform.Find(kvp.Key.Name);
            if (label != null)
            {
                Debug.Log(label.ToString());
                RenderLabel(label, kvp.Value);
            } else
            {
                Debug.LogError($"no label found for {kvp.Key.Name}");
            }
        }
    }

    #region - - - [ attrib handlers ] - - -
    private static void RenderFloatToString(Text label, IPackageAttribute<float, ToStringParams> attrib)
    {
        label.text = $"{attrib.RenderParams.Prefix}{attrib.DisplayValue}{attrib.RenderParams.Suffix}";
    }

    private static void RenderString(Text label, IPackageAttribute<string, ToStringParams> attrib)
    {
        label.text = $"{attrib.RenderParams.Prefix}{attrib.DisplayValue}{attrib.RenderParams.Suffix}";
    }

    private void RenderImage(Image image, IPackageAttribute<string, ToImageParams> attrib)
    {
        image.sprite = _sprites.First(l => l.Name == attrib.DisplayValue).Image;
    }
    #endregion
}