using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.Genuine
{
    public class UIActions : MonoBehaviour
    {
        public void CheckForGenuine()
        {
            OPS.AntiCheat.Detector.GenuineCheckDetector.Singleton.ManualGenuineCheck();
        }
    }
}