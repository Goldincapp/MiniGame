using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OPS - AntiCheat
using OPS.AntiCheat;
using OPS.AntiCheat.Speed;

namespace OPS.AntiCheat.Demo.Speed
{
    public class ProtectedTimeDemo : MonoBehaviour
    {

        private void Start()
        {
            Debug.Log("------------------");
            Debug.Log("Protected Time Demo");
            Debug.Log("------------------");

            // Add the namespace OPS.AntiCheat.Speed
            // Replace UnityEngine.Time with the protected one.
            // Now access the protected deltaTime, time, timeScale, realtimeSinceStartup, ....

            Debug.Log(Time.deltaTime + " : " + ProtectedTime.deltaTime);
            Debug.Log(Time.unscaledDeltaTime + " : " + ProtectedTime.unscaledDeltaTime);
            Debug.Log(Time.time + " : " + ProtectedTime.time);
            Debug.Log(Time.unscaledTime + " : " + ProtectedTime.unscaledTime);
            Debug.Log(Time.timeSinceLevelLoad + " : " + ProtectedTime.timeSinceLevelLoad);
            Debug.Log(Time.realtimeSinceStartup + " : " + ProtectedTime.realtimeSinceStartup);
        }
    }
}