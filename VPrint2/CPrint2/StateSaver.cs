/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace CPrint2
{
    public class StateSaver
    {
        public static event ThreadExceptionEventHandler Error;

        private static readonly StateSaver m_Instance = new StateSaver();

        public static StateSaver Default { get { return m_Instance; } }

        private volatile Hashtable m_Table = Hashtable.Synchronized(new Hashtable(StringComparer.CurrentCultureIgnoreCase));

        public string Path { get; set; }

        public void Load()
        {
            Debug.Assert(Path != null);
            try
            {
                var file = new FileInfo(Path);
                if (file.Exists)
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (var str = file.OpenRead())
                    {
                        m_Table.Clear();
                        var table = (Hashtable)formatter.Deserialize(str);
                        m_Table = Hashtable.Synchronized(table);
                    }
                }

                Initialize();
            }
            catch (Exception ex)
            {
                if(Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
        }

        public void Save()
        {
            Debug.Assert(Path != null);
            try
            {
                var file = new FileInfo(Path);
                file.DeleteSafe();

                BinaryFormatter formatter = new BinaryFormatter();
                using (var str = file.OpenWrite())
                    formatter.Serialize(str, m_Table);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
        }

        public T Get<T>(string key, T @default = default(T))
        {
            if (!m_Table.Contains(key))
                m_Table[key] = @default;

            return (T)m_Table[key];
        }

        public void Set(string key, object data)
        {
            m_Table[key] = data;
        }

        public void Get(Control cnt)
        {
            var txt = cnt as TextBoxBase;
            if (txt != null)
            {
                txt.Text = Convert.ToString(m_Table[cnt.Name]);
            }
            else
            {
                var cb = cnt as CheckBox;
                if (cb != null)
                {
                    cb.Checked = Convert.ToBoolean(m_Table[cnt.Name]);
                }
                else
                {
                    var rb = cnt as RadioButton;
                    if (rb != null)
                    {
                        if (Convert.ToBoolean(m_Table[cnt.Name]))
                            rb.PerformClick();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public void Set(Control cnt)
        {
            var txt = cnt as TextBoxBase;
            if (txt != null)
            {
                m_Table[cnt.Name] = txt.Text;
            }
            else
            {
                var cb = cnt as CheckBox;
                if (cb != null)
                {
                    m_Table[cnt.Name] = cb.Checked;
                }
                else
                {
                    var rb = cnt as RadioButton;
                    if (rb != null)
                    {
                        m_Table[cnt.Name] = rb.Checked;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        private void Initialize()
        {
        }
    }
}
