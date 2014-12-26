/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class StringEx2
    {
        [TargetedPatchingOptOut("na")]
        public static string GetContentType(this string fullFileName)
        {
            if (string.IsNullOrWhiteSpace(fullFileName))
                return "";

            var ext = Path.GetExtension(fullFileName);

            switch (ext.ToLowerInvariant())
            {
                case ".bmp": 
                    return "image/bmp";
                case ".jpg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".tif":
                    return "image/tiff";
                case ".png":
                    return "image/png";
                case ".html":
                    return "text/html";
                case ".js":
                    return "text/javascript";
                case ".xml":
                    return "text/xml";
                default:
                    return "";
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void StartProcessSafe(this string fullFileName)
        {
            try
            {
                Process.Start(new ProcessStartInfo(fullFileName));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
