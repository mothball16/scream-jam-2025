using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Attributes
{
    internal class OrderID : IPackageAttribute<float, ToStringParams>
    {
        public AttributeHandler Handler { get; set; } = AttributeHandler.NumToString;
        public ToStringParams RenderParams { get; set; } = new("", "");
        public float Value
        {
            get { return DisplayValue; }
            set { }
        }
        public float DisplayValue { get; set; }
    }
}
