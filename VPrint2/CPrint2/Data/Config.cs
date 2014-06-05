/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Configuration;

namespace CPrint2.Data
{
    public class Config
    {
        public static string CommandInputPath
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandInputPath"];
            }
        }

        public static string ImageOutputPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ImageOutputPath"];
            }
        }

        public static string CommandFilter
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandFilter"];
            }
        }

        public static string ImageFileFilter
        {
            get
            {
                return ConfigurationManager.AppSettings["ImageFileFilter"];
            }
        }

        public static string LiveServerIP
        {
            get
            {
                return ConfigurationManager.AppSettings["LiveServerIP"];
            }
        }

        /// <summary>
        /// 192.168.58.59
        /// </summary>
        public static string TestServerIP
        {
            get
            {
                return ConfigurationManager.AppSettings["TestServerIP"];
            }
        }

        public static string ScanServerIP
        {
            get
            {
                return ConfigurationManager.AppSettings["ScanServerIP"];
            }
        }

        public static int CountryID
        {
            get
            {
                var s = ConfigurationManager.AppSettings["CountryID"];
                int iso;
                return int.TryParse(s, out iso) ? iso : 0;
            }
        }

        public static int FolderID
        {
            get
            {
                var s = ConfigurationManager.AppSettings["FolderID"];
                int folderId;
                return int.TryParse(s, out folderId) ? folderId : 0;
            }
        }

        public static string PresenterPath
        {
            get
            {
                var s = ConfigurationManager.AppSettings["PresenterPath"];
                return s;
            }
        }

        /// <summary>
        /// ms
        /// </summary>
        public static int CommondFolderDeleteWait
        {
            get
            {
                var s = ConfigurationManager.AppSettings["CommondFolderDeleteWait"];
                int delay;
                return int.TryParse(s, out delay) ? delay : 1000;
            }
        }

        /// <summary>
        /// ms
        /// </summary>
        public static int ImagePickupDelay
        {
            get
            {
                var s = ConfigurationManager.AppSettings["ImagePickupDelay"];
                int delay;
                return int.TryParse(s, out delay) ? delay : 1000;
            }
        }

        public static int MinWidth
        {
            get
            {
                var s = ConfigurationManager.AppSettings["MinWidth"];
                int n;
                return int.TryParse(s, out n) ? n : 100;
            }
        }

        public static int MaxWidth
        {
            get
            {
                var s = ConfigurationManager.AppSettings["MaxWidth"];
                int n;
                return int.TryParse(s, out n) ? n : 100;
            }
        }

        public static int MinHeight
        {
            get
            {
                var s = ConfigurationManager.AppSettings["MinHeight"];
                int n;
                return int.TryParse(s, out n) ? n : 100;
            }
        }

        public static int MaxHeight
        {
            get
            {
                var s = ConfigurationManager.AppSettings["MaxHeight"];
                int n;
                return int.TryParse(s, out n) ? n : 100;
            }
        }

        public static bool GrayPicture
        {
            get
            {
                var s = ConfigurationManager.AppSettings["GrayPicture"];
                bool n;
                return bool.TryParse(s, out n) ? n : false;
            }
        }
    }
}
