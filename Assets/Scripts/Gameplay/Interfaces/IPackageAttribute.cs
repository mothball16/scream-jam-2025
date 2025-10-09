using Assets.Enums;
using System;
using UnityEngine;

// fuck this shit
/// <summary>
/// the non-generic interface with Handler defining what is expected of the generics in 
/// IPackageAttribute(T, U)
/// </summary>
public interface IPackageAttribute 
{
    public AttributeHandler Handler { get; set; }
}
/// <summary>
/// the stuff to be accessed once we are sure of the types
/// rendering is handled by PackageDecorator, which renderer to use is determined by the handler
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPackageAttribute<T, U> : IPackageAttribute
{
    public U RenderParams { get; set; }
    public T Value { get; set; }
    public T DisplayValue { get; set; }
}