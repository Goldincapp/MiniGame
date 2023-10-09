using System;
using System.Collections;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Detector
{
    /// <summary>
    /// Custom delegate to receive a callback when an unallowed teleportation was detected!
    /// </summary>
    public delegate void OnTeleportationCheatDetected();

    /// <summary>
    /// Detect when a cheater / hacker manipulated the application itself!
    /// </summary>
    public class TeleportationCheatDetector : ADetector<TeleportationCheatDetector>
    {
        /// <summary>
        /// Number of allowed possible teleporation until real teleporation got detected (Used for false positives).
        /// </summary>
        [Tooltip("Number of allowed possible teleporation until real teleporation got detected (Used for false positives). Recommended: 5")]
        [Range(1, 20)]
        public int AllowedFalsePositives = 5;

        /// <summary>
        /// Amount of False Positives until a cheat got really detected!
        /// </summary>
        public override int FalsePositiveAmount
        {
            get
            {
                return this.AllowedFalsePositives;
            }
        }

        /// <summary>
        /// Amount of checks in a row, where no cheat got detected, to reset the possible teleportation counter.
        /// </summary>
        public override int ResetAmount
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        /// <summary>
        /// Event: Attach an OnTeleportationCheatDetected to get informed when a cheater got detected!
        /// </summary>
        public static event OnTeleportationCheatDetected OnTeleportationCheatDetected;

        /// <summary>
        /// Notify on cheat detected.
        /// </summary>
        protected override void OnCheatDetected()
        {
            base.OnCheatDetected();

            if (OnTeleportationCheatDetected != null)
            {
                OnTeleportationCheatDetected();
            }
        }

        /// <summary>
        /// Clear on destroy.
        /// </summary>
        private void OnDestroy()
        {
            OnTeleportationCheatDetected = null;
        }
    }
}