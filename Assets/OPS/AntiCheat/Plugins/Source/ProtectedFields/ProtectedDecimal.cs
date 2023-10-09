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
    /// Represents a protected decimal. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedDecimal : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private int[] securedValues;

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
            this.fakeValue = (double) this.Value;
        }

        /// <summary>
        /// Unity deserialization hook. So the right values will be deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this = (decimal) this.fakeValue;
        }

        /// <summary>
        /// Create a new protected decimal with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedDecimal(decimal _Value = 0)
        {
            this.securedValues = decimal.GetBits(_Value);

            // Setup fake value.
            this.fakeValue = (double) _Value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public decimal Value
        {
            get
            {
                try
                {
                    decimal var_RealValue = new decimal(this.securedValues);

                    // Check for cheating!
                    if (this.fakeValue != (double) var_RealValue)
                    {
                        FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                    }

                    return var_RealValue;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                this.securedValues = decimal.GetBits(value);

                // Setup fake value.
                this.fakeValue = (double) value;
            }
        }

        public void Dispose()
        {
            this.securedValues = null;
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

        // core
        public static implicit operator ProtectedDecimal(decimal _Value)
        {
            return new ProtectedDecimal(_Value);
        }

        public static implicit operator decimal(ProtectedDecimal _Value)
        {
            return _Value.Value;
        }

        // decimal - double
        public static implicit operator ProtectedDouble(ProtectedDecimal _Value)
        {
            return new ProtectedDouble((double)_Value.Value);
        }

        public static implicit operator ProtectedDecimal(ProtectedDouble _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        // decimal - int16
        public static implicit operator ProtectedInt16(ProtectedDecimal _Value)
        {
            return new ProtectedInt16((Int16)_Value.Value);
        }

        public static implicit operator ProtectedDecimal(ProtectedInt16 _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        // decimal - int32
        public static implicit operator ProtectedInt32(ProtectedDecimal _Value)
        {
            return new ProtectedInt32((Int32)_Value.Value);
        }

        public static implicit operator ProtectedDecimal(ProtectedInt32 _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        // decimal - int64
        public static implicit operator ProtectedInt64(ProtectedDecimal _Value)
        {
            return new ProtectedInt64((Int64)_Value.Value);
        }

        public static implicit operator ProtectedDecimal(ProtectedInt64 _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        // decimal - float
        public static implicit operator ProtectedFloat(ProtectedDecimal _Value)
        {
            return new ProtectedFloat((float)_Value.Value);
        }

        public static implicit operator ProtectedDecimal(ProtectedFloat _Value)
        {
            return new ProtectedDecimal((decimal)_Value.Value);
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedDecimal)
            {
                return this.Value == ((ProtectedDecimal)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        // Compare
        public static bool operator <(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value < v2.Value;
        }
        public static bool operator <=(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value <= v2.Value;
        }
        public static bool operator >(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value > v2.Value;
        }
        public static bool operator >=(ProtectedDecimal v1, ProtectedDecimal v2)
        {
            return v1.Value >= v2.Value;
        }

        #endregion
    }
}