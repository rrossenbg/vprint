
namespace VPrinting.Pdf
{
    public class PdfCreationInfo
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Author { get; set; }
        public string Creator { get; set; }
    }

    public class PdfSignInfo
    {
        public string pfxFilePath { get; set; }
        public string pfxKeyPass { get; set; }
        public byte[] docPass { get; set; }
        public string signImagePath { get; set; }
        public string reasonForSigning { get; set; }
        public string location { get; set; }
    }
}
