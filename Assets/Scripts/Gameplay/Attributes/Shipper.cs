using Assets.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Attributes
{
    internal class Shipper : IPackageAttribute<string, ToImageParams>
    {
        public AttributeHandler Handler { get; set; } = AttributeHandler.StringToImage;
        public ToImageParams RenderParams { get; set; } = new(Color.White);
        public string Value { get { return DisplayValue; } set { return; } }
        public string DisplayValue { get; set; }
        
    }
}
