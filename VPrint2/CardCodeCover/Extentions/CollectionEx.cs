/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Collections.Generic;
using System.Windows.Forms;

namespace CardCodeCover
{
    public static class CollectionEx
    {
        public static List<DataGridViewRow> ToList(this DataGridViewSelectedRowCollection coll)
        {
            var list = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in coll)
                list.Add(row);
            return list;
        }
    }
}
