/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPrint2.Colections;
using CPrint2.Common;
using CPrint2.Data;

namespace CPrint2
{
    public class ImageProcessor
    {
        /// <summary>
        /// KNOW HOW
        /// http://www.emgu.com/forum/viewtopic.php?f=7&t=4238
        /// http://stackoverflow.com/questions/18727747/how-to-get-video-stream-from-webcam-in-emgu-cv
        /// </summary>
        private static readonly IgnoreList<string> ms_Files = new IgnoreList<string>();

        public static event ThreadExceptionEventHandler Error;

        public static readonly ImageProcessor Default = new ImageProcessor();

        public ImageProcessor()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public void ProcessCommand()
        {
            var obj = new DataObj(826, 12345, 1234567, 1);
            MultyCamForm.Default.ProcessCommand(false, obj);
        }

        public void ProcessCommandFile(string fileName)
        {
            if (!ms_Files.Add(fileName))
                return;

            Task.Factory.StartNew((o) =>
            {
                Thread.Sleep(300);

                string fullFileName = Convert.ToString(o);

                var file1 = new FileInfo(fullFileName);

                try
                {
                    string text = File.ReadAllText(fullFileName);

                    var obj = DataObj.Parse(text);

                    if (obj == null || !obj.IsValid)
                        return;

                    MultyCamForm.Default.ProcessCommand(false, obj);
                }
                catch (Exception ex)
                {
                    FireError(ex);
                }
                finally
                {
                    file1.DeleteSafe();
                }

            }, fileName, TaskCreationOptions.LongRunning);
        }     

        public static void EmptyCommandFolderSafe()
        {
            foreach (var file in Directory.GetFiles(Config.CommandInputPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // no errors
                }
            }
        }

        public static void EmptyImageFolderSafe()
        {
            foreach (var file in Directory.GetFiles(Config.ImageOutputPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // no errors
                }
            }
        }

        public static void Clear()
        {
            ms_Files.Clear();
        }

        public static void FireError(Exception ex)
        {
            if (Error != null)
                Error(typeof(ImageProcessor), new ThreadExceptionEventArgs(ex));
        }
    }
}


////Image Sensor: 1/4"CMOS, 640×480 Pixels
////Frame Rate: 30fps@320x240,
////@160x120, 15fps@640x480, @800x600
////Lens: F=2.4, f=3.8 mm
////View Angle: 79 degree
////Focus Range: 10cm to infinity
////Exposure Control: Automatic
////White Balance: Automatic
////Still Image Capture Res.:
////(Installing Driver)
////2560x1920, 1600x1200,
////2048x1536,1280X1024,
////1024x768, 800x600, 640X480,
////352x288, 320x240, 160x120
////Microphone for WebCam: Built-in Microphone
////Flicker Control: 50Hz, 60Hz and None
////Interface: USB 2.0 Port