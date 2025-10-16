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
        public static void Talk(RequestDialogueEvent e)
        {
            EventBus.Publish(e);
        }

        public static void TalkDeferred(RequestDialogueEvent e, float delay)
        {
            DOVirtual.DelayedCall(delay, () => {
                EventBus.Publish(e);
            });
        }
    }
}
