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
        private const float DelayMult = 0.5f;

        public static void Talk(RequestDialogueEvent e)
        {
            EventBus.Publish(e);
        }

        public static void TalkDeferred(RequestDialogueEvent e, float delay)
        {
            Defer(delay, () => {
                EventBus.Publish(e);
            });
        }

        public static void Defer(float delay, Action a)
        {
            DOVirtual.DelayedCall(delay * DelayMult, () => a());
        }
    }
}
