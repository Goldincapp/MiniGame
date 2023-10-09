using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.Serialization;

// OPS - AntiCheat
using OPS.AntiCheat.Detector;

namespace OPS.AntiCheat.Field
{
    /// <summary>
    /// Represents a protected date time. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedDateTime : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private ProtectedInt64 securedInt64;

        /// <summary>
        /// A honeypot pretending to be the orignal value. If some user tried to change this value via a cheat / hack engine, you will get notified.
        /// The protected value will keep its true value.
        /// </summary>
        [SerializeField]
        private Int64 fakeValue;

        /// <summary>
        /// Unity serialization hook. So the right values will be serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = Value.Ticks;
        }

        /// <summary>
        /// Unity deserialization hook. So the right values will be deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = new DateTime(this.fakeValue);
        }

        /// <summary>
        /// Create a new protected DateTime with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedDateTime(DateTime _Value)
        {
            this.securedInt64 = new ProtectedInt64(_Value.Ticks);

            // Setup fake value.
            this.fakeValue = _Value.Ticks;
        }

        /// <summary>
        /// Set and access the true unencrypted field value.
        /// </summary>
        public DateTime Value
        {
            get
            {
                DateTime var_RealValue = new DateTime(this.securedInt64.Value);

                // Check for cheating!
                if (this.fakeValue != var_RealValue.Ticks)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return var_RealValue;
            }
            set
            {
                //Set protected Value.
                this.securedInt64.Value = value.Ticks;

                // Setup fake value.
                this.fakeValue = value.Ticks;
            }
        }

        public void Dispose()
        {
            
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #region Implicit operator

        public static implicit operator ProtectedDateTime(DateTime _Value)
        {
            return new ProtectedDateTime(_Value);
        }

        public static implicit operator DateTime(ProtectedDateTime _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedDateTime v1, ProtectedDateTime v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedDateTime v1, ProtectedDateTime v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedDateTime)
            {
                return this.Value == ((ProtectedDateTime)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}