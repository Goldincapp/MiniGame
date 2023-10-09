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
    /// Represents a protected unsigned int16. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedUInt16 : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private UInt16 securedValue;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        private UInt16 randomSecret;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        internal UInt16 RandomSecret
        {
            get
            {
                return randomSecret;
            }

            set
            {
                randomSecret = value;
            }
        }

        /// <summary>
        /// A honeypot pretending to be the orignal value. If some user tried to change this value via a cheat / hack engine, you will get notified.
        /// The protected value will keep its true value.
        /// </summary>
        [SerializeField]
        private UInt16 fakeValue;

        /// <summary>
        /// Unity serialization hook. So the right values will be serialized.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.fakeValue = Value;
        }

        /// <summary>
        /// Unity deserialization hook. So the right values will be deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = this.fakeValue;
        }

        /// <summary>
        /// Create a new protected UInt16 with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedUInt16(UInt16 value = 0)
        {
            this.randomSecret = (UInt16)ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int16.MaxValue);
            this.securedValue = (UInt16)(value ^ randomSecret);

            //
            this.fakeValue = value;
        }

        private ProtectedUInt16(Int32 value = 0)
            :this((UInt16)value)
        {
        }
        
        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public UInt16 Value
        {
            get
            {
                UInt16 var_RealValue = (UInt16)(this.securedValue ^ this.randomSecret);

                // Check for cheating!
                if (this.fakeValue != var_RealValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return var_RealValue;
            }
            set
            {
                this.securedValue = (UInt16)(value ^ this.randomSecret);

                //
                this.fakeValue = value;
            }
        }

        public void Dispose()
        {
            this.securedValue = 0;
            this.randomSecret = 0;
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

        public static implicit operator ProtectedUInt16(UInt16 _Value)
        {
            return new ProtectedUInt16(_Value);
        }

        public static implicit operator UInt16(ProtectedUInt16 _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Calculation operator

        // Addition
        public static ProtectedUInt16 operator +(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return new ProtectedUInt16(v1.Value + v2.Value);
        }

        // Subtraction
        public static ProtectedUInt16 operator -(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return new ProtectedUInt16(v1.Value - v2.Value);
        }

        // Multiplication
        public static ProtectedUInt16 operator *(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return new ProtectedUInt16(v1.Value * v2.Value);
        }

        // Division
        public static ProtectedUInt16 operator /(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return new ProtectedUInt16(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedUInt16)
            {
                return this.Value == ((ProtectedUInt16)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        // Compare
        public static bool operator <(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return v1.Value < v2.Value;
        }
        public static bool operator <=(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return v1.Value <= v2.Value;
        }
        public static bool operator >(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return v1.Value > v2.Value;
        }
        public static bool operator >=(ProtectedUInt16 v1, ProtectedUInt16 v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}