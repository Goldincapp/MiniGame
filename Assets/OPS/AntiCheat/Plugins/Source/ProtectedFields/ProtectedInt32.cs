using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// OPS - AntiCheat
using OPS.AntiCheat.Detector;

namespace OPS.AntiCheat.Field
{
    /// <summary>
    /// Represents a protected int32. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedInt32 : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private Int32 securedValue;

        /// <summary>
        /// Get and set the encrypted true value.
        /// MOSTLY YOU DO NOT WANT TO USE THIS. USE THE 'Value' PROPERTY!
        /// </summary>
        internal Int32 SecuredValue
        {
            get
            {
                return securedValue;
            }

            set
            {
                this.securedValue = value;
            }
        }

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        private Int32 randomSecret;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        internal Int32 RandomSecret
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
        private Int32 fakeValue;

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
        /// Create a new protected Int32 with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedInt32(Int32 value = 0)
        {
            this.randomSecret = ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);
            this.securedValue = value ^ randomSecret;

            //
            this.fakeValue = value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public Int32 Value
        {
            get
            {
                Int32 var_RealValue = (Int32)(this.securedValue ^ this.randomSecret);

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

        /// <summary>
        /// Return the value without any possible cheat checking. Is used for the player prefs.
        /// </summary>
        internal Int32 Value_WithoutCheck
        {
            get
            {
                Int32 var_Value = (Int32)(this.securedValue ^ this.randomSecret);

                return var_Value;
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
            return this.Value;
        }

        #region Implicit operator

        public static implicit operator ProtectedInt32(Int32 _Value)
        {
            return new ProtectedInt32(_Value);
        }

        public static implicit operator Int32(ProtectedInt32 _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Calculation operator

        // Addition
        public static ProtectedInt32 operator +(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return new ProtectedInt32(v1.Value + v2.Value);
        }

        // Subtraction
        public static ProtectedInt32 operator -(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return new ProtectedInt32(v1.Value - v2.Value);
        }

        // Multiplication
        public static ProtectedInt32 operator *(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return new ProtectedInt32(v1.Value * v2.Value);
        }

        // Division
        public static ProtectedInt32 operator /(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return new ProtectedInt32(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedInt32)
            {
                return this.Value == ((ProtectedInt32)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        // Compare
        public static bool operator <(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return v1.Value < v2.Value;
        }
        public static bool operator <=(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return v1.Value <= v2.Value;
        }
        public static bool operator >(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return v1.Value > v2.Value;
        }
        public static bool operator >=(ProtectedInt32 v1, ProtectedInt32 v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}