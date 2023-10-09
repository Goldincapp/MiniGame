using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity
using UnityEngine;

// OPS - AntiCheat
using OPS.AntiCheat.Detector;

namespace OPS.AntiCheat.Speed
{
    /// <summary>
    /// Use this ProtectedTime instead of the UnityEngine.Time.
    /// The ProtectedTime will return the values UnityEngine.Time returns, until a cheat / hack got detected.
    /// Then the time values will be calculated by the system ticks.
    /// </summary>
    public sealed class ProtectedTime
    {
        /// <summary>
        /// The time in seconds it took to complete the last frame (Read Only).
        /// </summary>
        public static float deltaTime
        {
            get
            {
                if (SpeedHackDetector.Singleton != null && SpeedHackDetector.Singleton.CheatDetected == true)
                {
                    return SpeedHackDetector.Singleton.deltaTime;
                }

                return Time.deltaTime;
            }
        }

        /// <summary>
        ///   <para>The timeScale-independent interval in seconds from the last frame to the current one (Read Only).</para>
        /// </summary>
        public static float unscaledDeltaTime
        {
            get
            {
                if (SpeedHackDetector.Singleton != null && SpeedHackDetector.Singleton.CheatDetected == true)
                {
                    return SpeedHackDetector.Singleton.unscaledDeltaTime;
                }

                return Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// The scale at which the time is passing. This can be used for slow motion effects.
        /// </summary>
        public static float timeScale
        {
            get
            {
                if (SpeedHackDetector.Singleton != null)
                {
                    return SpeedHackDetector.Singleton.timeScale;
                }

                return Time.timeScale;
            }
            set
            {
                Time.timeScale = value;
            }
        }

        /// <summary>
        /// The time at the beginning of this frame (Read Only). This is the time in seconds since the start of the game.
        /// </summary>
        public static float time
        {
            get
            {
                if (SpeedHackDetector.Singleton != null && SpeedHackDetector.Singleton.CheatDetected == true)
                {
                    return SpeedHackDetector.Singleton.time;
                }

                return Time.time;
            }
        }

        /// <summary>
        ///   <para>The timeScale-independent time for this frame (Read Only). This is the time in seconds since the start of the game.</para>
        /// </summary>
        public static float unscaledTime
        {
            get
            {
                if (SpeedHackDetector.Singleton != null && SpeedHackDetector.Singleton.CheatDetected == true)
                {
                    return SpeedHackDetector.Singleton.unscaledTime;
                }

                return Time.unscaledTime;
            }
        }

        /// <summary>
        ///   <para>The time this frame has started (Read Only). This is the time in seconds since the last level has been loaded.</para>
        /// </summary>
        public static float timeSinceLevelLoad
        {
            get
            {
                if (SpeedHackDetector.Singleton != null && SpeedHackDetector.Singleton.CheatDetected == true)
                {
                    return SpeedHackDetector.Singleton.timeSinceLevelLoad;
                }

                return Time.timeSinceLevelLoad;
            }
        }

        /// <summary>
        /// The real time in seconds since the game started (Read Only).
        /// </summary>
        public static float realtimeSinceStartup
        {
            get
            {
                if (SpeedHackDetector.Singleton != null && SpeedHackDetector.Singleton.CheatDetected == true)
                {
                    return SpeedHackDetector.Singleton.realtimeSinceStartup;
                }

                return Time.realtimeSinceStartup;
            }
        }
    }
}