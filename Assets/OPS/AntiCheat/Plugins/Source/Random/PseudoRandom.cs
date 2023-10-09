using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.AntiCheat.Random
{
    /// <summary>
    /// Uses the System.Random class to generate random values.
    /// Is very fast but predictable under high effort.
    /// </summary>
    public class PseudoRandom : IRandomProvider
    {
        private static readonly System.Random generator = new System.Random();

        /// <summary>
        /// Returns a random number between Int32.MinValue, Int32.MaxValue - 1.
        /// </summary>
        /// <returns></returns>
        public int RandomInt32()
        {
            // Min is inclusive, Max is exclusive.
            return generator.Next(Int32.MinValue, Int32.MaxValue);
        }

        /// <summary>
        /// Returns a random number between _Min, _Max - 1.
        /// </summary>
        /// <param name="_Min">Inclusive</param>
        /// <param name="_Max">Exclusive</param>
        /// <returns></returns>
        public int RandomInt32(Int32 _Min, Int32 _Max)
        {
            // Min is inclusive, Max is exclusive.
            return generator.Next(_Min, _Max);
        }
    }
}
