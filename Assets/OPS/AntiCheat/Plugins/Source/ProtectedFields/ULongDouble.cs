using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OPS.AntiCheat.Field
{
    /// <summary>
    /// Helper class to parse double to long and the other way around.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct ULongDouble
    {
        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong longValue;
    }
}
