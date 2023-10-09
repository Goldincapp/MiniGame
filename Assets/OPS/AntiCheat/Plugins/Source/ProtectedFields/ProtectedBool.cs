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
    /// Represents a protected boolean. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedBool : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private byte securedValue;

        /// <summary>
        /// Get and set the encrypted true value.
        /// MOSTLY YOU DO NOT WANT TO USE THIS. USE THE 'Value' PROPERTY!
        /// </summary>
        internal byte SecuredValue
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
        /// A secret key the true value getting encypted with.
        /// </summary>
        private Int32 randomSecret;

        /// <summary>
        /// A secret key the true value getting encypted with.
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
        private bool fakeValue;

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
        /// Create a new protected boolean with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedBool(bool _Value = false)
        {
            this.randomSecret = ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);
            byte var_BoolAsByte = (_Value == true) ? (byte)1 : (byte)0;
            this.securedValue = (byte)(var_BoolAsByte ^ randomSecret);

            //
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Set and access the true unencrypted field value.
        /// </summary>
        public bool Value
        {
            get
            {
                byte var_BoolAsByte = (byte)(this.securedValue ^ this.randomSecret);
                bool var_RealValue = (var_BoolAsByte == 1) ? true : false;

                // Check for cheating!
                if (this.fakeValue != var_RealValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return var_RealValue;
            }
            set
            {
                byte var_BoolAsByte = (value == true) ? (byte)1 : (byte)0;
                this.securedValue = (byte)(var_BoolAsByte ^ randomSecret);

                //
                this.fakeValue = value;
            }
        }

        /// <summary>
        /// Return the value without any possible cheat checking. Is used for the player prefs.
        /// </summary>
        internal bool Value_WithoutCheck
        {
            get
            {
                byte var_BoolAsByte = (byte)(this.securedValue ^ this.randomSecret);
                bool var_Value = (var_BoolAsByte == 1) ? true : false;

                return var_Value;
            }
        }

        public void Dispose()
        {
            this.securedValue = 0;
            this.randomSecret = 0;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        #region Implicit operator

        public static implicit operator ProtectedBool(bool _Value)
        {
            return new ProtectedBool(_Value);
        }

        public static implicit operator bool(ProtectedBool _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedBool v1, ProtectedBool v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedBool v1, ProtectedBool v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedBool)
            {
                return this.Value == ((ProtectedBool)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}