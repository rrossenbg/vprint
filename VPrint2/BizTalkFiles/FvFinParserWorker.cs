using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Collections.Generic;

namespace BizTalkFiles
{
    public class FvFinParserWorker : CycleWorkerBase, IAppFolderInfoHolder
    {
        public string FvFinInputFolder { get; set; }
        public string FvFinParsedFolder { get; set; }
        public int MaxProcessFilesCount { get; set; }

        public string ErrorFolder { get; set; }
        public string ArchiveFolder { get; set; }
        public string ParsedErrFolder { get; set; }

        protected override void ThreadFunction()
        {
            var dir = new DirectoryInfo(FvFinInputFolder);
            FileInfo[] files = dir.GetFiles("*.xml");

            RootElementProcessor processor = new RootElementProcessor(this);

            int count = 0;


            foreach (var file in files)
            {
                if (!Running)
                    break;

                if (count++ > MaxProcessFilesCount)
                {
                    Trace.WriteLine("Too many files. Gets to Sleep.", "FLV");
                    break;
                }

                Trace.WriteLine("Processing file: {0}".format(file), "FLV");

                if (!file.IsFileLocked())
                {
                    try
                    {
                        HashSet<string> uniqueSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

                        var elements = XDocument.Load(file.FullName).Descendants("root").ToList<XElement>();

                        foreach (XElement element in elements)
                        {
                            processor.Process(element, file, uniqueSet);
                            Thread.Yield();
                        }

                        Trace.WriteLine("Moving archive file: {0} to {1}".format(file.Name, ArchiveFolder), "FLV");
                        var name2 = Path.GetFileNameWithoutExtension(file.Name).Unique().Limit(250).concat(".xml");
                        var fileName = Path.Combine(ArchiveFolder, name2);
                        file.MoveTo(fileName);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error processing file: {0}".format(file.Name), "FLV");
                        Trace.WriteLine(ex.ToString(), "FLV");
                        Trace.WriteLine("Moving file to err folder: {0}".format(ErrorFolder), "FLV");

                        var fileName = Path.Combine(ErrorFolder, file.Name);
                        new Action<string>((fn) => file.MoveTo(fn)).RunSafe(fileName);
                    }
                    finally
                    {
                        FireStep();
                        Trace.WriteLine("=============================", "FLV");
                    }
                }
                else
                {
                    Trace.WriteLine("Splitter: File is locked: {0}".format(file), "FLV");
                }
            }
        }
    }

    public interface IAppFolderInfoHolder
    {
        string FvFinInputFolder { get; set; }
        string FvFinParsedFolder { get; set; }
        string ErrorFolder { get; set; }
        string ArchiveFolder { get; set; }
        string ParsedErrFolder { get; set; }
    }

    public class AppInfoHolder : IAppFolderInfoHolder
    {
        public string FvFinInputFolder { get; set; }
        public string FvFinParsedFolder { get; set; }
        public string ErrorFolder { get; set; }
        public string ArchiveFolder { get; set; }
        public string ParsedErrFolder { get; set; }

        public bool IsInitialized
        {
            get
            {
                return
                    !FvFinInputFolder.IsNullOrWhiteSpace() &&
                    !FvFinParsedFolder.IsNullOrWhiteSpace() &&
                    !ErrorFolder.IsNullOrWhiteSpace() &&
                    !ArchiveFolder.IsNullOrWhiteSpace() &&
                    !ParsedErrFolder.IsNullOrWhiteSpace();
            }
        }
    }

    public class RootElementProcessor
    {
        private readonly Dictionary<string, string> m_Iso;
        private const int MAX_FILE_LENGTH = 250;
        private readonly IAppFolderInfoHolder m_infoholder;

        public RootElementProcessor(IAppFolderInfoHolder infoholder)
        {
            m_infoholder = infoholder;

            m_Iso = new Dictionary<string, string>()
            {
                {"AR", "S"},
                {"AT", "S"},
                {"BE", "S"},
                {"CZ", "S"},
                {"DK", "S"},
                {"FR", null},
                {"DE", null},
                {"GR", "S"},
                {"HU", "S"},
                {"IE", null},
                {"IT", "S"},
                {"LU", "S"},
                {"MX", "D"},
                {"MA", "D"},
                {"NL", "S"},
                {"PT", null},
                {"SG", "D"},
                {"ES", "S"},
                {"SE", "S"},
                {"CH", "S"},
                {"GB", null},
                {"UY", "D"},
            };
        }

        public void Process(XElement element, FileInfo file, HashSet<string> uniqueSet = null)
        {
            string fileName = string.Empty;
            var vnumber = (XElement)element.Nodes().FirstOrDefault((x) => x.ToString().Contains("VoucherNumber"));
            var iso = (XElement)element.Nodes().FirstOrDefault((x) => x.ToString().Contains("RefundCountry"));
            var saleType = (XElement)element.Nodes().FirstOrDefault((x) => x.ToString().Contains("SaleType"));
            if (vnumber == null)
            {
                //NO VOUCHERNUMBER
                Trace.WriteLine("Error: VoucherNumber missing", "FLV");
                var name1 = Path.GetFileNameWithoutExtension(file.Name);
                fileName = Path.Combine(m_infoholder.ParsedErrFolder, name1.Unique().Limit(MAX_FILE_LENGTH).concat(".xml"));
                var content = string.Join("", element.ToString(), "\r\n<!-- NO VOUCHERNUMBER FileName:{0} -->".format(file.Name));
                File.WriteAllText(fileName, content);
            }
            else if(iso == null)
            {
                //NO ISO
                Trace.WriteLine("Error: RefundCountry missing", "FLV");
                var name1 = Path.GetFileNameWithoutExtension(file.Name);
                fileName = Path.Combine(m_infoholder.ParsedErrFolder, name1.Unique().Limit(MAX_FILE_LENGTH).concat(".xml"));
                var content = string.Join("", element.ToString(), "\r\n<!-- NO REFUNDCOUNTRY FileName:{0} -->".format(file.Name));
                File.WriteAllText(fileName, content);
            }
            else if(saleType== null)
            {
                //NO ISO
                Trace.WriteLine("Error: SaleType missing", "FLV");
                var name1 = Path.GetFileNameWithoutExtension(file.Name);
                fileName = Path.Combine(m_infoholder.ParsedErrFolder, name1.Unique().Limit(MAX_FILE_LENGTH).concat(".xml"));
                var content = string.Join("", element.ToString(), "\r\n<!-- NO SALETYPE FileName:{0} -->".format(file.Name));
                File.WriteAllText(fileName, content);
            }
            else if (!vnumber.Value.IsInt())
            {
                //NOT AN INT
                Trace.WriteLine("Error: VoucherNumber not an Int", "FLV");
                var name1 = Path.GetFileNameWithoutExtension(file.Name);
                fileName = Path.Combine(m_infoholder.ParsedErrFolder, name1.Unique().Limit(MAX_FILE_LENGTH).concat(".xml"));
                var content = string.Join("", element.ToString(), "\r\n<!-- VOUCHERNUMBER NOT AN INT FileName:{0}-->".format(file.Name));
                File.WriteAllText(fileName, content);
            }            
            //else if (uniqueSet != null && !uniqueSet.Add(((XElement)vnumber).Value.TrimSafe()))
            //{
            //    //ALREADY INSERTED VOUCHERID
            //    Trace.WriteLine("Error: VoucherNumber not unique", "FLV");
            //    var name1 = Path.GetFileNameWithoutExtension(file.Name);
            //    fileName = Path.Combine(m_infoholder.ParsedErrFolder, name1.Unique().Limit(MAX_FILE_LENGTH).concat(".xml"));
            //    var content = string.Join("", element.ToString(), "\r\n<!-- VOUCHERNUMBER ALREADY INSERTED FileName:{0}-->".format(file.Name));
            //    File.WriteAllText(fileName, content);
            //}
            else
            {
                if (m_Iso[iso.Value] != null && m_Iso[iso.Value] != saleType.Value)
                    saleType.Value = m_Iso[iso.Value];

                //SUCCESSES
                fileName = Path.Combine(m_infoholder.FvFinParsedFolder, "FLV".Unique().Limit(MAX_FILE_LENGTH).concat(".xml"));
                Trace.WriteLine("Creating BizTalk file: {0}".format(fileName), "FLV");

                var builder = new StringBuilder();
                builder.AppendLine("<ns0:VFPData xmlns:ns0='http://DDSchema.CommonEnvelope'>");
                builder.AppendLine(element.ToString());
                builder.AppendLine("</ns0:VFPData>");

                using (StreamWriter writer = new StreamWriter(fileName))
                    writer.Write(builder.ToString());
                builder.Clear();
            }
        }
    }
}