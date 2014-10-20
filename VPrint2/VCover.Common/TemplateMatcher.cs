﻿/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;

namespace VCover.Common
{
    public class TemplateMatcher : IDisposable
    {
        private readonly static ArrayList ms_TemplateMatchers = ArrayList.Synchronized(new ArrayList());
        private readonly static ArrayList ms_Templates = ArrayList.Synchronized(new ArrayList());
        private readonly Image<Bgr, byte> m_source;
        private MatchTemplate m_template;
        private Rectangle m_match;

        public TemplateMatcher(string sourceFilePath)
        {
            ms_TemplateMatchers.Add(this);

            m_source = new Image<Bgr, byte>(sourceFilePath);
        }

        public static void AddTemplate(MatchTemplate template)
        {
            ms_Templates.Add(template);
        }

        public bool MatchTemplate(float threshold = 0.65f)
        {
            //#if DEBUG
            //            Image<Bgr, byte> imageToShow = source.Copy();
            //#endif
            MatchTemplate[] arr = new MatchTemplate[ms_Templates.Count];

            ms_Templates.CopyTo(arr);

            foreach (MatchTemplate template in arr)
            {
                using (Image<Gray, float> result = m_source.MatchTemplate(template.Cover.Template, TM_TYPE.CV_TM_CCOEFF_NORMED))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;

                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    // You can try different values of the threshold. 
                    // I guess somewhere between 0.75 and 0.95 would be good.
                    if (maxValues[0] > threshold)
                    {
                        m_template = template;
                        // This is a match. Do something with it, for example draw a rectangle around it.
                        m_match = new Rectangle(maxLocations[0], template.Cover.Template.Size);
                        //#if DEBUG
                        //                    imageToShow.Draw(match, new Bgr(Color.Red), 3);
                        //#endif
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool MatchTemplate(Image<Bgr, byte> source, Image<Bgr, byte> template, ref Rectangle match, float threshold = 0.65f)
        {
            using (Image<Gray, float> result = source.MatchTemplate(template, TM_TYPE.CV_TM_CCOEFF_NORMED))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;

                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                // You can try different values of the threshold. 
                // I guess somewhere between 0.75 and 0.95 would be good.
                if (maxValues[0] > threshold)
                {
                    // This is a match. Do something with it, for example draw a rectangle around it.
                    match = new Rectangle(maxLocations[0], template.Size);
                    //#if DEBUG
                    //                    imageToShow.Draw(match, new Bgr(Color.Red), 3);
                    //#endif
                    return true;
                }
                return false;
            }
        }

        public void PixellateHiddenAreas(string toFileName)
        {
            if (m_template == null)
                throw new ArgumentNullException("m_template");

            foreach (var hidden in m_template.HiddenAreas)
            {
                var r = m_match;
                r.Offset(hidden.Offset.Width, hidden.Offset.Height);
                var r1 = new Rectangle(r.Location, hidden.Rectangle.Size);

                using (var result = m_source.Copy(r1))
                //TODO:: create 1,2,3 file names
                    result.Save(toFileName);

                m_source.Pixellate(r1);
                //m_source.Pixellate(hidden.Rectangle);
            }
        }

        public void Save(string toFileName)
        {
            m_source.Save(toFileName);
        }

        public void Dispose()
        {
            using (m_source) ;
            ms_TemplateMatchers.Remove(this);
        }
    }

    /// <summary>
    /// Where template is. Template info
    /// </summary>
    [Serializable]
    public class MatchTemplate : IDisposable
    {
        private Image<Bgr, byte> m_Image;

        public string Name { get; set; }
        public TemplateInfo Cover = new TemplateInfo();
        public Image<Bgr, byte> Image { get { return m_Image; } set { using (m_Image); m_Image = value; } }

        public List<HiddenAreaInfo> HiddenAreas = new List<HiddenAreaInfo>();
        public bool HasAreas { get { return HiddenAreas.Count != 0; } }

        public void Dispose()
        {
            using (Cover) ;
            using (m_Image) ;
            HiddenAreas.Clear();
        }
    }

    /// <summary>
    /// Where cover is. Cover info.
    /// </summary>
    [Serializable]
    public class TemplateInfo : IDisposable
    {
        private Image<Bgr, byte> m_Template;

        public Rectangle Match { get; set; }

        public Image<Bgr, byte> Template { get { return m_Template; } set { using (m_Template);m_Template = value; } }

        public void Dispose()
        {
            using (m_Template) ;
        }

        public void Clear()
        {
            Match = Rectangle.Empty;
            Template = null;
        }
    }

    /// <summary>
    /// Where hidden area is. Hidden area info.
    /// </summary>
    [Serializable]
    public class HiddenAreaInfo
    {
        public Rectangle Rectangle { get; set; }
        public Size Offset { get; set; }
        public double Distance { get; set; }
    }
}