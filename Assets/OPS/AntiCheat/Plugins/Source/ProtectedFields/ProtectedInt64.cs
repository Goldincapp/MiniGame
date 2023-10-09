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
    /// Represents a protected int64. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedInt64 : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private Int64 securedValue;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        private Int64 randomSecret;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        internal Int64 RandomSecret
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
        private Int64 fakeValue;

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
        /// Create a new protected Int64 with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedInt64(Int64 value = 0)
        {
            this.randomSecret = (Int64)ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);
            this.securedValue = value ^ randomSecret;

            //
            this.fakeValue = value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public Int64 Value
        {
            get
            {
                Int64 var_RealValue = (Int64)(this.securedValue ^ this.randomSecret);

                // Check for cheating!
                if (this.fakeValue != var_RealValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return var_RealValue;
            }
            set
            {
                this.securedValue = value ^ this.randomSecret;

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
            return (int)this.Value;
        }

        #region Implicit operator

        public static implicit operator ProtectedInt64(Int64 _Value)
        {
            return new ProtectedInt64(_Value);
        }

        public static implicit operator Int64(ProtectedInt64 _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Calculation operator

        // Addition
        public static ProtectedInt64 operator +(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return new ProtectedInt64(v1.Value + v2.Value);
        }

        // Subtraction
        public static ProtectedInt64 operator -(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return new ProtectedInt64(v1.Value - v2.Value);
        }

        // Multiplication
        public static ProtectedInt64 operator *(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return new ProtectedInt64(v1.Value * v2.Value);
        }

        // Division
        public static ProtectedInt64 operator /(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return new ProtectedInt64(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedInt64)
            {
                return this.securedValue == ((ProtectedInt64)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        // Compare
        public static bool operator <(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return v1.Value < v2.Value;
        }
        public static bool operator <=(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return v1.Value <= v2.Value;
        }
        public static bool operator >(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return v1.Value > v2.Value;
        }
        public static bool operator >=(ProtectedInt64 v1, ProtectedInt64 v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}