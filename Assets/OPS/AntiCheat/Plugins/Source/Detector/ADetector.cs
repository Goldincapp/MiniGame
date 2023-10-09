using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OPS.AntiCheat.Detector
{
    /// <summary>
    /// A base detector class.
    /// </summary>
    public abstract class ADetector<T> : PersistentSingleton<T>
        where T : MonoBehaviour
    {
        /// <summary>
        /// Amount of False Positives until a cheat got really detected!
        /// </summary>
        public abstract int FalsePositiveAmount { get; }

        /// <summary>
        /// Currently possible cheat amounts detected.
        /// </summary>
        private int possibleCheatDetectedAmount;

        /// <summary>
        /// Amount of checks in a row, where no cheat got detected, to reset the possible cheat counter.
        /// </summary>
        public abstract int ResetAmount { get; }

        /// <summary>
        /// Currently following impossible cheat amounts detected.
        /// </summary>
        private int impossibleCheatDetectedAmount;

        /// <summary>
        /// True: A cheat / hack got detected!
        /// </summary>
        private bool cheatDetected;

        /// <summary>
        /// Getter: Returns true if a cheat got detected.
        /// </summary>
        public bool CheatDetected
        {
            get
            {
                return this.cheatDetected;
            }
        }

        /// <summary>
        /// Getter: Returns of the cheat detection is active.
        /// Setter: Assign if cheat detection is active or deactived.
        /// </summary>
        [Tooltip("Enable or disable the cheat / hack detection and notification. Default: Active / True")]
        public bool CheatDetectionActive = true;

        /// <summary>
        /// Getter: Returns true if a cheat got detected.
        /// Setter: Assign true if a possible cheat got detected, it will increase the cheat indicator. 
        /// Assign false if no possible cheat got detected. After a defined amount of following no detection count, 
        /// the possible cheat detected amount will be reset.
        /// </summary>
        public bool PossibleCheatDetected
        {
            get
            {
                // Check if detection got deactivated.
                if (!this.CheatDetectionActive)
                {
                    return false;
                }

                return this.cheatDetected;
            }
            set
            {
                // Check if detection got deactivated.
                if (!this.CheatDetectionActive)
                {
                    return;
                }

                if (value)
                {
                    // Increase possible cheat amount.
                    this.possibleCheatDetectedAmount += 1;

                    // Clear the reset timer.
                    this.impossibleCheatDetectedAmount = 0;

                    // If to many possible cheats detected, real cheat got detected.
                    if (this.possibleCheatDetectedAmount >= this.FalsePositiveAmount)
                    {
                        this.cheatDetected = true;

                        // Reset cheat detected amount!
                        this.possibleCheatDetectedAmount = 0;

                        //Notify cheat detected!
                        this.OnCheatDetected();
                    }
                }
                else
                {
                    // Increase impossible cheat amount.
                    this.impossibleCheatDetectedAmount += 1;

                    // If to many possible cheats detected, real cheat got detected.
                    if (this.impossibleCheatDetectedAmount >= this.ResetAmount)
                    {
                        // Reset no cheat detected amount!
                        this.impossibleCheatDetectedAmount = 0;

                        // Reset cheat detected amount!
                        this.possibleCheatDetectedAmount = 0;
                    }
                }
            }
        }

        /// <summary>
        /// When a cheat got more than the FalsePositiveAmount detected this method getting called.
        /// </summary>
        protected virtual void OnCheatDetected()
        {

        }

        /// <summary>
        /// Unity OnEnable hook.
        /// Attaches to the SceneManager.sceneLoaded hook.
        /// </summary>
        protected virtual void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        /// <summary>
        /// Unity OnDisable hook.
        /// Dettaches from the SceneManager.sceneLoaded hook.
        /// </summary>
        protected virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        /// <summary>
        /// Event when a new scene got loaded.
        /// </summary>
        /// <param name="_Scene"></param>
        /// <param name="_Mode"></param>
        protected virtual void OnLevelFinishedLoading(Scene _Scene, LoadSceneMode _Mode)
        {
        }
    }
}