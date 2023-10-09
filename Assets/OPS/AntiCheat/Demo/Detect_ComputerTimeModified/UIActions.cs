using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.ComputerTime
{
    public class UIActions : MonoBehaviour
    {
        public void UseLocalCompareTime()
        {
            OPS.AntiCheat.Detector.ComputerTimeHackDetector.Singleton.ForceInternetTime = false;
        }

        public void UseInternetCompareTime()
        {
            OPS.AntiCheat.Detector.ComputerTimeHackDetector.Singleton.ForceInternetTime = true;
        }
    }
}