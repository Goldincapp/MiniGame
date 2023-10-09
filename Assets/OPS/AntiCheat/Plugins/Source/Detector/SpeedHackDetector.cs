using System;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

// OPS - AntiCheat
using OPS.AntiCheat.Speed;

namespace OPS.AntiCheat.Detector
{
    /// <summary>
    /// Custom delegate to receive speed hack detection events.
    /// </summary>
    public delegate void OnSpeedHackDetected(ESpeedingType _SpeedingType);

    /// <summary>
    /// Detect when a cheater / hacker tries to manipulate the game speed.
    /// </summary>
    public class SpeedHackDetector : ADetector<SpeedHackDetector>
    {
        /// <summary>
        /// Allowed Time difference in seconds between 'Game Time' and 'Real Time' until possible speeding got detected.
        /// </summary>
        [Tooltip("Allowed Time difference in seconds between 'Game Time' and 'Real Time' until possible speeding got detected. Recommended: 0.1")]
        [Range(0.001f, 2f)]
        public float TimeToleranceUntilPossibleHackDetection = 0.1f;

        /// <summary>
        /// Interval in seconds in which to check for possible cheats. Used to find a realistic number of false positives.
        /// </summary>
        [Tooltip("Interval in seconds in which to check for possible cheats. Used to find a realistic number of false positives. Recommended: 0.5")]
        [Range(0.001f, 2f)]
        public float RecheckIntervalForPossibleCheating = 0.5f;

        /// <summary>
        /// Number of allowed possible speedings until real speeding got detected (Used for false positives).
        /// </summary>
        [Tooltip("Number of allowed possible speedings until real speeding got detected (Used for false positives). Recommended: 5")]
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

        [Tooltip("Number of positive checks in a row, until the possible speedings counter (Allowed false positives) getting reset. Recommended: 30")]
        [Range(1, 100)]
        public int PositiveChecksUntilFalsePositiveReset = 30;

        /// <summary>
        /// Amount of checks in a row, where no cheat got detected, to reset the possible speeding counter.
        /// </summary>
        public override int ResetAmount
        {
            get
            {
                return this.PositiveChecksUntilFalsePositiveReset;
            }
        }

        /// <summary>
        /// Recognized speeding type by cheater / hacker.
        /// </summary>
        private ESpeedingType recognizedSpeedType;

        /// <summary>
        /// Event: Attach an OnSpeedHackDetected to get informed when a cheater got detected!
        /// </summary>
        public static event OnSpeedHackDetected OnSpeedHackDetected;

        /// <summary>
        /// A list of unity delta times.
        /// </summary>
        private List<float> deltaTimeList = new List<float>();

        /// <summary>
        /// Returns the mean of the delta time list.
        /// </summary>
        /// <returns></returns>
        private float MeanDeltaTime()
        {
            if (this.deltaTimeList.Count == 0)
            {
                return 0;
            }

            float var_Value = 0;
            for (int i = 0; i < this.deltaTimeList.Count; i++)
            {
                var_Value += this.deltaTimeList[i];
            }

            var_Value /= this.deltaTimeList.Count;

            return var_Value;
        }

        /// <summary>
        /// Utc time of previous tick.
        /// </summary>
        private long previousUtcTime;

        /// <summary>
        /// Utc time of previous checked tick.
        /// </summary>
        private long previousCheckedUtcTime;

        /// <summary>
        /// A list of system utc delta times.
        /// </summary>
        private List<float> utcTimeList = new List<float>();

        /// <summary>
        /// Returns the mean of the system utc delta time list.
        /// </summary>
        /// <returns></returns>
        private float MeanUtcTime()
        {
            if (this.utcTimeList.Count == 0)
            {
                return 0;
            }

            float var_Value = 0;
            for (int i = 0; i < this.utcTimeList.Count; i++)
            {
                var_Value += this.utcTimeList[i];
            }

            var_Value /= this.utcTimeList.Count;

            return var_Value;
        }

        // Unity time properties.
        internal float time { get; private set; }
        internal float deltaTime { get; private set; }
        internal float unscaledTime { get; private set; }
        internal float unscaledDeltaTime { get; private set; }
        internal float realtimeSinceStartup { get; private set; }
        internal float timeSinceLevelLoad { get; private set; }
        internal float timeScale { get { return Time.timeScale; } }

        /// <summary>
        /// Unity start hook. Set the first Utc time of previous tick.
        /// </summary>
        private void Start()
        {
            this.previousUtcTime = DateTime.UtcNow.Ticks;
            this.previousCheckedUtcTime = this.previousUtcTime;
        }

        /// <summary>
        /// Unity update hook. Check here for speeding.
        /// </summary>
        private void Update()
        {
            // Calculate the passed ticks.
            long var_UtcTimeNow = DateTime.UtcNow.Ticks;
            long var_SpanUtcTime = var_UtcTimeNow - this.previousUtcTime;

            // Reset Utc previous tick.
            this.previousUtcTime = var_UtcTimeNow;

            // Check if enough time passed since last check.
            if(this.TickToSec(var_UtcTimeNow - this.previousCheckedUtcTime) >= this.RecheckIntervalForPossibleCheating)
            {
                // Reset Utc previous check tick.
                this.previousCheckedUtcTime = this.previousUtcTime;

                // Count of list entries, for the mean calcuation.
                int var_CountAmount = 10;

                // Increase system delta time counter.
                this.utcTimeList.Add(this.TickToSec(var_SpanUtcTime) * this.timeScale);
                if (this.utcTimeList.Count > var_CountAmount)
                {
                    this.utcTimeList.RemoveAt(0);
                }

                // Increase unity delta time counter.
                this.deltaTimeList.Add(Time.deltaTime);
                if (this.deltaTimeList.Count > var_CountAmount)
                {
                    this.deltaTimeList.RemoveAt(0);
                }

                // If a cheat got detected calculate time based on system ticks.
                if (this.CheatDetected)
                {
                    //Speed Hack got detected, so use self calculated!
                    this.unscaledDeltaTime = this.TickToSec(var_SpanUtcTime);
                    this.unscaledTime += this.unscaledDeltaTime;
                    this.realtimeSinceStartup += this.unscaledDeltaTime;

                    this.deltaTime = this.unscaledDeltaTime * this.timeScale;
                    this.time += this.deltaTime;
                    this.timeSinceLevelLoad += this.deltaTime;
                }
                else
                {
                    // No cheat / hack got detected, so synchronize calculated time with unity time.
                    this.SynchronizationWithUnityTime();

                    // Check if speed hack detected!
                    if (this.deltaTimeList.Count >= var_CountAmount)
                    {
                        if (Mathf.Abs(this.MeanDeltaTime() - this.MeanUtcTime()) > this.TimeToleranceUntilPossibleHackDetection)
                        {
                            this.PossibleCheatDetected = true;
                        }
                        else
                        {
                            this.PossibleCheatDetected = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert CPU ticks to seconds.
        /// </summary>
        /// <param name="_Tick"></param>
        /// <returns></returns>
        private float TickToSec(long _Tick)
        {
            return Convert.ToSingle(_Tick) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// Synchronize calculated with the unity time.
        /// </summary>
        private void SynchronizationWithUnityTime()
        {
            this.time = Time.time;
            this.unscaledTime = Time.unscaledTime;
            this.deltaTime = Time.deltaTime;
            this.unscaledDeltaTime = Time.unscaledDeltaTime;
            this.realtimeSinceStartup = Time.realtimeSinceStartup;
            this.timeSinceLevelLoad = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// Internal check what kind of speedings happened!
        /// </summary>
        private void InternOnSpeedHackDetected()
        {
            if (this.MeanDeltaTime() <= 0.001f)
            {
                this.recognizedSpeedType = ESpeedingType.Stopped;
            }
            if (this.MeanDeltaTime() < this.MeanUtcTime())
            {
                this.recognizedSpeedType = ESpeedingType.SlowedDown;
            }
            if (this.MeanDeltaTime() > this.MeanUtcTime())
            {
                this.recognizedSpeedType = ESpeedingType.SpeedUp;
            }
        }

        /// <summary>
        /// A cheat got more tiems than the false positives detected!
        /// So notify delegates.
        /// </summary>
        protected override void OnCheatDetected()
        {
            base.OnCheatDetected();

            // Find type of speed hack / cheat.
            this.InternOnSpeedHackDetected();

            // Notify events.
            if (OnSpeedHackDetected != null)
            {
                OnSpeedHackDetected(this.recognizedSpeedType);
            }
        }

        /// <summary>
        /// On a new level got loaded, reset the time since level loaded time.
        /// </summary>
        /// <param name="_Scene"></param>
        /// <param name="_Mode"></param>
        protected override void OnLevelFinishedLoading(Scene _Scene, LoadSceneMode _Mode)
        {
            base.OnLevelFinishedLoading(_Scene, _Mode);

            // Reset time.
            this.timeSinceLevelLoad = 0.0f;
        }

        /// <summary>
        /// On application looses or gets focus, reset timer.
        /// </summary>
        /// <param name="_Focus"></param>
        private void OnApplicationFocus(bool _Focus)
        {
            // Reset Utc Time
            long var_UtcTimeNow = DateTime.UtcNow.Ticks;
            this.previousUtcTime = var_UtcTimeNow;
        }

        /// <summary>
        ///On application paused or unpaused, reset timer.
        /// </summary>
        /// <param name="_Pause"></param>
        private void OnApplicationPause(bool _Pause)
        {
            // Reset Utc Time
            long var_UtcTimeNow = DateTime.UtcNow.Ticks;
            this.previousUtcTime = var_UtcTimeNow;
        }

        /// <summary>
        /// Clear on destroy.
        /// </summary>
        private void OnDestroy()
        {
            OnSpeedHackDetected = null;
        }
    }
}