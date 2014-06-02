/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;

namespace VPrinting.Attributes
{
    public class OnLiveAttribute : Attribute
    {
        public DateTime Date { get; set; }
        public string Comment { get; set; }

        public OnLiveAttribute()
        {
            Date = DateTime.Today;
        }

        public OnLiveAttribute(string date)
        {
            DateTime d;
            if (DateTime.TryParse(date, out d))
                Date = d;
            else
                Date = DateTime.Today;
        }

        public OnLiveAttribute(string comment, string date)
            : this(date)
        {
            Comment = comment;
        }
    }
}
