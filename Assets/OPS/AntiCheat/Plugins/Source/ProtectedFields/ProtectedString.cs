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
    /// Represents a protected string. In almost all cases you can just replace your default type with the protected one.
    /// The usage of the protected string has some overhead because of the complex encryption and encoding. Use only if necessary. 
    /// </summary>
    [Serializable]
    public struct ProtectedString : IProtected, System.IDisposable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Encrypts a _String with _Secret and encodes it to UTF8.
        /// </summary>
        /// <param name="_String">The string you want to protect.</param>
        /// <param name="_Secret">The secret key for protection.</param>
        /// <returns></returns>
        private static string EncryptToUTF8(String _String, int _Secret)
        {
            if(_String == null)
            {
                return null;
            }

            if(_String.Length == 0)
            {
                return "";
            }

            uint key1 = 0x45435345 + (uint)_Secret;
            uint key2 = 0x95656543;

            byte[] buff1 = System.Text.UTF8Encoding.UTF8.GetBytes(_String);
            byte[] buff = new byte[buff1.Length + 1];

            buff[0] = (byte)(ProtectedFieldsSettings.RandomProvider.RandomInt32(1, Int32.MaxValue) % 256); // (UnityEngine.Random.Range(0, 255) % 256);

            byte d = buff[0];// 0x13;

            for (int i = 1; i < buff.Length; i++)
            {
                buff[i] = buff1[i - 1];
                key1 = (key1 * 4343255 + d + 5235457) % 0xFFFFFFFE;
                key2 = (key2 * 5354354 + d + 22646641) % 0xFFFFFFFE;

                d = buff[i];

                buff[i] = (byte)((uint)buff[i] ^ key1);
                buff[i] = (byte)((byte)buff[i] + (byte)key2);
            }

            return System.Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Decrypts a protected _String with _Secret and encodes it to UTF8.
        /// </summary>
        /// <param name="_String">The string you want to unprotect.</param>
        /// <param name="_Secret">The secret key for protection.</param>
        /// <returns></returns>
        private static string DecryptFromUTF8(String _String, int _Secret)
        {
            if (_String == null)
            {
                return null;
            }

            if (_String.Length == 0)
            {
                return "";
            }

            uint key1 = 0x45435345 + (uint)_Secret;
            uint key2 = 0x95656543;

            byte[] buff1 = System.Convert.FromBase64String(_String);
            byte[] buff = new byte[buff1.Length - 1];

            byte d = buff1[0];// 0x13;
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = buff1[i + 1];
                key1 = (key1 * 4343255 + d + 5235457) % 0xFFFFFFFE;
                key2 = (key2 * 5354354 + d + 22646641) % 0xFFFFFFFE;

                buff[i] = (byte)((byte)buff[i] - (byte)key2);
                buff[i] = (byte)((uint)buff[i] ^ key1);
                d = buff[i];
            }

            return System.Text.UTF8Encoding.UTF8.GetString(buff, 0, buff.Length);
        }

        /// <summary>
        /// The encrypted true value.
        /// </summary>
        private string securedValue;

        /// <summary>
        /// Get and set the encrypted true value.
        /// MOSTLY YOU DO NOT WANT TO USE THIS. USE THE 'Value' PROPERTY!
        /// </summary>
        internal string SecuredValue
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
        private string fakeValue;

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
        /// Create a new protected string with _Value.
        /// </summary>
        /// <param name="_Value"></param>
        public ProtectedString(String _Value = null)
        {
            this.randomSecret = ProtectedFieldsSettings.RandomProvider.RandomInt32(1, +5432);
            this.securedValue = EncryptToUTF8(_Value, this.randomSecret);

            //
            this.fakeValue = _Value;
        }

        /// <summary>
        /// Set and access the real unencrypted field value.
        /// </summary>
        public string Value
        {
            get
            {
                try
                {
                    String var_RealValue = DecryptFromUTF8(securedValue, this.randomSecret);

                    // Check for cheating!
                    if (this.fakeValue != var_RealValue)
                    {
                        FieldCheatDetector.Singleton.PossibleCheatDetected = true;
                    }

                    return var_RealValue;
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                this.securedValue = EncryptToUTF8(value, this.randomSecret);

                //
                this.fakeValue = value;
            }
        }

        /// <summary>
        /// Return the value without any possible cheat checking. Is used for the player prefs.
        /// </summary>
        internal string Value_WithoutCheck
        {
            get
            {
                try
                {
                    String var_Value = DecryptFromUTF8(securedValue, this.randomSecret);

                    return var_Value;
                }
                catch
                {
                    return "";
                }
            }
        }

        public void Dispose()
        {
            this.randomSecret = 0;
            this.securedValue = null;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override int GetHashCode()
        {
            if(this.Value == null)
            {
                return 0;
            }

            return this.Value.GetHashCode();
        }

        #region Implicit operator

        public static implicit operator ProtectedString(String _Value)
        {
            return new ProtectedString(_Value);
        }

        public static implicit operator String(ProtectedString _Value)
        {
            return _Value.Value;
        }

        #endregion

        #region Calculation operator

        // Addition
        public static ProtectedString operator +(ProtectedString v1, ProtectedString v2)
        {
            return new ProtectedString(v1.Value + v2.Value);
        }

        #endregion

        #region Equality operator

        // Equality
        public static bool operator ==(ProtectedString v1, ProtectedString v2)
        {
            return v1.Value == v2.Value;
        }
        public static bool operator !=(ProtectedString v1, ProtectedString v2)
        {
            return v1.Value != v2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectedString)
            {
                return this.Value == ((ProtectedString)obj).Value;
            }

            if(this.Value == null && obj == null)
            {
                return true;
            }

            return this.Value.Equals(obj);
        }

        #endregion
    }
}