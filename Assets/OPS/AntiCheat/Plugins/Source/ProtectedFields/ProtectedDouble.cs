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
    /// Represents a protected double. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedDouble : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private ULongDouble securedValue;

        /// <summary>
        /// Get and set the encrypted true value.
        /// MOSTLY YOU DO NOT WANT TO USE THIS. USE THE 'Value' PROPERTY!
        /// </summary>
        public double SecuredValue
        {
            get
            {
                return this.securedValue.doubleValue;
            }
            set
            {
                this.securedValue.doubleValue = value;
            }
        }

        /// <summary>
        /// Used for calculation if the long / double values for the secured value.
        /// </summary>
        private ULongDouble manager;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        private UInt64 randomSecret;

        /// <summary>
        /// A secret key the true value gets encypted with.
        /// </summary>
        internal UInt64 RandomSecret
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
        private double fakeValue;

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
        /// Create a new protected double with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedDouble(double value = 0)
        {
            this.randomSecret = (ulong)ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            //
            this.securedValue.longValue = 0;
            this.securedValue.doubleValue = value;
            this.securedValue.longValue = this.securedValue.longValue ^ this.randomSecret;

            //
            this.manager.longValue = 0;
            this.manager.doubleValue = 0;

            //
            this.fakeValue = value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public double Value
        {
            get
            {
                this.manager.longValue = this.securedValue.longValue ^ this.randomSecret;

                // Check for cheating!
                if (this.fakeValue != this.manager.doubleValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return this.manager.doubleValue;
            }
            set
            {
                this.manager.doubleValue = value;
                this.manager.longValue = this.manager.longValue ^ this.randomSecret;
                this.securedValue.doubleValue = this.manager.doubleValue;

                //
                this.fakeValue = value;
            }
        }

        public void Dispose()
        {
            this.securedValue.longValue = 0;
            this.manager.longValue = 0;
            this.randomSecret = 0;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.securedValue.doubleValue.GetHashCode();
        }

        #region Implicit operator

        // core
        public static implicit operator ProtectedDouble(double _Value)
        {
            return new ProtectedDouble(_Value);
        }

        public static implicit operator double(ProtectedDouble _Value)
        {
            return _Value.Value;
        }

        // double - int16
        public static implicit operator ProtectedInt16(ProtectedDouble _Value)
        {
            return new ProtectedInt16((Int16)_Value.Value);
        }

        public static implicit operator ProtectedDouble(ProtectedInt16 _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        // double - int32
        public static implicit operator ProtectedInt32(ProtectedDouble _Value)
        {
            return new ProtectedInt32((Int32)_Value.Value);
        }

        public static implicit operator ProtectedDouble(ProtectedInt32 _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        // double - int64
        public static implicit operator ProtectedInt64(ProtectedDouble _Value)
        {
            return new ProtectedInt64((Int64)_Value.Value);
        }

        public static implicit operator ProtectedDouble(ProtectedInt64 _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        // double - float
        public static implicit operator ProtectedFloat(ProtectedDouble _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        public static implicit operator ProtectedDouble(ProtectedFloat _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        #endregion

        #region Calculation operation

        // Addition
        public static ProtectedDouble operator +(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value + v2.Value);
        }

        // Subtraction
        public static ProtectedDouble operator -(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value - v2.Value);
        }

        // Multiplication
        public static ProtectedDouble operator *(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value * v2.Value);
        }

        // Division
        public static ProtectedDouble operator /(ProtectedDouble v1, ProtectedDouble v2)
        {
            return new ProtectedDouble(v1.Value / v2.Value);
        }

        #endregion

        #region Equality operation

        // Equality
        public static bool operator ==(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedDouble)
            {
                return this.Value == ((ProtectedDouble)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        // Compare
        public static bool operator <(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value < v2.Value;
        }
        public static bool operator <=(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value <= v2.Value;
        }
        public static bool operator >(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value > v2.Value;
        }
        public static bool operator >=(ProtectedDouble v1, ProtectedDouble v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}