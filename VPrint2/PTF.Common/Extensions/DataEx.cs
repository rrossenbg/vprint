/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime;

namespace VPrinting
{
    public static class DataEx
    {
        [TargetedPatchingOptOut("na")]
        public static string ToXml(this DataTable table, string tableName = "Default")
        {
            if (table.TableName.IsNullOrWhiteSpace())
                table.TableName = tableName;

            StringWriter writer = new StringWriter();
            table.WriteXml(writer, XmlWriteMode.WriteSchema, true);
            string dataTableXml = writer.ToString();
            return dataTableXml;
        }

        [TargetedPatchingOptOut("na")]
        public static DataTable ToDataTable(this string xml)
        {
            DataTable table = new DataTable();
            StringReader reader = new StringReader(xml);
            table.ReadXml(reader);
            return table;
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<Dictionary<string, object>> GetData(this DataTable table)
        {
            var columns = Enumerable.Cast<DataColumn>(table.Columns);

            return table.AsEnumerable().Select(r => columns.Select(c => new { Column = c.ColumnName, Value = r[c] })
                             .ToDictionary(i => i.Column, i => i.Value != DBNull.Value ? i.Value : null));
        }

        public static DataTable ToDataTable(this IEnumerable<Dictionary<string, object>> source)
        {
            DataTable table = new DataTable();

            var firstItem = source.FirstOrDefault();
            if (firstItem != null)
            {
                foreach (var key in firstItem)
                    table.Columns.Add(new DataColumn() { ColumnName = key.Key, DataType = key.Value.GetType() });

                foreach (var item in source)
                {
                    var row = table.NewRow();
                    foreach (var key in item)
                        row[key.Key] = key.Value;

                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public static DataTable ToDataTable2(this IEnumerable<Dictionary<string, object>> parents)
        {
            var table = new DataTable();

            foreach (var parent in parents)
            {
                var children = parent.Values
                                     .OfType<IEnumerable<IDictionary<string, object>>>()
                                     .ToArray();

                var length = children.Any() ? children.Length : 1;

                var parentEntries = parent.Where(x => x.Value is string)
                                          .Repeat(length)
                                          .ToLookup(x => x.Key, x => x.Value);

                var childEntries = children.SelectMany(x => x.First())
                                           .ToLookup(x => x.Key, x => x.Value);

                var allEntries = parentEntries.Concat(childEntries)
                                              .ToDictionary(x => x.Key, x => x.ToArray());

                var headers = Enumerable.Cast<DataColumn>(allEntries.Select(x => x.Key).Except(Enumerable.Cast<string>(table.Columns))).Select(x => x.ColumnName)
                                        .Select(x => new DataColumn(x))
                                        .ToArray();

                table.Columns.AddRange(headers);

                var addedRows = new int[length];
                for (int i = 0; i < length; i++)
                    addedRows[i] = table.Rows.IndexOf(table.Rows.Add());

                foreach (DataColumn col in table.Columns)
                {
                    object[] columnRows;
                    if (!allEntries.TryGetValue(col.ColumnName, out columnRows))
                        continue;

                    for (int i = 0; i < addedRows.Length; i++)
                        table.Rows[addedRows[i]][col] = columnRows[i];
                }
            }

            return table;
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int times)
        {
            source = source.ToArray();
            return Enumerable.Range(0, times).SelectMany(_ => source);
        }
    }
}
