/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PTF.Reports
{
    public class Tag   
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class TagRepository
    {
        private readonly List<Tag> m_tagList = new List<Tag>();

        public bool IsEmpty
        {
            get { return m_tagList.Count == 0; }
        }

        public void SetTag(INamedObject obj)
        {
            lock (((ICollection)m_tagList).SyncRoot)
            {
                m_tagList.Add(new Tag { ID = obj.GetID(), Name = obj.GetName(), Count = 1 });
            }
        }

        public void SetTags(IQueryable<INamedObject> query)
        {
            lock (((ICollection)m_tagList).SyncRoot)
            {
                m_tagList.Clear();

                foreach (var obj in query)
                    m_tagList.Add(new Tag { ID = obj.GetID(), Name = obj.GetName(), Count = 1 });
            }
        }

        public IEnumerable<Tag> GetTags(string queryString, int count, Comparison<Tag> sortComp = null)
        {
            lock (((ICollection)m_tagList).SyncRoot)
            {
                var q = (from t in m_tagList
                         where t.Name.ToUpper().Contains(queryString.ToUpper())
                         select t);

                if (sortComp == null)
                    return q;

                var list = new List<Tag>(q);
                list.Sort(sortComp);
                return list;
            }
        }

        public void Clear()
        {
            lock (((ICollection)m_tagList).SyncRoot)
            {
                m_tagList.Clear();
            }
        }
    }
}