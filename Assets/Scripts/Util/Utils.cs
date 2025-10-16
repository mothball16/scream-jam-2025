using Assets.Scripts.Events;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Util
{
    /// <summary>
    /// i HATE TYPING
    /// </summary>
    internal static class Utils
    {
        private const float DelayMult = 1;

        public static void Talk(RequestDialogueEvent e)
        {
            EventBus.Publish(e);
        }

        public static void TalkDeferred(float delay, RequestDialogueEvent e)
        {
            Defer(delay, () => {
                EventBus.Publish(e);
            });
        }

        public static void Defer(float delay, Action a)
        {
            DOVirtual.DelayedCall(delay * DelayMult, () => a());
        }

        public static void Fade(FadeToEvent e)
        {
            EventBus.Publish(e);
        }
    }
}
