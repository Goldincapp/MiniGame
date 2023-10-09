using System;

namespace OPS.AntiCheat.Detector
{
    /// <summary>
    /// Custom delegate to receive field cheat detection events.
    /// </summary>
    public delegate void OnFieldCheatDetected();

    /// <summary>
    /// Detect when a cheater / hacker tries to manipulate one of the protected fields.
    /// </summary>
    public sealed class FieldCheatDetector : ADetector<FieldCheatDetector>
    {
        /// <summary>
        /// Amount of False Positives until a cheat got really detected!
        /// Default: 1 - Because there are no false positive in this case.
        /// </summary>
        public override int FalsePositiveAmount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Amount of checks in a row, where no cheat got detected, to reset the possible cheat counter.
        /// Default: Int32.Max - Is not required.
        /// </summary>
        public override int ResetAmount
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        /// <summary>
        /// Event: Attach an OnFieldCheatDetected to get informed when a cheater got detected!
        /// </summary>
        public static event OnFieldCheatDetected OnFieldCheatDetected;

        /// <summary>
        /// Override the cheat detection for custom event.
        /// </summary>
        protected override void OnCheatDetected()
        {
            base.OnCheatDetected();
            if (OnFieldCheatDetected != null)
            {
                OnFieldCheatDetected();
            }
        }

        /// <summary>
        /// Clear on destroy.
        /// </summary>
        private void OnDestroy()
        {
            OnFieldCheatDetected = null;
        }
    }
}