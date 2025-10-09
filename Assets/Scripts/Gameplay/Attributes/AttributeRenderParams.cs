using System.Drawing;

namespace Assets.Scripts.Gameplay.Attributes
{
    // pretty obvious: label is rendered onto as prefix + disp. value + suffix
    internal record ToStringParams(string Prefix = "", string Suffix = "") : IRenderParams;
    // the sprite itself is cached on the decorator, string is for lookup
    internal record ToImageParams(Color Tint) : IRenderParams;
}