using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

// Unity
using UnityEngine;

// OPS - AntiCheat
using OPS.AntiCheat.Speed;

namespace OPS.AntiCheat.Detector
{
    /// <summary>
    /// Custom delegate to receive computer time hack detection events.
    /// </summary>
    public delegate void OnComputerTimeHackDetected();

    /// <summary>
    /// Detect when a cheater / hacker tries to manipulate the computer time.
    /// </summary>
    public class ComputerTimeHackDetector : ADetector<ComputerTimeHackDetector>
    {
        /// <summary>
        /// Allowed time difference in seconds between the game start time plus runtime compared to the realtime until a Hacker get detected.
        /// </summary>
        [Tooltip("Allowed time difference in seconds between the game start time plus runtime compared to the realtime until a Hacker get detected.")]
        [Range(2f, 25f)]
        public float TimeToleranceUntilPossibleHackDetection = 5f;

        /// <summary>
        /// Interval in seconds in which to check for possible cheats. Used to find a realistic number of false positives.
        /// </summary>
        [Tooltip("Interval in seconds in which to check for possible cheats. Used to find a realistic number of false positives. Recommended: 5")]
        [Range(0.001f, 60f)]
        public float RecheckIntervalForPossibleCheating = 5f;

        /// <summary>
        /// Number of allowed possible speedings until real speeding got detected (Used for false positives).
        /// </summary>
        [Tooltip("Number of allowed possible speedings until real speeding got detected (Used for false positives). Recommended: 5")]
        [Range(1, 20)]
        public int AllowedFalsePositives = 2;

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

        [Tooltip("Number of positive checks in a row, until the possible time cheat counter (Allowed false positives) getting reset. Recommended: 30")]
        [Range(1, 100)]
        public int PositiveChecksUntilFalsePositiveReset = 30;

        /// <summary>
        /// Amount of checks in a row, where no cheat got detected, to reset the possible computer time modification counter.
        /// </summary>
        public override int ResetAmount
        {
            get
            {
                return this.PositiveChecksUntilFalsePositiveReset;
            }
        }

        [Tooltip("Enable to read the current UTC time from the internet. Requires that the user has internet! Else the UTC time will be calculated based on the system ticks.")]
        public bool ForceInternetTime = false;

        /// <summary>
        /// System time when the runtime got started.
        /// </summary>
        private DateTime systemRuntimeStartDateTime;

        /// <summary>
        /// Difference in seconds between the unity runtime start and start of this gameobject.
        /// </summary>
        private float differenceToStart;

        /// <summary>
        /// After recognizing a possible cheat, this field will be filled with the difference between the game time and real time.
        /// Required, because not each possible cheat is a real cheating attempt. Maybe a system lagged or got focused / unfocused.
        /// </summary>
        private float balancing;

        /// <summary>
        /// Contains the compare date time. Updated every time, when checking for possible cheats.
        /// </summary>
        public DateTime CompareDateTime { get; private set; }

        /// <summary>
        /// Event: Attach an OnComputerTimeHackDetected to get informed when a cheater got detected!
        /// </summary>
        public static event OnComputerTimeHackDetected OnComputerTimeHackDetected;

        /// <summary>
        /// Unity start hook. Assign the start times, and run cheat / hack check coroutine.
        /// </summary>
        private void Start()
        {
            // Init
            this.systemRuntimeStartDateTime = DateTime.UtcNow;
            this.differenceToStart = Time.realtimeSinceStartup;
            this.balancing = 0;

            // Start checking coroutine.
            this.StartCoroutine(this.CheckComputerTime());
        }

        /// <summary>
        /// Runs in the interval of the recheck field.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckComputerTime()
        {
            while (true)
            {
                // Either internet or local system time.
                if (this.ForceInternetTime)
                {
                    this.CompareDateTime = this.GetInternetCompareTime();
                }
                else
                {
                    this.CompareDateTime = this.GetLocalCompareTime();
                }

                // System time now.
                DateTime var_DateTimeNow = DateTime.UtcNow;

                // Get the time difference between system now and compare time.
                System.TimeSpan var_Difference = var_DateTimeNow - this.CompareDateTime;

                // Get total difference in seconds and cast to float (unity uses always float instead of double).
                float var_TotalDifferenceInSeconds = (float)var_Difference.TotalSeconds;

                // Check if a possible cheat was found!
                if (Mathf.Abs(var_TotalDifferenceInSeconds) > this.TimeToleranceUntilPossibleHackDetection)
                {
                    this.PossibleCheatDetected = true;

                    // Assign balance.
                    this.balancing = var_TotalDifferenceInSeconds;
                }
                else
                {
                    this.PossibleCheatDetected = false;
                }

                yield return new WaitForSecondsRealtime(this.RecheckIntervalForPossibleCheating);
            }
        }

        /// <summary>
        /// Notify on cheat detected.
        /// </summary>
        protected override void OnCheatDetected()
        {
            base.OnCheatDetected();

            if (OnComputerTimeHackDetected != null)
            {
                OnComputerTimeHackDetected();
            }
        }

        /// <summary>
        /// Return a calculated utc date time based on the game start time and past system ticks.
        /// </summary>
        /// <returns></returns>
        private DateTime GetLocalCompareTime()
        {
            return this.systemRuntimeStartDateTime.AddSeconds(UnityEngine.Time.realtimeSinceStartup - this.differenceToStart + this.balancing);
        }

        /// <summary>
        /// Return a utc date time based on a nist server.
        /// </summary>
        /// <returns></returns>
        private DateTime GetInternetCompareTime()
        {
            try
            {
                return this.GetUtcNistTime().AddSeconds(this.balancing);
            }
            catch
            {
                return this.GetLocalCompareTime();
            }
        }

        /// <summary>
        /// Returns the Utc time from a nist server.
        /// </summary>
        /// <returns></returns>
        private DateTime GetUtcNistTime(String _Server = "time.nist.gov", int _Port = 13)
        {
            TcpClient var_Client = new TcpClient(_Server, _Port);
            using (StreamReader var_Reader = new StreamReader(var_Client.GetStream()))
            {
                String var_Response = var_Reader.ReadToEnd();
                String var_UtcDateTimeString = var_Response.Substring(7, 17);
                DateTime var_DateTime = DateTime.ParseExact(var_UtcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                return DateTime.SpecifyKind(var_DateTime, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Clear on destroy.
        /// </summary>
        private void OnDestroy()
        {
            this.StopAllCoroutines();

            OnComputerTimeHackDetected = null;
        }
    }
}