
using System.Collections.Generic;
using System.Collections;

namespace VPrinting.Pdf
{
    public class PdfCreationInfo
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Author { get; set; }
        public string Creator { get; set; }
        public ArrayList MetaData { get; set; }
    }

    public class PdfSignInfo
    {
        public string pfxFilePath { get; set; }
        public string pfxKeyPass { get; set; }
        public byte[] DocPass { get; set; }
        public string SignImagePath { get; set; }
        public string ReasonForSigning { get; set; }
        public string Location { get; set; }
        public ArrayList MetaData { get; set; }
    }
}
