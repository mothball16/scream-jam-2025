using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Attributes
{
    internal class From : IPackageAttribute<string, ToStringParams>
    {
        public AttributeHandler Handler { get; set; } = AttributeHandler.String;
        public ToStringParams RenderParams { get; set; } = new();
        public string Value { get { return DisplayValue; } set { return; } }
        public string DisplayValue { get; set; }
        public bool IsValid { get; set; }
        
    }
}
