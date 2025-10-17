using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public record SetTimedEvent(string ID, float Time, Action Callback);

    public record CancelTimedEvent(string ID);
}
