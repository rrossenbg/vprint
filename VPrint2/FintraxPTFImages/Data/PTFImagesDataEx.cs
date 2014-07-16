/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Linq;
using System.Web;

namespace FintraxPTFImages.Data
{
    public partial class PTFImagesDataDataContext
    {
        public static PTFImagesDataDataContext Default
        {
            get
            {
                lock (typeof(PTFImagesDataDataContext))
                {
                    if (HttpContext.Current == null)
                        return new PTFImagesDataDataContext();

                    if (HttpContext.Current.Items["PTFImagesDataDataContext"] == null)
                        HttpContext.Current.Items["PTFImagesDataDataContext"] = new PTFImagesDataDataContext();
                    return (PTFImagesDataDataContext)HttpContext.Current.Items["PTFImagesDataDataContext"];
                }
            }
        }

        public void BeginHistory(int userId, int countryId, string sessionId, DateTime time, bool submit = true)
        {
            //var history = new UserActivityHistory();
            //history.uah_userId = userId;
            //history.uah_isoId = countryId;
            //history.uah_begin = time;
            //history.uah_sessionId = sessionId;
            //history.createdAt = DateTime.Now;

            //this.UserActivityHistories.InsertOnSubmit(history);
            //if (submit)
            //    this.SubmitChanges();
        }

        public void EndHistory(string sessionId, DateTime time, bool submit = true)
        {
            //TODO

            //var history = this.UserActivityHistories.FirstOrDefault((h) => h.uah_sessionId == sessionId);
            //if (history != null)
            //{
            //    history.uah_end = time;
            //    if (submit)
            //        this.SubmitChanges();
            //}
        }
    }
}