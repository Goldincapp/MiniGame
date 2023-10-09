using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.Speed
{
    public class ProtectedRotateScript : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            this.transform.Rotate(Vector3.up * OPS.AntiCheat.Speed.ProtectedTime.deltaTime * 20f, Space.World);
        }
    }
}