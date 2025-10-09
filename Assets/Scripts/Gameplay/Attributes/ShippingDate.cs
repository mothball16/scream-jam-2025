using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Attributes
{
    internal class ShippingDate : IPackageAttribute<string, ToStringParams>
    {
        public AttributeHandler Handler { get; set; } = AttributeHandler.String;
        public ToStringParams RenderParams { get; set; } = new("", "");
        public string Value { get; set; }
        public string DisplayValue
        {
            get
            {
                return new DateTime(1970, 1, 1).AddDays(Convert.ToInt32(Value)).ToString("d");
            }
            set
            {

            }
        }

    }
}
