using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// OPS - AntiCheat
using OPS.AntiCheat.Detector;
using OPS.AntiCheat.Speed;

namespace OPS.AntiCheat.Demo.Speed
{
    public class CustomEventOnTimeCheatDetected : MonoBehaviour
    {
        // Some text element to display found cheating.
        public UnityEngine.UI.Text cheatFound;

        // Attach to the SpeedHackDetector OnSpeedHackDetected event.
        private void Start()
        {
            OPS.AntiCheat.Detector.SpeedHackDetector.OnSpeedHackDetected += Custom_OnSpeedHackDetected;
        }

        // Your custom event, what to do, if a cheat got detected.
        private void Custom_OnSpeedHackDetected(OPS.AntiCheat.Speed.ESpeedingType _SpeedingType)
        {
            switch (_SpeedingType)
            {
                case OPS.AntiCheat.Speed.ESpeedingType.Stopped:
                    {
                        this.cheatFound.text = "Speed Hack Detected! Cheater stopped the Game!";
                        break;
                    }
                case OPS.AntiCheat.Speed.ESpeedingType.SlowedDown:
                    {
                        this.cheatFound.text = "Speed Hack Detected! Cheater slowed down the Game!";
                        break;
                    }
                case OPS.AntiCheat.Speed.ESpeedingType.SpeedUp:
                    {
                        this.cheatFound.text = "Speed Hack Detected! Cheater speeded up the Game!";
                        break;
                    }
            }
            this.cheatFound.color = Color.red;
        }
    }
}