using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public enum FadeDirection
    {
        In, Out
    }
    public record FadeToEvent(Color FadeColor, float TweenTime, FadeDirection Direction = FadeDirection.In);
}
