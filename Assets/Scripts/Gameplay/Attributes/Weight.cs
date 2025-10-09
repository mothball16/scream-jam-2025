using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Attributes
{
    internal class Weight : IPackageAttribute<float, ToStringParams>
    {
        private static ToStringParams RealRenderParams { get; } = new("", "lbs");
        public AttributeHandler Handler { get; set; } = AttributeHandler.NumToString;
        public ToStringParams RenderParams { get; set; } = new("", "lbs");
        public float Value { get; set; }
        public float DisplayValue { get; set; }
        
    }
}
