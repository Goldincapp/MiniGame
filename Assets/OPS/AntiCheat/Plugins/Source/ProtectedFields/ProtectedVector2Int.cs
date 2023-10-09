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
    /// Represents a protected vector2 int. In almost all cases you can just replace your default type with the protected one.
    /// </summary>
    [Serializable]
    public struct ProtectedVector2Int : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The encrypted true value x.
        /// </summary>
        private int securedValueX;

        /// <summary>
        /// The encrypted true value y.
        /// </summary>
        private int securedValueY;

        /// <summary>
        /// Get and set the encrypted true value.
        /// MOSTLY YOU DO NOT WANT TO USE THIS. USE THE 'Value' PROPERTY!
        /// </summary>
        internal Vector2Int SecuredValue
        {
            get
            {
                return new Vector2Int(this.securedValueX, this.securedValueY);
            }

            set
            {
                this.securedValueX = value.x;
                this.securedValueY = value.y;
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
        private Vector2Int fakeValue;

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
        /// Create a new protected Vector2Int with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedVector2Int(Vector2Int _Value)
        {
            this.randomSecret = ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue);

            this.securedValueX = _Value.x ^ this.randomSecret;

            this.securedValueY = _Value.y ^ this.randomSecret;

            this.fakeValue = _Value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public Vector2Int Value
        {
            get
            {
                Vector2Int var_RealValue = new Vector2Int();

                var_RealValue.x = this.securedValueX ^ this.randomSecret;

                var_RealValue.y = this.securedValueY ^ this.randomSecret;

                // Check for cheating!
                if (this.fakeValue != var_RealValue)
                {
                    FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                }

                return var_RealValue;
            }
            set
            {
                this.securedValueX = value.x ^ this.randomSecret;

                this.securedValueY = value.y ^ this.randomSecret;
            }
        }

        /// <summary>
        /// Return the value without any possible cheat checking. Is used for the player prefs.
        /// </summary>
        internal Vector2Int Value_WithoutCheck
        {
            get
            {
                Vector2Int var_RealValue = new Vector2Int();

                var_RealValue.x = this.securedValueX ^ this.randomSecret;

                var_RealValue.y = this.securedValueY ^ this.randomSecret;

                return var_RealValue;
            }
        }

        public void Dispose()
        {
            this.securedValueX = 0;
            this.securedValueY = 0;
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

        public static implicit operator ProtectedVector2Int(Vector2Int _Value)
        {
            return new ProtectedVector2Int(_Value);
        }

        public static implicit operator Vector2Int(ProtectedVector2Int _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedVector2Int v1, ProtectedVector2Int v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedVector2Int v1, ProtectedVector2Int v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedVector2Int)
            {
                return this.Value == ((ProtectedVector2Int)obj).Value;
            }
            return this.Value.Equals(obj);
        }

        #endregion
    }
}