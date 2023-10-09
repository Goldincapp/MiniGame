using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.AntiCheat.Random
{
    /// <summary>
    /// Provider for random values.
    /// </summary>
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns a random number between Int32.MinValue, Int32.MaxValue - 1.
        /// </summary>
        /// <returns></returns>
        Int32 RandomInt32();

        /// <summary>
        /// Returns a random number between _Min, _Max - 1.
        /// </summary>
        /// <param name="_Min">Inclusive</param>
        /// <param name="_Max">Exclusive</param>
        /// <returns></returns>
        Int32 RandomInt32(Int32 _Min, Int32 _Max);
    }
}
