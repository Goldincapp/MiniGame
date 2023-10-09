using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity
using UnityEngine;
using UnityEngine.Internal;

// OPS - AntiCheat
using OPS.AntiCheat.Field;

namespace OPS.AntiCheat.Prefs
{
    /// <summary>
    /// Protected version of the unity PlayerPrefs. Contains also additional save and load able types.
    /// </summary>
    public sealed class ProtectedPlayerPrefs
    {
        /// <summary>
        /// Returns true if key exists in the preferences.
        /// </summary>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static bool HasKey(String _Key)
        {
            return PlayerPrefs.HasKey(_Key + "_Protected");
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetInt(string _Key, int _Value)
        {
            //Init Protected with value _Value
            ProtectedInt32 var_Protected = new ProtectedInt32(_Value);
            //Set intern value as value for _Key
            PlayerPrefs.SetInt(_Key + "_Protected", var_Protected.SecuredValue);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", var_Protected.RandomSecret);

            // Auto save if activated.
            if(AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static int GetInt(string _Key, [DefaultValue("0")] int _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                //Create empty Protected
                ProtectedInt32 var_Protected = new ProtectedInt32();

                //Load intern Value
                int var_InternValue = PlayerPrefs.GetInt(_Key + "_Protected");
                var_Protected.SecuredValue = var_InternValue;

                //Load Key
                int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                var_Protected.RandomSecret = var_Key;

                return var_Protected.Value_WithoutCheck;
            }

            return PlayerPrefs.GetInt(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static int GetInt(string _Key)
        {
            return ProtectedPlayerPrefs.GetInt(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetFloat(string _Key, float _Value)
        {
            unchecked
            {
                //Init Protected with value _Value
                ProtectedFloat var_Protected = new ProtectedFloat(_Value);
                //Set intern value as value for _Key
                PlayerPrefs.SetInt(_Key + "_Protected", (int)var_Protected.SecuredValue.intValue);
                //Save under the _Key+_ProtectedHash, the secret.
                PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Protected.RandomSecret);
            }

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static float GetFloat(string _Key, [DefaultValue("0.0F")] float _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                unchecked
                {
                    //Create empty Protected
                    ProtectedFloat var_Protected = new ProtectedFloat();

                    //Load intern Value
                    int var_InternValue = PlayerPrefs.GetInt(_Key + "_Protected");

                    UIntFloat var_UIntFloat = new UIntFloat();
                    var_UIntFloat.floatValue = 0;
                    var_UIntFloat.intValue = 0;
                    var_UIntFloat.intValue = (uint)var_InternValue;

                    var_Protected.SecuredValue = var_UIntFloat;

                    //Load Key
                    int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                    var_Protected.RandomSecret = (uint)var_Key;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static float GetFloat(string _Key)
        {
            return ProtectedPlayerPrefs.GetFloat(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetString(string _Key, string _Value)
        {
            //Init Protected with value _Value
            ProtectedString var_Protected = new ProtectedString(_Value);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_Protected.SecuredValue);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", var_Protected.RandomSecret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static string GetString(string _Key, [DefaultValue("\"\"")] string _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                //Create empty Protected
                ProtectedString var_Protected = new ProtectedString();

                //Load intern Value
                string var_InternValue = PlayerPrefs.GetString(_Key + "_Protected");

                var_Protected.SecuredValue = var_InternValue;

                //Load Key
                int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                var_Protected.RandomSecret = var_Key;

                return var_Protected.Value_WithoutCheck;
            }

            return PlayerPrefs.GetString(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static string GetString(string _Key)
        {
            return ProtectedPlayerPrefs.GetString(_Key, "");
        }

        ////////////////////// CUSTOM ////////////////////////////

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetBool(string _Key, bool _Value)
        {
            //Init Protected with value _Value
            ProtectedBool var_Protected = new ProtectedBool(_Value);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_Protected.SecuredValue.ToString());
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", var_Protected.RandomSecret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static bool GetBool(string _Key, [DefaultValue("false")] bool _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                //Create empty Protected
                ProtectedBool var_Protected = new ProtectedBool();

                //Load intern Value
                string var_InterValueString = PlayerPrefs.GetString(_Key + "_Protected");
                var_Protected.SecuredValue = byte.Parse(var_InterValueString);

                //Load Key
                int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                var_Protected.RandomSecret = var_Key;

                return var_Protected.Value_WithoutCheck;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static bool GetBool(string _Key)
        {
            return ProtectedPlayerPrefs.GetBool(_Key, false);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector2(string _Key, Vector2 _Value)
        {
            //Init Protected with value _Value
            ProtectedVector2 var_Protected = new ProtectedVector2(_Value);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_Protected.SecuredValue.x + "|" + var_Protected.SecuredValue.y);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Protected.RandomSecret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector2 GetVector2(string _Key, Vector2 _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                //Create empty Protected
                ProtectedVector2 var_Protected = new ProtectedVector2();

                //Load intern Value
                string var_InternValue = PlayerPrefs.GetString(_Key + "_Protected");
                string[] var_InterValueSplit = var_InternValue.Split('|');

                var_Protected.SecuredValue = new Vector2(float.Parse(var_InterValueSplit[0]), float.Parse(var_InterValueSplit[1]));

                //Load Key
                int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                var_Protected.RandomSecret = (uint)var_Key;

                return var_Protected.Value_WithoutCheck;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector2 GetVector2(string _Key)
        {
            return ProtectedPlayerPrefs.GetVector2(_Key, Vector2.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector3(string _Key, Vector3 _Value)
        {
            //Init Protected with value _Value
            ProtectedVector3 var_Protected = new ProtectedVector3(_Value);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_Protected.SecuredValue.x + "|" + var_Protected.SecuredValue.y + "|" + var_Protected.SecuredValue.z);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Protected.RandomSecret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector3 GetVector3(string _Key, Vector3 _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                //Create empty Protected
                ProtectedVector3 var_Protected = new ProtectedVector3();

                //Load intern Value
                string var_InternValue = PlayerPrefs.GetString(_Key + "_Protected");
                string[] var_InterValueSplit = var_InternValue.Split('|');

                var_Protected.SecuredValue = new Vector3(float.Parse(var_InterValueSplit[0]), float.Parse(var_InterValueSplit[1]), float.Parse(var_InterValueSplit[2]));

                //Load Key
                int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                var_Protected.RandomSecret = (uint)var_Key;

                return var_Protected.Value_WithoutCheck;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector3 GetVector3(string _Key)
        {
            return ProtectedPlayerPrefs.GetVector3(_Key, Vector3.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector4(string _Key, Vector4 _Value)
        {
            //Init Protected with value _Value
            ProtectedVector4 var_Protected = new ProtectedVector4(_Value);
            //Set intern value as value for _Key
            PlayerPrefs.SetString(_Key + "_Protected", var_Protected.SecuredValue.x + "|" + var_Protected.SecuredValue.y + "|" + var_Protected.SecuredValue.z + "|" + var_Protected.SecuredValue.w);
            //Save under the _Key+_ProtectedHash, the secret.
            PlayerPrefs.SetInt(_Key + "_ProtectedHash", (int)var_Protected.RandomSecret);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector4 GetVector4(string _Key, Vector4 _DefaultValue)
        {
            if (PlayerPrefs.HasKey(_Key + "_ProtectedHash"))
            {
                //Create empty Protected
                ProtectedVector4 var_Protected = new ProtectedVector4();

                //Load intern Value
                string var_InternValue = PlayerPrefs.GetString(_Key + "_Protected");
                string[] var_InterValueSplit = var_InternValue.Split('|');

                var_Protected.SecuredValue = new Vector4(float.Parse(var_InterValueSplit[0]), float.Parse(var_InterValueSplit[1]), float.Parse(var_InterValueSplit[2]), float.Parse(var_InterValueSplit[3]));

                //Load Key
                int var_Key = PlayerPrefs.GetInt(_Key + "_ProtectedHash");
                var_Protected.RandomSecret = (uint)var_Key;

                return var_Protected.Value_WithoutCheck;
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector4 GetVector4(string _Key)
        {
            return ProtectedPlayerPrefs.GetVector4(_Key, Vector4.zero);
        }
        
        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetQuaternion(string _Key, Quaternion _Value)
        {
            Vector4 var_Vector = new Vector4(_Value.x, _Value.y, _Value.z, _Value.w);
            SetVector4(_Key, var_Vector);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Quaternion GetQuaternion(string _Key)
        {
            Vector4 var_Vector = ProtectedPlayerPrefs.GetVector4(_Key, Vector4.zero);
            return new Quaternion(var_Vector.x, var_Vector.y, var_Vector.z, var_Vector.w);
        }

        /// <summary>
        /// Activate or deactivate force autosaving of modified preferences to the disk.
        /// </summary>
        public static bool AutoSave = false;

        /// <summary>
        /// Writes all modified preferences to disk.
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Removes key and its corresponding value from the preferences.
        /// </summary>
        public static void DeleteKey(String _Key)
        {
            PlayerPrefs.DeleteKey(_Key + "_Protected");

            PlayerPrefs.DeleteKey(_Key + "_ProtectedHash");         
        }
    }
}
