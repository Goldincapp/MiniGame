using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.ComputerTime
{
    public class CustomEventOnComputerTimeHackDetected : MonoBehaviour
    {
        // Some text element to display found cheating.
        public UnityEngine.UI.Text cheatFound;

        // Some text element to display system utc time.
        public UnityEngine.UI.Text systemTimeText;

        // Some text element to display compre utc time.
        public UnityEngine.UI.Text compareTimeText;

        // Attach to the ComputerTimeHackDetector OnComputerTimeHackDetected event.
        private void Start()
        {
            OPS.AntiCheat.Detector.ComputerTimeHackDetector.OnComputerTimeHackDetected += Custom_OnComputerTimeHackDetected;
        }

        // Display the system time and compare time at the gui.
        private void Update()
        {
            systemTimeText.text = "Local system UTC time: " + DateTime.UtcNow.ToLongTimeString();
            compareTimeText.text = "AntiCheat compare UTC time: " + OPS.AntiCheat.Detector.ComputerTimeHackDetector.Singleton.CompareDateTime.ToLongTimeString();
        }

        // Your custom event, what to do, if a cheat got detected.
        private void Custom_OnComputerTimeHackDetected()
        {
            this.cheatFound.text = "CHEAT DETECTED!";
            this.cheatFound.color = Color.red;
        }
    }
}