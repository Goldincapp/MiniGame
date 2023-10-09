using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.AntiCheat.Random
{
    /// <summary>
    /// Uses the System.Security.Cryptography.RNGCryptoServiceProvider class to generate random values.
    /// It is slower than System.Random but unpredictable because of its crypto-strength seed.
    /// </summary>
    public class TrueRandom : IRandomProvider
    {
        private static readonly System.Security.Cryptography.RandomNumberGenerator generator = new System.Security.Cryptography.RNGCryptoServiceProvider();

        /// <summary>
        /// Returns a random number between Int32.MinValue, Int32.MaxValue - 1.
        /// </summary>
        /// <returns></returns>
        public int RandomInt32()
        {
            return this.RandomInt32(Int32.MinValue, Int32.MaxValue);
        }

        /// <summary>
        /// Returns a random number between _Min, _Max - 1.
        /// </summary>
        /// <param name="_Min">Inclusive</param>
        /// <param name="_Max">Exclusive</param>
        /// <returns></returns>
        public int RandomInt32(Int32 _Min, Int32 _Max)
        {
            // Used for the random bytes.
            byte[] var_Bytes = new byte[sizeof(Int32)];

            // Get the random bytes.
            generator.GetBytes(var_Bytes);

            // Convert the random bytes to an Int32 value.
            Int32 var_Value = BitConverter.ToInt32(var_Bytes, 0);

            // Make sure is between min/max.
            if(var_Value < _Min)
            {
                var_Value = _Min;
            }
            else if (var_Value > _Max - 1)
            {
                var_Value = _Max - 1;
            }

            return var_Value;
        }
    }
}
