using System;
using System.Collections.Generic;
using System.IO;
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
    /// Thread safe file based protected player prefs.
    /// </summary>
    public static class ProtectedFileBasedPlayerPrefs
    {
        /// <summary>
        /// Custom file path.
        /// Default is: Application.persistentDataPath + System.IO.Path.PathSeparator + "playerprefs.dat" 
        /// </summary>
        public static String FilePath { get; set; } = Application.persistentDataPath + System.IO.Path.PathSeparator + "playerprefs.dat";

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static object lockHandle = new object();

        /// <summary>
        /// Data structure of a single PlayerPrefs entry.
        /// </summary>
        private struct DataStruct
        {
            public EPlayerPrefsType PlayerPrefsType { get; }

            public String Key { get; }

            public String Value { get; }

            public String Hash { get; }

            public DataStruct(EPlayerPrefsType _PlayerPrefsType, String _Key, String _Value, String _Hash)
            {
                this.PlayerPrefsType = _PlayerPrefsType;
                this.Key = _Key;
                this.Value = _Value;
                this.Hash = _Hash;
            }
        }

        /// <summary>
        /// Key to DataStruct Dictionary.
        /// </summary>
        private static Dictionary<String, DataStruct> currentDataStructMapping;

        /// <summary>
        /// Returns true if key exists in the preferences.
        /// </summary>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static bool HasKey(String _Key)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                return currentDataStructMapping.ContainsKey(_Key);
            }
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetInt(string _Key, int _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedInt32 var_Protected = new ProtectedInt32(_Value);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.INT, _Key, var_Protected.SecuredValue.ToString(), var_Protected.RandomSecret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static int GetInt(string _Key, [DefaultValue("0")] int _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty Protected
                    ProtectedInt32 var_Protected = new ProtectedInt32();

                    // Load secured value
                    if (!int.TryParse(currentDataStructMapping[_Key].Value, out int var_Value))
                    {
                        return PlayerPrefs.GetInt(_Key, _DefaultValue);
                    }
                    var_Protected.SecuredValue = var_Value;

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                    {
                        return PlayerPrefs.GetInt(_Key, _DefaultValue);
                    }
                    var_Protected.RandomSecret = var_hash;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return PlayerPrefs.GetInt(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static int GetInt(string _Key)
        {
            return GetInt(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetFloat(string _Key, float _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                unchecked
                {
                    // Init Protected with value _Value
                    ProtectedFloat var_Protected = new ProtectedFloat(_Value);
                    // Create new DataStruct
                    DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.FLOAT, _Key, var_Protected.SecuredValue.intValue.ToString(), var_Protected.RandomSecret.ToString());
                    // Assign to mapping
                    currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
                }
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static float GetFloat(string _Key, [DefaultValue("0.0F")] float _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    unchecked
                    {
                        // Create empty Protected
                        ProtectedFloat var_Protected = new ProtectedFloat();

                        // Load secured value
                        if (!int.TryParse(currentDataStructMapping[_Key].Value, out int var_Value))
                        {
                            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
                        }

                        UIntFloat var_UIntFloat = new UIntFloat();
                        var_UIntFloat.floatValue = 0;
                        var_UIntFloat.intValue = 0;
                        var_UIntFloat.intValue = (uint)var_Value;

                        var_Protected.SecuredValue = var_UIntFloat;

                        // Load hash
                        if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                        {
                            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
                        }
                        var_Protected.RandomSecret = (uint)var_hash;

                        return var_Protected.Value_WithoutCheck;
                    }
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
            return GetFloat(_Key, 0);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetString(string _Key, string _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                //Init Protected with value _Value
                ProtectedString var_Protected = new ProtectedString(_Value);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.STRING, _Key, var_Protected.SecuredValue.ToString(), var_Protected.RandomSecret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static string GetString(string _Key, [DefaultValue("\"\"")] string _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty Protected
                    ProtectedString var_Protected = new ProtectedString();

                    // Load intern Value
                    string var_Value = currentDataStructMapping[_Key].Value;
                    var_Protected.SecuredValue = var_Value;

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                    {
                        return PlayerPrefs.GetString(_Key, _DefaultValue);
                    }
                    var_Protected.RandomSecret = var_hash;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return PlayerPrefs.GetString(_Key, _DefaultValue);
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static string GetString(string _Key)
        {
            return GetString(_Key, "");
        }

        ////////////////////// CUSTOM ////////////////////////////

        /// <summary>
        ///   <para>Sets the _Value of the preference identified by _Key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetBool(string _Key, bool _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedBool var_Protected = new ProtectedBool(_Value);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.BOOL, _Key, var_Protected.SecuredValue.ToString(), var_Protected.RandomSecret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static bool GetBool(string _Key, [DefaultValue("false")] bool _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty Protected
                    ProtectedBool var_Protected = new ProtectedBool();

                    // Load secured value
                    if (!byte.TryParse(currentDataStructMapping[_Key].Value, out byte var_Value))
                    {
                        return _DefaultValue;
                    }
                    var_Protected.SecuredValue = var_Value;

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                    {
                        return _DefaultValue;
                    }
                    var_Protected.RandomSecret = var_hash;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static bool GetBool(string _Key)
        {
            return GetBool(_Key, false);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector2(string _Key, Vector2 _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                //Init Protected with value _Value
                ProtectedVector2 var_Protected = new ProtectedVector2(_Value);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.VECTOR2, _Key, var_Protected.SecuredValue.x + "|" + var_Protected.SecuredValue.y, var_Protected.RandomSecret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector2 GetVector2(string _Key, Vector2 _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty Protected
                    ProtectedVector2 var_Protected = new ProtectedVector2();

                    // Load value
                    String var_Value = currentDataStructMapping[_Key].Value;
                    String[] var_ValueSplit = var_Value.Split('|');

                    var_Protected.SecuredValue = new Vector2(float.Parse(var_ValueSplit[0]), float.Parse(var_ValueSplit[1]));

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                    {
                        return _DefaultValue;
                    }
                    var_Protected.RandomSecret = (uint)var_hash;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector2 GetVector2(string _Key)
        {
            return GetVector2(_Key, Vector2.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector3(string _Key, Vector3 _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                // Init Protected with value _Value
                ProtectedVector3 var_Protected = new ProtectedVector3(_Value);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.VECTOR3, _Key, var_Protected.SecuredValue.x + "|" + var_Protected.SecuredValue.y + "|" + var_Protected.SecuredValue.z, var_Protected.RandomSecret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector3 GetVector3(string _Key, Vector3 _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty Protected
                    ProtectedVector3 var_Protected = new ProtectedVector3();

                    // Load value
                    String var_Value = currentDataStructMapping[_Key].Value;
                    String[] var_ValueSplit = var_Value.Split('|');

                    var_Protected.SecuredValue = new Vector3(float.Parse(var_ValueSplit[0]), float.Parse(var_ValueSplit[1]), float.Parse(var_ValueSplit[2]));

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                    {
                        return _DefaultValue;
                    }
                    var_Protected.RandomSecret = (uint)var_hash;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector3 GetVector3(string _Key)
        {
            return GetVector3(_Key, Vector3.zero);
        }

        /// <summary>
        ///   <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void SetVector4(string _Key, Vector4 _Value)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                //Init Protected with value _Value
                ProtectedVector4 var_Protected = new ProtectedVector4(_Value);
                // Create new DataStruct
                DataStruct var_DataStruct = new DataStruct(EPlayerPrefsType.VECTOR4, _Key, var_Protected.SecuredValue.x + "|" + var_Protected.SecuredValue.y + "|" + var_Protected.SecuredValue.z + "|" + var_Protected.SecuredValue.w, var_Protected.RandomSecret.ToString());
                // Assign to mapping
                currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        /// <param name="_DefaultValue"></param>
        public static Vector4 GetVector4(string _Key, Vector4 _DefaultValue)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                if (currentDataStructMapping.ContainsKey(_Key))
                {
                    // Create empty Protected
                    ProtectedVector4 var_Protected = new ProtectedVector4();

                    // Load value
                    String var_Value = currentDataStructMapping[_Key].Value;
                    String[] var_ValueSplit = var_Value.Split('|');

                    var_Protected.SecuredValue = new Vector4(float.Parse(var_ValueSplit[0]), float.Parse(var_ValueSplit[1]), float.Parse(var_ValueSplit[2]), float.Parse(var_ValueSplit[3]));

                    // Load hash
                    if (!int.TryParse(currentDataStructMapping[_Key].Hash, out int var_hash))
                    {
                        return _DefaultValue;
                    }
                    var_Protected.RandomSecret = (uint)var_hash;

                    return var_Protected.Value_WithoutCheck;
                }
            }

            return _DefaultValue;
        }

        /// <summary>
        ///   <para>Returns the value corresponding to _Key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="_Key"></param>
        public static Vector4 GetVector4(string _Key)
        {
            return GetVector4(_Key, Vector4.zero);
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
            Vector4 var_Vector = GetVector4(_Key, Vector4.zero);
            return new Quaternion(var_Vector.x, var_Vector.y, var_Vector.z, var_Vector.w);
        }

        /// <summary>
        /// Removes the PlayerPrefs at _Key.
        /// </summary>
        /// <param name="_Key"></param>
        public static void DeleteKey(String _Key)
        {
            // Load the mapping if not already loaded.
            Load();

            lock (lockHandle)
            {
                currentDataStructMapping.Remove(_Key);
            }

            // Save the mapping.
            Save();
        }

        /// <summary>
        /// Loads the data struct mapping, if not already loaded.
        /// </summary>
        private static void Load()
        {
            lock (lockHandle)
            {
                if (currentDataStructMapping == null)
                {
                    currentDataStructMapping = new Dictionary<string, DataStruct>();

                    if (System.IO.File.Exists(FilePath))
                    {
                        using (FileStream var_FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (BinaryReader var_Reader = new BinaryReader(var_FileStream))
                            {
                                // Read the count.
                                int var_Count = var_Reader.ReadInt32();

                                // Read each element.
                                for (int i = 0; i < var_Count; i++)
                                {
                                    DataStruct var_DataStruct = ReadDataStruct(var_Reader);

                                    currentDataStructMapping[var_DataStruct.Key] = var_DataStruct;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the data struct mapping if there is one.
        /// </summary>
        private static void Save()
        {
            lock (lockHandle)
            {
                if (currentDataStructMapping == null)
                {
                    return;
                }

                using (FileStream var_FileStream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (BinaryWriter var_Writer = new BinaryWriter(var_FileStream))
                    {
                        // Write the count.
                        var_Writer.Write(currentDataStructMapping.Count);

                        // Write each element.
                        foreach(var var_Pair in currentDataStructMapping)
                        {
                            WriteDataStruct(var_Writer, currentDataStructMapping[var_Pair.Key]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read a DataStruct from the _Reader.
        /// </summary>
        /// <param name="_Reader"></param>
        /// <returns></returns>
        private static DataStruct ReadDataStruct(BinaryReader _Reader)
        {
            EPlayerPrefsType var_PlayerPrefsType = (EPlayerPrefsType)_Reader.ReadByte();

            String var_Key = ReadString(_Reader);

            String var_Value = ReadString(_Reader);

            String var_Hash = ReadString(_Reader);

            return new DataStruct(var_PlayerPrefsType, var_Key, var_Value, var_Hash);
        }

        /// <summary>
        /// Writes a DataStruct to _Writer.
        /// </summary>
        /// <param name="_Writer"></param>
        /// <param name="_DataStruct"></param>
        private static void WriteDataStruct(BinaryWriter _Writer, DataStruct _DataStruct)
        {
            _Writer.Write((byte)_DataStruct.PlayerPrefsType);

            WriteString(_Writer, _DataStruct.Key);

            WriteString(_Writer, _DataStruct.Value);

            WriteString(_Writer, _DataStruct.Hash);
        }

        /// <summary>
        /// Read a string with length.
        /// </summary>
        /// <param name="_Reader"></param>
        /// <returns></returns>
        private static string ReadString(BinaryReader _Reader)
        {
            int var_Length = _Reader.ReadInt32();
            if (var_Length > 0 && var_Length <= _Reader.BaseStream.Length - _Reader.BaseStream.Position)
            {
                byte[] var_StringData = _Reader.ReadBytes(var_Length);
                String var_String = Encoding.UTF8.GetString(var_StringData);

                return var_String;
            }
            return "";
        }

        /// <summary>
        /// Write string and write the length.
        /// </summary>
        /// <param name="_Writer"></param>
        /// <param name="_String"></param>
        private static void WriteString(BinaryWriter _Writer, String _String)
        {
            byte[] var_Bytes = Encoding.UTF8.GetBytes(_String);
            _Writer.Write(var_Bytes.Length);
            _Writer.Write(var_Bytes);
        }
    }
}