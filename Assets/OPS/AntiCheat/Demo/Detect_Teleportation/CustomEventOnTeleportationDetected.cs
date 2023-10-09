using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.Teleport
{
    public class CustomEventOnTeleportationDetected : MonoBehaviour
    {
        // Some text element to display found cheating.
        public UnityEngine.UI.Text cheatFound;

        // Attach to the TeleportationCheatDetector OnTeleportationCheatDetected event.
        private void Start()
        {
            OPS.AntiCheat.Detector.TeleportationCheatDetector.OnTeleportationCheatDetected += Custom_OnTeleportationCheatDetected;
        }

        // Your custom event, what to do, if a cheat got detected.
        private void Custom_OnTeleportationCheatDetected()
        {
            this.cheatFound.text = "CHEAT DETECTED!";
            this.cheatFound.color = Color.red;
        }
    }
}