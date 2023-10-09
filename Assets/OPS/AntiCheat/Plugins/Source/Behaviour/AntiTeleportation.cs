using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Unity
using UnityEngine;

//OPS
using OPS.AntiCheat.Detector;

namespace OPS.AntiCheat.Behaviour
{
    /// <summary>
    /// Attach to some moving object and protect it against teleportation hacks.
    /// </summary>
    public class AntiTeleportation : MonoBehaviour
    {
        /// <summary>
        /// Maximal meter per seconds allowed.
        /// </summary>
        [Tooltip("Set the maximal meter per second (m/s) for this gameobject.")]
        public float MaxSpeed;

        /// <summary>
        /// The position of this gameobject at the previous fixed update.
        /// </summary>
        private Vector3 lastPosition;

        /// <summary>
        /// Unity start hook. Set the first last position.
        /// </summary>
        private void Start()
        {
            this.lastPosition = this.transform.position;
        }

        /// <summary>
        /// Unity fixed update hook. 
        /// </summary>
        private void FixedUpdate()
        {
            Vector3 var_CurrentPosition = this.transform.position;

            // Maximal moved meters.
            float var_MaxMovement = this.MaxSpeed * Time.fixedDeltaTime;

            // Check if moved distance larger than allowed maximal movement.
            // Teleport to maximal moved position.
            if (Vector3.Distance(var_CurrentPosition, this.lastPosition) > var_MaxMovement)
            {
                Vector3 var_Direction = var_CurrentPosition - this.lastPosition;
                this.TeleportTo(this.lastPosition + var_Direction * var_MaxMovement);

                TeleportationCheatDetector.Singleton.PossibleCheatDetected = true;
            }
            else
            {
                this.lastPosition = var_CurrentPosition;

                TeleportationCheatDetector.Singleton.PossibleCheatDetected = false;
            }
        }

        /// <summary>
        /// If you want to process an allowed teleportation for a moving object.
        /// </summary>
        /// <param name="_Position"></param>
        public void TeleportTo(Vector3 _Position)
        {
            this.transform.position = _Position;

            this.lastPosition = this.transform.position;
        }

        /// <summary>
        /// If you want to process an allowed teleportation for a moving object.
        /// </summary>
        /// <param name="_Position"></param>
        /// <param name="_Rotation"></param>
        public void TeleportTo(Vector3 _Position, Quaternion _Rotation)
        {
            this.transform.position = _Position;
            this.transform.rotation = _Rotation;

            this.lastPosition = this.transform.position;
        }
    }
}