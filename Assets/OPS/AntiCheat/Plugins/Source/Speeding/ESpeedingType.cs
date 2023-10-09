using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.AntiCheat.Speed
{
    /// <summary>
    /// Recognized type of speeding by a cheater / hacker.
    /// </summary>
    public enum ESpeedingType : byte
    {
        Stopped = 0,
        SlowedDown = 1,
        SpeedUp = 2
    }
}