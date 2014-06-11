using System;
using System.Collections.Generic;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.Models
{
    public class HistoryModel
    {
        public string User { get; set; }
        public string Page { get; set; }
        public DateTime Date { get; set; }

        public static explicit operator HistoryModel(History p1)
        {
            var model = new HistoryModel()
            {
                Page = p1.Page,
                Date = p1.Date,
                User = p1.Session.UserDetail.FullName,
            };
            return model;
        }
    }

    public class SessionModel
    {
        public int UserID { get; set; }

        public string User { get; set; }
        public string BrowserSession { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }
        public ICollection<History> History { get; set; }

        public static explicit operator SessionModel(Session p1)
        {
            var model = new SessionModel()
            {
                User = UserDetail.GetFullNameById(p1.UserID.GetValueOrDefault()),
                BrowserSession = p1.BrowserSessionID,
                Begin = p1.Begin,
                End = p1.End.GetValueOrDefault(DateTime.MinValue),
                UserAgent = p1.UserAgent,
                History = p1.Histories,
                IP = p1.IP.IP1,
            };
            return model;
        }
    }
}