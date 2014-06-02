/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

//#define SAVE_SETTINGS_TO_FILE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using PremierTaxFree.PTFLib.Net;

namespace PremierTaxFree.PTFLib
{
    public partial class SettingsTable
    {
        public event ThreadExceptionEventHandler Error;

        /// <summary>
        /// All data table
        /// </summary>
        public Hashtable DataTable { get; set; }
        private string m_activeSettingsName;

        /// <summary>
        /// Active settings table
        /// </summary>
        public Hashtable ActiveTable { get; private set; }

        private static readonly SettingsTable ms_Settings = new SettingsTable();

        /// <summary>
        /// Singlenton instance property
        /// </summary>
        public static SettingsTable Default
        {
            get { return ms_Settings; }
        }

        /// <summary>
        /// Get object by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        /// <summary>
        /// Set object by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static T Get<T>(string name, T @default)
        {
            var ht = (SettingsKeys.Contains(name)) ? Default.ActiveTable : Default.DataTable;
            if (!ht.Contains(name))
                ht[name] = @default;
            return (T)ht[name];
        }

        /// <summary>
        /// Sets value to settings table
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void Set(string name, object value)
        {
            //  Structures and IClonable classes can only be added and automatically cloned to the settings table
            //  Any other types added to settings table will not be multiplied during new setting creation
            var ht = (SettingsKeys.Contains(name)) ? Default.ActiveTable : Default.DataTable;
            ht[name] = value;
        }

        /// <summary>
        /// Creates new settings.
        /// Values are extracted (copied) from current settings values.
        /// All structures are copied. All objects are cloned. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subkeys"></param>
        public void CopyTable(string name, params string[] subkeys)
        {
            var table = new Hashtable();

            foreach (var key in subkeys)
            {
                if (ActiveTable[key] != null)
                {
                    //Copy objects and structures
                    //All objects should be ICloneable or 
                    //else won't be copied
                    object value = (ActiveTable[key] is ICloneable) ? ((ICloneable)ActiveTable[key]).Clone() : (object)ActiveTable[key];
                    table[key] = value;
                }
            }

            DataTable[name] = table;
        }

        /// <summary>
        /// Switches settings
        /// </summary>
        /// <param name="name"></param>
        public void LoadTable(string name)
        {
            Debug.Assert(name != null);

            if (string.CompareOrdinal(name, m_activeSettingsName) != 0)
            {
                m_activeSettingsName = name;

                if (!DataTable.ContainsKey(name))
                    DataTable[name] = new Hashtable();

                ActiveTable = Hashtable.Synchronized((Hashtable)DataTable[name]);
            }
        }

        public List<string> GetSettingNames()
        {
            List<string> list = new List<string>();
            lock (DataTable.SyncRoot)
            {
                foreach (DictionaryEntry item in DataTable)
                    if (item.Value is Hashtable)
                        list.Add(item.Key.Cast<string>());
            }
            return list;
        }

        /// <summary>
        /// Read settings from file
        /// </summary>
        public void Read()
        {
#if SAVE_SETTINGS_TO_FILE
            try
            {

                var fullFileName = GetFileName();

                if (!File.Exists(fullFileName))
                {
                    DataTable = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
                }
                else
                {
                    using (FileStream fs = new FileStream(fullFileName, FileMode.Open))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        byte[] buffer2 = buffer.Unprotect();
                        using (MemoryStream memory = new MemoryStream(buffer2))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            DataTable = Hashtable.Synchronized((Hashtable)formatter.Deserialize(memory));
                        }
                    }
                }

                LoadTable("default");
                
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
#else
            //Error prove
            DataTable = Hashtable.Synchronized(
                    DBConfigValue.ReadSf<Hashtable>(Strings.VScan_SettingsTable, new Hashtable(StringComparer.InvariantCultureIgnoreCase)));

            LoadTable("default");
#endif
        }

        /// <summary>
        /// Save settings to file
        /// </summary>
        public void Save()
        {
            try
            {
#if SAVE_SETTINGS_TO_FILE
                var fullFileName = GetFileName();

                using (MemoryStream memory = new MemoryStream())
                using (FileStream file = new FileStream(fullFileName, FileMode.OpenOrCreate))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memory, DataTable);
                    byte[] buffer = memory.ToArray().Protect();
                    file.Write(buffer, 0, buffer.Length);
                }
#else
                DBConfigValue.Save(Strings.VScan_SettingsTable, DataTable);
#endif
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
        }

        /// <summary>
        /// Generate file name
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            var asm = Assembly.GetEntryAssembly();
            if (asm != null)
                return Path.ChangeExtension(asm.Location, "dat");

            return "C:\\settings.dat";
        }

        /// <summary>
        /// All keys defined here are settings specific
        /// </summary>
        public static string[] SettingsKeys
        {
            get
            {
                return new string[]
                {
                    Strings.VScan_AutoInsertDataAfterScan,
                    Strings.VScan_AutoReadBarcodeAfterScan,
                    Strings.VScan_DefaultBackColor,
                    Strings.VScan_DefaultCountryCode,
                    Strings.VScan_DefaultFontFamily,
                    Strings.VScan_DefaultFontSize,
                    Strings.VScan_DefaultForeColor,
                    Strings.VScan_DefaultLineSize,
                    Strings.VScan_DistanceFromBarcodeBottomLeftToHiddenArea,
                    Strings.VScan_HiddenAreaSize,
                    Strings.VScan_ImageBorderColor,
                    Strings.VScan_ImageBorderColorDistance,
                    Strings.VScan_ImPrinterTemplate,
                    Strings.VScan_MaximumOpenedScanForms,
                    Strings.VScan_PrintAreaLocation,
                    Strings.VScan_SleepBeforeCleanTime,
                    Strings.VScan_TWAINUseDefaultScanner,
                    Strings.VScan_TWAINUseDefaultScannerSettings,
                    Strings.VScan_UseImPrinter,
                    Strings.VScan_VoucherLayout,
                    Strings.VScan_HiddenAreaDrawingCfg,
                };
            }
        }
    }
}