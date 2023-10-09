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
    /// Represents a protected quaternion. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedQuaternion : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value x.
        /// </summary>
        private UIntFloat securedValueX;

        /// <summary>
        /// The encrypted true value y.
        /// </summary>
        private UIntFloat securedValueY;

        /// <summary>
        /// The encrypted true value z.
        /// </summary>
        private UIntFloat securedValueZ;

        /// <summary>
        /// The encrypted true value w.
        /// </summary>
        private UIntFloat securedValueW;

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
        private Quaternion fakeValue;

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
        /// Create a new protected Quaternion with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedQuaternion(Quaternion _Value)
        {
            this.randomSecret = (UInt32)ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            this.securedValueX.intValue = 0;
            this.securedValueX.floatValue = _Value.x;
            this.securedValueX.intValue = this.securedValueX.intValue ^ this.randomSecret;

            this.securedValueY.intValue = 0;
            this.securedValueY.floatValue = _Value.y;
            this.securedValueY.intValue = this.securedValueY.intValue ^ this.randomSecret;

            this.securedValueZ.intValue = 0;
            this.securedValueZ.floatValue = _Value.z;
            this.securedValueZ.intValue = this.securedValueZ.intValue ^ this.randomSecret;

            this.securedValueW.intValue = 0;
            this.securedValueW.floatValue = _Value.w;
            this.securedValueW.intValue = this.securedValueW.intValue ^ this.randomSecret;

            this.manager.intValue = 0;
            this.manager.floatValue = 0;

            this.fakeValue = _Value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public Quaternion Value
        {
            get
            {
                Quaternion var_RealValue = new Quaternion();

                this.manager.intValue = this.securedValueX.intValue ^ this.randomSecret;
                var_RealValue.x = this.manager.floatValue;

                this.manager.intValue = this.securedValueY.intValue ^ this.randomSecret;
                var_RealValue.y = this.manager.floatValue;

                this.manager.intValue = this.securedValueZ.intValue ^ this.randomSecret;
                var_RealValue.z = this.manager.floatValue;

                this.manager.intValue = this.securedValueW.intValue ^ this.randomSecret;
                var_RealValue.w = this.manager.floatValue;

                // Check for cheating!
                if (this.fakeValue != var_RealValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return var_RealValue;
            }
            set
            {
                this.manager.floatValue = value.x;
                this.manager.intValue = this.manager.intValue ^ this.randomSecret;
                this.securedValueX.floatValue = this.manager.floatValue;

                this.manager.floatValue = value.y;
                this.manager.intValue = this.manager.intValue ^ this.randomSecret;
                this.securedValueY.floatValue = this.manager.floatValue;

                this.manager.floatValue = value.z;
                this.manager.intValue = this.manager.intValue ^ this.randomSecret;
                this.securedValueZ.floatValue = this.manager.floatValue;

                this.manager.floatValue = value.w;
                this.manager.intValue = this.manager.intValue ^ this.randomSecret;
                this.securedValueW.floatValue = this.manager.floatValue;
            }
        }

        /// <summary>
        /// Return the value without any possible cheat checking. Is used for the player prefs.
        /// </summary>
        internal Quaternion Value_WithoutCheck
        {
            get
            {
                Quaternion var_RealValue = new Quaternion();

                this.manager.intValue = this.securedValueX.intValue ^ this.randomSecret;
                var_RealValue.x = this.manager.floatValue;

                this.manager.intValue = this.securedValueY.intValue ^ this.randomSecret;
                var_RealValue.y = this.manager.floatValue;

                this.manager.intValue = this.securedValueZ.intValue ^ this.randomSecret;
                var_RealValue.z = this.manager.floatValue;

                this.manager.intValue = this.securedValueW.intValue ^ this.randomSecret;
                var_RealValue.w = this.manager.floatValue;

                return var_RealValue;
            }
        }

        public void Dispose()
        {
            this.securedValueX.intValue = 0;
            this.securedValueY.intValue = 0;
            this.securedValueZ.intValue = 0;
            this.manager.intValue = 0;
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

        public static implicit operator ProtectedQuaternion(Quaternion _Value)
        {
            return new ProtectedQuaternion(_Value);
        }

        public static implicit operator Quaternion(ProtectedQuaternion _Value)
        {
            return _Value.Value;
        }

        public static implicit operator ProtectedQuaternion(ProtectedVector4 _Value)
        {
            return new ProtectedQuaternion(new Quaternion(_Value.Value.x, _Value.Value.y, _Value.Value.z, _Value.Value.w));
        }

        public static implicit operator ProtectedVector4(ProtectedQuaternion _Value)
        {
            return new ProtectedVector4(new Vector4(_Value.Value.x, _Value.Value.y, _Value.Value.z, _Value.Value.w));
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedQuaternion v1, ProtectedQuaternion v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedQuaternion v1, ProtectedQuaternion v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedQuaternion)
            {
                return this.Value == ((ProtectedQuaternion)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}