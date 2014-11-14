/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Configuration;
using System.Drawing;
using VPrinting;

namespace VCover.Data
{
    public class Config
    {
        static Config()
        {
            var str = ConfigurationManager.AppSettings["CountryID"];
            int iso;
            CountryID = int.TryParse(str, out iso) ? iso : 0;

            ////-----
            //str = ConfigurationManager.AppSettings["FolderID"];
            //int folderId;
            //FolderID = int.TryParse(str, out folderId) ? folderId : 0;

            ////-----
            //str = ConfigurationManager.AppSettings["CAMERAS"];
            //int cameras;
            //CAMERAS = int.TryParse(str, out cameras) ? cameras : 2;

            ////-----
            //str = ConfigurationManager.AppSettings["FRAME_WIDTH"];
            //int witdh;
            //FRAME_WIDTH = int.TryParse(str, out witdh) ? witdh : 2304;
            ////-----
            //str = ConfigurationManager.AppSettings["FRAME_HEIGHT"];
            //int height;
            //FRAME_HEIGHT = int.TryParse(str, out height) ? height : 1536;

            ////-----
            //str = ConfigurationManager.AppSettings["FRAME_COUNT"];
            //int count;
            //FRAME_COUNT = int.TryParse(str, out count) ? count : 10;

            ////-----
            //str = ConfigurationManager.AppSettings["FRAME_SHOWN_INSEC"];
            //int frameShownInSec;
            //FRAME_SHOWN_INSEC = int.TryParse(str, out frameShownInSec) ? frameShownInSec : 10;

            ////-----
            //str = ConfigurationManager.AppSettings["CommondFolderDeleteWait"];
            //int delay;
            //CommondFolderDeleteWait = int.TryParse(str, out delay) ? delay : 1000;
        }

        public static int CountryID
        {
            get;
            private set;
        }

        public static string OUT_FOLDER
        {
            get
            {
                return ConfigurationManager.AppSettings["OUT_FOLDER"];
            }
        }

        public static string TEMPLATES_FOLDER
        {
            get
            {
                return ConfigurationManager.AppSettings["TEMPLATES_FOLDER"];
            }
        }

        public static string ImageFileFilter
        {
            get
            {
                return ConfigurationManager.AppSettings["ImageFileFilter"];
            }
        }
    }
}