/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DEMATLib.Data;

namespace DEMATLib
{
    public class RetailerProcessor : Processor
    {
        public RetailerProcessor(string exportDirectory)
            : base(exportDirectory)
        {
        }

        public void Process(DateTime date)
        {
            DateTime from = date.Date;
            DateTime to = from.AddDays(1);

            Trace.WriteLine(string.Format("Exporting from {0:dd-MM-yyyy}\t to {1:dd-MM-yyyy}", from, to), "DEMAT");

            try
            {
                var result = DEMARDataAccess.SelectAllRetailersToReport(from, to);

                var b = new BDV_RetailerInfoBuilder();
                foreach (var r in result)
                    b.AddRetailer(r.dmh_br_id, r.dmh_br_DEMAT_contract_date,
                        r.dmh_br_DEMAT_contact_email,
                        r.dmh_br_enable_DEMAT_export);

                var fileName = b.CreateFileName(0);
                var fullFileName = Path.Combine(ExportDirectory, fileName);
                var xml = b.CreateXML();

                if (File.Exists(fullFileName))
                    File.Delete(fullFileName);

                File.WriteAllText(fullFileName, xml);
            }
            catch (Exception ex)
            {
                FireError(ex);
            }
            finally
            {
                Trace.WriteLine("", "DEMAT");
                Thread.Yield();
            }
        }
    }
}
