using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace OPS.AntiCheat.Demo.Teleport
{
    public class UIActions : MonoBehaviour
    {
        /// <summary>
        /// The example GameObject to teleport.
        /// </summary>
        public GameObject toTeleportGameObject;

        /// <summary>
        /// The AntiTeleportation Behaviour at the to teleport GameObject. Used to apply an allowed jump/teleport.
        /// </summary>
        private AntiCheat.Behaviour.AntiTeleportation antiTeleportationBehaviour;

        /// <summary>
        /// Unity Start hook. Read the AntiTeleportation Behaviour.
        /// </summary>
        private void Start()
        {
            this.antiTeleportationBehaviour = this.toTeleportGameObject.GetComponent<AntiCheat.Behaviour.AntiTeleportation>();
        }

        /// <summary>
        /// Do an allowed teleportation.
        /// </summary>
        public void AllowedTeleport()
        {
            this.antiTeleportationBehaviour.TeleportTo(Vector3.left);
        }

        /// <summary>
        /// Do an not allowed teleportation which will be recognized as speeding or teleportation.
        /// </summary>
        public void CheatedTeleport()
        {
            this.toTeleportGameObject.transform.position = Vector3.right;
        }
    }
}