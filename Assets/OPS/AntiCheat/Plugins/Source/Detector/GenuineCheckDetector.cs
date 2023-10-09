using System;
using System.Collections;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Detector
{
    /// <summary>
    /// Custom delegate to receive a callback when the genuine check failed!
    /// </summary>
    public delegate void OnGenuineCheckFailedDetected();

    /// <summary>
    /// Detect when a cheater / hacker manipulated the application itself!
    /// </summary>
    public class GenuineCheckDetector : ADetector<GenuineCheckDetector>
    {
        /// <summary>
        /// Enable to check if the application is genuine only on detector start. The genuine check can be resource intensive. Disable to check in a define interval.
        /// </summary>
        [Tooltip("Enable to check if the application is genuine only on detector start. The genuine check can be resource intensive. Disable to check in a define interval. Recommended: True")]
        public bool CheckGenuineOnlyOnGameStart = true;

        /// <summary>
        /// Interval in seconds in which to check the genuine of the application.
        /// </summary>
        [Tooltip("Interval in seconds in which to check the genuine of the application. Recommended: 60")]
        [Range(0.001f, 600f)]
        public float RecheckIntervalForPossibleCheating = 60f;

        /// <summary>
        /// Amount of False Positives until a cheat got really detected!
        /// </summary>
        public override int FalsePositiveAmount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Amount of checks in a row, where no cheat got detected, to reset the possible speeding counter.
        /// </summary>
        public override int ResetAmount
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        /// <summary>
        /// Event: Attach an OnGenuineCheckFailedDetected to get informed when a cheater got detected!
        /// </summary>
        public static event OnGenuineCheckFailedDetected OnGenuineCheckFailedDetected;

        /// <summary>
        /// Unity start hook. Run the genuine check coroutine.
        /// </summary>
        private void Start()
        {
            // Manually check for genuine on game start once.
            if(this.CheckGenuineOnlyOnGameStart)
            {
                this.ManualGenuineCheck();
            }

            // Repeating genuine check - Start only when genuine check is available!
            if(Application.genuineCheckAvailable)
            {
                // Start checking coroutine.
                this.StartCoroutine(this.CheckGenuine());
            }
        }

        /// <summary>
        /// Runs in the interval of the recheck field.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckGenuine()
        {
            while (true)
            {
                if(!this.CheckGenuineOnlyOnGameStart)
                {
                    if (Application.genuine)
                    {
                        this.PossibleCheatDetected = false;
                    }
                    else
                    {
                        this.PossibleCheatDetected = true;
                    }
                }

                yield return new WaitForSecondsRealtime(this.RecheckIntervalForPossibleCheating);
            }
        }

        /// <summary>
        /// Manually check for genuine and return if tampering was detected.
        /// </summary>
        /// <returns></returns>
        public bool ManualGenuineCheck()
        {
            if (Application.genuineCheckAvailable)
            {
                if (Application.genuine)
                {
                    this.PossibleCheatDetected = false;
                }
                else
                {
                    this.PossibleCheatDetected = true;
                }
            }

            return this.PossibleCheatDetected;
        }

        /// <summary>
        /// Notify on cheat detected.
        /// </summary>
        protected override void OnCheatDetected()
        {
            base.OnCheatDetected();

            if (OnGenuineCheckFailedDetected != null)
            {
                OnGenuineCheckFailedDetected();
            }
        }

        /// <summary>
        /// Clear on destroy.
        /// </summary>
        private void OnDestroy()
        {
            this.StopAllCoroutines();

            OnGenuineCheckFailedDetected = null;
        }
    }
}