using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// OPS - AntiCheat
using OPS.AntiCheat.Random;

namespace OPS.AntiCheat.Field
{
    /// <summary>
    /// Global settings for all protected fields.
    /// </summary>
    public static class ProtectedFieldsSettings
    {
        /// <summary>
        /// The provider for random numbers.
        /// Default: TrueRandom
        /// </summary>
        public static IRandomProvider RandomProvider = new TrueRandom();
    }
}
