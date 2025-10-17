using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Attributes
{
    internal class OrderID : IPackageAttribute<string, ToStringParams>
    {
        public AttributeHandler Handler { get; set; } = AttributeHandler.String;
        public ToStringParams RenderParams { get; set; } = new("", "");
        public string Value
        {
            get { return DisplayValue; }
            set { }
        }
        public string DisplayValue { get; set; }
    }
}
