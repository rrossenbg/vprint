/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;

namespace CPrint2.Data
{
    public class DataObj2
    {
        public Guid Id { get; set; }
        public int Iso { get; set; }
        public int VoucherId { get; set; }
        public int PartN { get; set; }
        public int RetailerId { get; set; }

        public DataObj2()
        {
            Id = Guid.NewGuid();
        }

        public DataObj2(int iso, int vid, int part, int brId)
        {
            Iso = iso;
            VoucherId = vid;
            PartN = part;
            RetailerId = brId;
        }

        public static DataObj2 Test()
        {
            return new DataObj2(826, 12345, 2, 123455);
        }

        public static DataObj2 Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            string[] strs = value.Split(';');

            if (strs.Length != 4)
                return null;

            int iso = 0, vid = 0, partN = 0, brid = 0;

            if (!int.TryParse(strs[0], out iso))
                return null;

            if (!int.TryParse(strs[1], out vid))
                return null;

            if (!int.TryParse(strs[2], out partN))
                return null;

            if (!int.TryParse(strs[3], out brid))
                return null;

            var obj = new DataObj2() { Iso = iso, VoucherId = vid, PartN = partN, RetailerId = brid };
            return obj;
        }
    }

    //public class DataObj : IEquatable<DataObj>, ICloneable
    //{
    //    public int Iso { get; set; }
    //    public int BrId { get; set; }
    //    public int VId { get; set; }
    //    public int PartN { get; set; }
    //    public Guid Id { get; private set; }
    //    public string FileName { get; set; }

    //    public bool IsValid
    //    {
    //        get
    //        {
    //            return (Iso > 0 && BrId > 0 && VId > 0 && PartN > 0);
    //        }
    //    }

    //    /// <summary>
    //    /// 826;123456;34567890;1;True
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToString()
    //    {
    //        return string.Format("{0};{1};{2};{3}", Iso, BrId, VId, PartN);
    //    }

    //    public DataObj(DataObj other)
    //    {
    //        this.Iso = other.Iso;
    //        this.BrId = other.BrId;
    //        this.VId = other.VId;
    //        this.PartN = other.PartN;
    //        this.Id = other.Id;
    //    }

    //    public DataObj(int iso, int brid, int vid, int partN)
    //    {
    //        this.Iso = iso;
    //        this.BrId = brid;
    //        this.VId = vid;
    //        this.PartN = partN;
    //        this.Id = CommonTools.ToGuid(iso, brid, vid, partN);
    //    }

    //    /// <summary>
    //    /// 826;123456;34567890;1
    //    /// </summary>
    //    /// <returns></returns>
    //    public static DataObj Parse(string value)
    //    {
    //        if (string.IsNullOrEmpty(value))
    //            throw new ArgumentException("value cannot be null or empty");

    //        string[] strs = value.Split(';');

    //        if (strs.Length != 5)
    //            throw new ArgumentException("strs.Length must be 5 (Iso, BrId, VId, PartN)");

    //        int iso = 0, brid = 0, vid = 0, partN = 0;
    //        bool submit = false;

    //        if (!int.TryParse(strs[0], out iso))
    //            return null;

    //        if (!int.TryParse(strs[1], out brid))
    //            return null;

    //        if (!int.TryParse(strs[2], out vid))
    //            return null;

    //        if (!int.TryParse(strs[3], out partN))
    //            return null;

    //        if (!bool.TryParse(strs[4], out submit))
    //            return null;

    //        var obj = new DataObj(iso, brid, vid, partN);
    //        return obj;
    //    }

    //    public static DataObj Test()
    //    {
    //        return new DataObj(826, 12345, 1234567, 1);
    //    }

    //    public bool Equals(DataObj other)
    //    {
    //        if (other == null)
    //            return false;
    //        return (this.Iso == other.Iso && this.BrId == other.BrId && this.VId == other.VId);
    //    }

    //    public void DeleteFiles()
    //    {
    //        try
    //        {
    //            File.Delete(FileName);
    //        }
    //        catch
    //        {
    //        }
    //    }

    //    public object Clone()
    //    {
    //        return new DataObj(this);
    //    }
    //}
}
