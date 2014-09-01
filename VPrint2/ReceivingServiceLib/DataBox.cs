/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using VPrinting;

namespace ReceivingServiceLib
{
    /// <summary>
    /// Sample usage
    /// </summary>
    /// <example>
    /// var box = new DataBox(Keys.id, Keys.v_number, Keys.sitecode, Keys.barcode, Keys.scandate, Keys.location, Keys.operator_id, Keys.session_Id);
    /// using (var r = comm.ExecuteReader(CommandBehavior.CloseConnection))
    /// while (r.Read())
    /// {
    ///     box.Add(Keys.id, r.Get<int>("id").Value);
    ///     box.Add(Keys.v_number, r.Get<int>("v_number").Value);
    ///     box.Add(Keys.sitecode, r.GetString("sitecode"));
    ///     box.Add(Keys.barcode, r.GetString("barcode"));
    ///     box.Add(Keys.scandate, r.Get<DateTime>("scandate").Value);
    ///     box.Add(Keys.location, r.GetString("location"));
    ///     box.Add(Keys.operator_id, r.Get<int>("operator_id").Value);
    ///     box.Add(Keys.session_Id, r.Get<Guid>("session_Id").Value);
    /// }
    /// </example>
    [Serializable]
    public class DataBox : IEnumerable<DataBox.BoxItem>
    {
        private bool m_NameValidation = false;
        private readonly Dictionary<string, BoxItem> m_Table = new Dictionary<string, BoxItem>();

        [Serializable]
        public class BoxItem
        {
            public string Name { get; set; }
            public object Value { get; set; }

            public BoxItem(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }

        public object this[string name]
        {
            get
            {
                if (!m_Table.ContainsKey(name))
                    throw new InvalidOperationException("Can't find " + name);

                return m_Table[name].Value;
            }
            set
            {
                AddInternal(new BoxItem(name, value));
            }
        }

        public DataBox()
        {
        }

        public DataBox(params string[] names)
        {
            m_NameValidation = true;

            foreach (string name in names)
                AddInternal(new BoxItem(name, null));
        }

        private void AddInternal(BoxItem value)
        {
            m_Table[value.Name] = value;
        }

        public void Add<T>(string name, T value)
        {
            if (m_NameValidation && !m_Table.ContainsKey(name))
                throw new InvalidOperationException("Cannot add name " + name);

            AddInternal(new BoxItem(name, value));
        }

        public void AddRange(IEnumerable<BoxItem> items)
        {
            foreach (BoxItem en in items)
                m_Table.Add(en.Name, en);
        }

        public T GetValue<T>(string name) where T : IConvertible
        {
            if (!m_Table.ContainsKey(name))
                throw new InvalidOperationException("Cannot find " + name);

            return m_Table[name].Value.Cast<T>();
        }

        public IEnumerator<DataBox.BoxItem> GetEnumerator()
        {
            foreach (string name in m_Table.Keys)
                yield return m_Table[name];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (string name in m_Table.Keys)
                yield return m_Table[name];
        }
    }
}
