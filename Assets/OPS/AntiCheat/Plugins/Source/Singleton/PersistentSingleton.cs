using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity
using UnityEngine;

namespace OPS.AntiCheat
{
    /// <summary>
    /// A thread safe singleton active in the whole application.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PersistentSingleton<T> : MonoBehaviour 
        where T : MonoBehaviour
    {
        /// <summary>
        /// The singleton itself.
        /// </summary>
        private static T singleton;

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static object lockHandle = new object();

        /// <summary>
        /// Returns an active singleton of this instance or creates a new one.
        /// </summary>
        public static T Singleton
        {
            get
            {
                lock (lockHandle)
                {
                    // GameObject got destroyed so Singleton too!
                    if (singleton != null && singleton.gameObject == null)
                    {
                        singleton = null;
                    }

                    if (singleton == null)
                    {
                        singleton = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return singleton;
                        }

                        if (singleton == null)
                        {
                            Create<T>();
                        }
                    }

                    return singleton;
                }
            }
        }

        /// <summary>
        /// Returns if a singleton exists.
        /// </summary>
        /// <returns></returns>
        public static bool Exists
        {
            get
            {
                return singleton != null;
            }
        }

        /// <summary>
        /// On awake, check if there is already a singleton.
        /// If there is one and it is not this, delete the gameobject.
        /// </summary>
        protected virtual void Awake()
        {
            // If a singleton already exists and this is not the singleton, destroy it immediate. Else keep it.
            if (Exists)
            {
                if(this != singleton)
                {
                    DestroyImmediate(this.gameObject);
                }
            }
            else
            {
                singleton = this as T;
            }
        }

        /// <summary>
        /// Create a GameObject adding T1 and set Singleton to the value T1.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        private static void Create<T1>() where T1 : T
        {
            // Already exists, just return.
            if(Exists)
            {
                return;
            }

            // Create gameobject.
            GameObject var_Singleton = new GameObject();

            // Add the singleton component to it.
            singleton = var_Singleton.AddComponent<T1>();
            var_Singleton.name = "(PersistentSingleton) " + typeof(T1).ToString();

            // Mark the gameobject as 'do not destroy'.
            DontDestroyOnLoad(var_Singleton);
        }
    }
}
