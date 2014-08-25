/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CardCodeCover
{
    public class TemplateInfoLight
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ImageSize { get; set; }
        public int TemplateSize { get; set; }
        public DateTime DateCreated { get; set; }

        protected DataAccess.TemplateInfoDb m_dbInfo;

        public TemplateInfoLight()
        {
        }

        public TemplateInfoLight(DataAccess.TemplateInfoDb info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Id = info.Id;
            Name = info.Name;
            ImageSize = info.ImageSize;
            TemplateSize = info.TemplateSize;
            DateCreated = info.CreatedAt;

            m_dbInfo = info;
        }

        public static explicit operator DataAccess.TemplateInfoDb(TemplateInfoLight info)
        {
            return info.m_dbInfo;
        }
    }

    public class TemplateInfo : TemplateInfoLight, IDisposable
    {
        public static TemplateInfo Instance
        {
            get
            {
                return new TemplateInfo();
            }
        }

        public Image<Bgr, byte> Image { get; set; }

        public Image<Bgr, byte> Template { get; set; }

        public CoverInfo CoverDetails { get; set; }

        public TemplateInfo()
        {
            CoverDetails = new CoverInfo();
            m_dbInfo = new DataAccess.TemplateInfoDb();
        }

        public TemplateInfo(TemplateInfoLight info)
        {
            base.m_dbInfo = (DataAccess.TemplateInfoDb)info;
            this.Name = info.Name;
            this.Image = new Image<Bgr, byte>((Bitmap)Bitmap.FromStream(new MemoryStream(base.m_dbInfo.Image)));
            this.Template = new Image<Bgr, byte>((Bitmap)Bitmap.FromStream(new MemoryStream(base.m_dbInfo.Template)));
            this.CoverDetails = base.m_dbInfo.HiddenAreas.ToObject<CoverInfo>();
        }

        public TemplateInfo Commit()
        {
            var dbinfo = m_dbInfo;
            dbinfo.Name = Name;

            lock (this.Image)
                dbinfo.Image = this.Image.Bitmap.ToArray(85L);

            dbinfo.ImageSize = dbinfo.Image.Length;

            lock (this.Template)
                dbinfo.Template = this.Template.Bitmap.ToArray(85L);

            dbinfo.TemplateSize = dbinfo.Template.Length;
            dbinfo.HiddenAreas = this.CoverDetails.FromObject();
            return this;
        }

        public void Dispose()
        {
            using (Image) ;
            using (Template) ;
        }
    }

    [Serializable]
    public class Cover
    {
        public double Distance { get; set; }
        public Rectangle Rectangle { get; set; }
    }

    [Serializable]
    public class CoverInfo
    {
        public Rectangle Match { get; set; }
        public List<Cover> HiddenAreas { get; set; }

        public bool HasMatch
        {
            get
            {
                return this.Match != Rectangle.Empty;
            }
        }

        public CoverInfo()
        {
            HiddenAreas = new List<Cover>();
        }

        public void Clear()
        {
            Match = Rectangle.Empty;
            HiddenAreas.Clear();
        }
    }
}
