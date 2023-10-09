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
    /// Represents a protected float. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedFloat : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private UIntFloat securedValue;

        /// <summary>
        /// Get and set the encrypted true value.
        /// MOSTLY YOU DO NOT WANT TO USE THIS. USE THE 'Value' PROPERTY!
        /// </summary>
        internal UIntFloat SecuredValue
        {
            get
            {
                return this.securedValue;
            }

            set
            {
                this.securedValue = value;
            }
        }

        /// <summary>
        /// Used for calculation if the int / float values for the secured value.
        /// </summary>
        private UIntFloat manager;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        private UInt32 randomSecret;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        internal UInt32 RandomSecret
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
        private float fakeValue;

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
        /// Create a new protected float with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedFloat(float value = 0)
        {
            this.randomSecret = (UInt32)ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            this.securedValue.intValue = 0;
            this.securedValue.floatValue = value;
            this.securedValue.intValue = this.securedValue.intValue ^ this.randomSecret;

            this.manager.intValue = 0;
            this.manager.floatValue = 0;

            //
            this.fakeValue = value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public float Value
        {
            get
            {
                this.manager.intValue = this.securedValue.intValue ^ this.randomSecret;

                // Check for cheating!
                if (this.fakeValue != this.manager.floatValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return this.manager.floatValue;
            }
            set
            {
                this.manager.floatValue = value;
                this.manager.intValue = this.manager.intValue ^ this.randomSecret;
                this.securedValue.floatValue = this.manager.floatValue;

                //
                this.fakeValue = value;
            }
        }

        /// <summary>
        /// Return the value without any possible cheat checking. Is used for the player prefs.
        /// </summary>
        internal float Value_WithoutCheck
        {
            get
            {
                this.manager.intValue = this.securedValue.intValue ^ this.randomSecret;

                return this.manager.floatValue;
            }
        }

        public void Dispose()
        {
            this.securedValue.intValue = 0;
            this.manager.intValue = 0;
            this.randomSecret = 0;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.securedValue.floatValue.GetHashCode();
        }

        #region Implicit operator

        // core
        public static implicit operator ProtectedFloat(float _Value)
        {
            return new ProtectedFloat(_Value);
        }

        public static implicit operator float(ProtectedFloat _Value)
        {
            return _Value.Value;
        }

        // float - int16
        public static implicit operator ProtectedInt16(ProtectedFloat _Value)
        {
            return new ProtectedInt16((Int16)_Value.Value);
        }

        public static implicit operator ProtectedFloat(ProtectedInt16 _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        // float - int32
        public static implicit operator ProtectedInt32(ProtectedFloat _Value)
        {
            return new ProtectedInt32((Int32)_Value.Value);
        }

        public static implicit operator ProtectedFloat(ProtectedInt32 _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        // float - int64
        public static implicit operator ProtectedInt64(ProtectedFloat _Value)
        {
            return new ProtectedInt64((Int64)_Value.Value);
        }

        public static implicit operator ProtectedFloat(ProtectedInt64 _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        #endregion

        #region Calculation operator

        // Addition
        public static ProtectedFloat operator +(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value + v2.Value);
        }

        // Subtraction
        public static ProtectedFloat operator -(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value - v2.Value);
        }

        // Multiplication
        public static ProtectedFloat operator *(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value * v2.Value);
        }

        // Division
        public static ProtectedFloat operator /(ProtectedFloat v1, ProtectedFloat v2)
        {
            return new ProtectedFloat(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedFloat)
            {
                return this.Value == ((ProtectedFloat)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        // Compare
        public static bool operator <(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value < v2.Value;
        }
        public static bool operator <=(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value <= v2.Value;
        }
        public static bool operator >(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value > v2.Value;
        }
        public static bool operator >=(ProtectedFloat v1, ProtectedFloat v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}