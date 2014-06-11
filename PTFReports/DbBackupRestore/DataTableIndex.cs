/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Configuration;
using System.Collections.Generic;

namespace BackupRestore
{
    public sealed class DataTableIndex : ConfigurationSection
    {
        private const string TABLENAME = "tableName";
        private const string COLUMNLIST = "columnList";

        private static ConfigurationPropertyCollection _Properties;

        private static bool _ReadOnly;

        private static readonly ConfigurationProperty ms_TableName = new ConfigurationProperty(TABLENAME, typeof(string), "Users", 
            ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty ms_ColumnList = new ConfigurationProperty(COLUMNLIST, typeof(long), (long)1000, 
            ConfigurationPropertyOptions.None);

        public DataTableIndex()
        {
            // Property initialization
            _Properties = new ConfigurationPropertyCollection();
            _Properties.Add(ms_TableName);
            _Properties.Add(ms_ColumnList);
        }

        // This is a key customization. 
        // It returns the initialized property bag.
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _Properties;
            }
        }

        private new bool IsReadOnly
        {
            get
            {
                return _ReadOnly;
            }
        }

        // Use this to disable property setting.
        private void ThrowIfReadOnly(string propertyName)
        {
            if (IsReadOnly)
                throw new ConfigurationErrorsException("The property " + propertyName + " is read only.");
        }

        // Customizes the use of CustomSection
        // by setting _ReadOnly to false.
        // Remember you must use it along with ThrowIfReadOnly.
        protected override object GetRuntimeObject()
        {
            // To enable property setting just assign true to
            // the following flag.
            _ReadOnly = true;
            return base.GetRuntimeObject();
        }

        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)] 
        public string TableName
        {
            get
            {
                return (string)this[TABLENAME];
            }
            set
            {
                // With this you disable the setting.
                // Remember that the _ReadOnly flag must
                // be set to true in the GetRuntimeObject.
                ThrowIfReadOnly("TableName");
                this[TABLENAME] = value;
            }
        }

        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/'\"|\\", MinLength = 1, MaxLength = 60)]
        public IEnumerable<string> ColumnList
        {
            get
            {
                var strs = Convert.ToString(this[COLUMNLIST]).Split(';');
                foreach (string s in strs)
                    yield return s;
            }
            set
            {
                this[COLUMNLIST] = value;
            }
        }
    }
}
