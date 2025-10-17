using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gameplay.Interfaces
{
    internal interface IScript
    {
        int MaxViolations { get; }
        int AvailablePages { get; }
        Action Script { get; }
    }
}
