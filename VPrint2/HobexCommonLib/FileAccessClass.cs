using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using HobexCommonLib.Collections;
using HobexCommonLib.Properties;
using HeaderNP = HobexCommonLib.VoucherNP.AuthenticationHeader;

namespace HobexCommonLib
{
    public class FileAccessClass
    {
        private const string LINE = "\n======================================================\n";

        public int MaxAllocationsCount { get; set; }
        public CustomDictionary m_AlreadyProcessed = new CustomDictionary();
        public List<int> m_TestTerminalIds;
        public List<string> m_ExcludeList = new List<string>();

        public int Count
        {
            get;
            set;
        }

        public FileAccessClass()
        {
            MaxAllocationsCount = 50000;
        }

        public void Prepare(string directory, IEnumerable<int> ignoreList)
        {
            DirectoryInfo di = new DirectoryInfo(directory);

            if (di.Exists)
                m_ExcludeList = di.GetFiles().ToList().ConvertAll(f => Path.GetFileNameWithoutExtension(f.Name));

            m_TestTerminalIds = new List<int>(ignoreList);
        }

        public void Process(HeaderNP header, string fullFileName, string outDir, string errDir)
        {
            m_AlreadyProcessed.Current = DateTime.Now.Date;
            m_AlreadyProcessed.DeleteButCurrent();

            string name = Path.GetFileName(fullFileName);
            string TerminalID = null;
            int CountryID = 0;
            int branchID = 0;
            int OrderValue = 0;
            string Message = "OK";

            CString result = "Begin at: {0}\r\n".format(DateTime.Now);
            try
            {
                result += "Read from In directory" + Environment.NewLine;

                try
                {
                    if (Count > MaxAllocationsCount)
                    {
                        result += "Too many allocations. Exiting..." + Environment.NewLine;
                        File.Move(fullFileName, errDir.GetFileNameInErrDir(fullFileName));
                        Message = "Too many allocations. Exiting...";
                        ServiceAccessClass.RecordImportSafe(fullFileName, CountryID, branchID, TerminalID, OrderValue, Message, null);
                        return;
                    }

                    result += "Processing: ".join(fullFileName) + Environment.NewLine;

                    XDocument xml = XDocument.Load(fullFileName);
                    var terminalId = xml.Root.ElementThrow("TerminalID").Value.ConvertTo<string, int>("TerminalID");
                    var tidName = xml.Root.ElementThrow("TIDName").Value;

                    var originator = xml.Root.ElementThrow("Originator").Value;
                    CountryID = xml.Root.ElementThrow("CountryID").Value.ConvertTo<string, int>("CountryID");
                    branchID = xml.Root.ElementThrow("RetailerID").Value.ConvertTo<string, int>("RetailerID");

                    var dateStr = xml.Root.ElementThrow("Date").Value;
                    var date = dateStr.ConvertTo<string, DateTime>();

                    bool containsTest = tidName.ToLowerSafe().Contains("test") || m_TestTerminalIds.Contains(terminalId);

                    bool alreadyAdded = m_AlreadyProcessed.Exists(Common.ToGuid(CountryID, terminalId, 0, 0));

                    TerminalID = terminalId.ToString();

                    bool allocatedPreviously = m_ExcludeList.Exists((s) => s.Contains(TerminalID));

                    bool allocationIsValid = !containsTest && !alreadyAdded && !allocatedPreviously;

                    // Commented temporary. There are not new stuff on live yet
                    // ServiceAccess.CheckTerminalValid(header, countryId, branchId, terminalId);
                    if (allocationIsValid)
                    {
                        m_AlreadyProcessed.Add(Common.ToGuid(CountryID, terminalId, 0, 0));

                        result += "File valid" + Environment.NewLine;

                        OrderValue = xml.Root.Element("OrderVolume").Value.ConvertTo<string, int>("OrderVolume");
                        if (OrderValue == 0)
                            OrderValue = 200;

                        var sBranchID = (branchID < 99999) ?
                            branchID + ServiceAccessClass.CalculateCheckDigit(header, branchID) :
                            branchID.ToString();

                        branchID = int.Parse(sBranchID);

                        int from = 0, to = 0;
                        string[] vouchers = ServiceAccessClass.AllocateVouchers(header, CountryID, branchID, TerminalID, OrderValue, originator,
                            out from, out to);

                        Count += OrderValue;

                        XDocument outXml = XDocument.Load(new StringReader(Resources.OutputTemplate));
                        outXml.Root.Element("Header").Element("TerminalID").Value = terminalId.ToString();
                        outXml.Root.Element("Header").Element("TIDName").Value = tidName;
                        outXml.Root.Element("Header").Element("Date").Value = dateStr;
                        outXml.Root.Element("Header").Element("OrderVolume").Value = OrderValue.ToString();
                        outXml.Root.Element("Header").Element("VoucherStartRange").Value = (from / 10).ToString();
                        outXml.Root.Element("Header").Element("VoucherEndRange").Value = (to / 10).ToString();

                        foreach (var voucher in vouchers)
                            outXml.Root.Element("Range").Add(new XElement("v_number", voucher));

                        string newName = string.Format("POSHostResponse_{0}_{1:yyMMdd}.xml", terminalId, date);
                        var path = Path.Combine(outDir, newName);
                        outXml.Save(path);

                        File.Delete(fullFileName);

                        ServiceAccessClass.RecordImportSafe(name, CountryID, branchID, TerminalID, OrderValue, Message, xml.ToString(SaveOptions.DisableFormatting));
                    }
                    else
                    {
                        string text = string.Empty;
                        if (containsTest)
                            text = "contains TEST";
                        else if (alreadyAdded)
                            text = "has been already added";
                        else if (allocatedPreviously)
                            text = "allocated previously";

                        Message = string.Format("XML not valid iso: {0} br: {1} pos: {2}. It {3}. \r\n ", CountryID, branchID, terminalId, text);

                        result += Message;

                        File.Move(fullFileName, errDir.GetFileNameInErrDir(fullFileName));

                        ServiceAccessClass.RecordImportSafe(name, CountryID, branchID, TerminalID, OrderValue, Message, xml.ToString(SaveOptions.DisableFormatting));
                    }
                }
                catch (Exception ex1)
                {              
                    result += "{0} \r\nCountryID: {1}\r\nBranchID: {2}\r\nTerminalID: {3}\r\nOrderValue: {4}\r\n{5}\r\n".format(
                                name, CountryID, branchID, TerminalID, OrderValue, ex1.Message);

                    string xmltext = null;
                    if (File.Exists(fullFileName))
                    {
                        xmltext = File.ReadAllText(fullFileName);
                        File.Move(fullFileName, errDir.GetFileNameInErrDir(fullFileName));
                    }

                    ServiceAccessClass.RecordImportSafe(name, CountryID, branchID, TerminalID, OrderValue, ex1.Message, xmltext);
                }
                finally
                {
                    result += "Completed: ".join(fullFileName) + Environment.NewLine;
                }
            }
            catch (Exception ex0)
            {
                result += ex0.ToString() + Environment.NewLine;
            }
            finally
            {
                result += "End at: {0}\r\n".format(DateTime.Now);
                result += LINE;
                Trace.WriteLine(result, "HS");
            }
        }
    }
}
