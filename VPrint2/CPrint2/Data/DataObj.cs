/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace CPrint2.Data
{
    public class DataObj : IEquatable<DataObj>
    {
        public int Iso { get; set; }
        public int BrId { get; set; }
        public int VId { get; set; }
        public int PartN { get; set; }
        public bool Submit { get; set; }

        public bool IsValid
        {
            get
            {
                return (Iso > 0 && BrId > 0 && VId > 0 && PartN > 0);
            }
        }

        public ArrayList Files = ArrayList.Synchronized(new ArrayList());

        /// <summary>
        /// 826;123456;34567890;1;True
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4}", Iso, BrId, VId, PartN, Submit);
        }

        /// <summary>
        /// 826;123456;34567890;1
        /// </summary>
        /// <returns></returns>
        public static DataObj Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("value cannot be null or empty");

            string[] strs = value.Split(';');

            if (strs.Length != 5)
                throw new ArgumentException("strs.Length must be 5 (Iso, BrId, VId, PartN, Submit)");

            int iso = 0, brid = 0, vid = 0, partN = 0;
            bool submit = false;

            if (!int.TryParse(strs[0], out iso))
                return null;

            if (!int.TryParse(strs[1], out brid))
                return null;

            if (!int.TryParse(strs[2], out vid))
                return null;

            if (!int.TryParse(strs[3], out partN))
                return null;

            if (!bool.TryParse(strs[4], out submit))
                return null;

            var obj = new DataObj();
            obj.Iso = iso;
            obj.BrId = brid;
            obj.VId = vid;
            obj.PartN = partN;
            obj.Submit = submit;
            return obj;
        }

        public bool Equals(DataObj other)
        {
            if (other == null)
                return false;
            return (this.Iso == other.Iso && this.BrId == other.BrId && this.VId == other.VId);
        }

        public void DeleteFiles()
        {
            lock (Files)
            {
                foreach (FileInfo fl in Files)
                    fl.DeleteSafe();

                Files.Clear();
            }
        }
    }
}
