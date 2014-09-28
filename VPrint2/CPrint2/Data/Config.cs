/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Configuration;
using System.Drawing;

namespace CPrint2.Data
{
    public class Config
    {
        static Config()
        {
            var str = ConfigurationManager.AppSettings["CountryID"];
            int iso;
            CountryID = int.TryParse(str, out iso) ? iso : 0;

            //-----
            str = ConfigurationManager.AppSettings["FolderID"];
            int folderId;
            FolderID = int.TryParse(str, out folderId) ? folderId : 0;

            //-----
            str = ConfigurationManager.AppSettings["CAMERAS"];
            int cameras;
            CAMERAS = int.TryParse(str, out cameras) ? cameras : 2;

            //-----
            str = ConfigurationManager.AppSettings["FRAME_WIDTH"];
            int witdh;
            FRAME_WIDTH = int.TryParse(str, out witdh) ? witdh : 2304;
            //-----
            str = ConfigurationManager.AppSettings["FRAME_HEIGHT"];
            int height;
            FRAME_HEIGHT = int.TryParse(str, out height) ? height : 1536;

            //-----
            str = ConfigurationManager.AppSettings["FRAME_COUNT"];
            int count;
            FRAME_COUNT = int.TryParse(str, out count) ? count : 10;

            //-----
            str = ConfigurationManager.AppSettings["FRAME_SHOWN_INSEC"];
            int frameShownInSec;
            FRAME_SHOWN_INSEC = int.TryParse(str, out frameShownInSec) ? frameShownInSec : 10;
            
            //-----
            str = ConfigurationManager.AppSettings["CommondFolderDeleteWait"];
            int delay;
            CommondFolderDeleteWait = int.TryParse(str, out delay) ? delay : 1000;
        }

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

        public static int CountryID { get; private set; }

        public static int FolderID { get; private set; }

        public static string PresenterPath
        {
            get
            {
                var s = ConfigurationManager.AppSettings["PresenterPath"];
                return s;
            }
        }

        public static int CAMERAS { get; set; }

        public static string CAMERA_CAPTION
        {
            get
            {
                return ConfigurationManager.AppSettings["CAMERA_CAPTION"];
            }
        }

        public static int FRAME_WIDTH { get; private set; }

        public static int FRAME_HEIGHT { get; private set; }

        public static int FRAME_COUNT { get; private set; }

        public static int FRAME_SHOWN_INSEC { get; private set; }

        /// <summary>
        /// ms
        /// </summary>
        public static int CommondFolderDeleteWait { get; private set; }

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

        public static Size MaxSize
        {
            get
            {
                var s = ConfigurationManager.AppSettings["MaxSize"];
                return s.ParseSize();
            }
        }

        public static Size MinSize
        {
            get
            {
                var s = ConfigurationManager.AppSettings["MinSize"];
                return s.ParseSize();
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
