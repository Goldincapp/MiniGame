using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.Genuine
{
    public class CustomEventOnGenuineCheckFailedDetected : MonoBehaviour
    {
        // Some text element to display found cheating.
        public UnityEngine.UI.Text cheatFound;

        // Attach to the GenuineCheckDetector OnGenuineCheckFailedDetected event.
        private void Start()
        {
            OPS.AntiCheat.Detector.GenuineCheckDetector.OnGenuineCheckFailedDetected += Custom_OnGenuineCheckFailedDetected;
        }

        // Your custom event, what to do, if a cheat got detected.
        private void Custom_OnGenuineCheckFailedDetected()
        {
            this.cheatFound.text = "CHEAT DETECTED!";
            this.cheatFound.color = Color.red;
        }
    }
}