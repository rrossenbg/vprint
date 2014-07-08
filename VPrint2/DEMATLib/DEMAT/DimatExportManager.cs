using System;
using System.Diagnostics;
using System.Threading;

namespace DEMATLib
{
    public class DIMATExportManager
    {
        public static int Iso { get; set; }
        public static volatile string ExportDirectory;

        public static event ThreadExceptionEventHandler Error
        {
            add
            {
                InvoiceProcessor.Error += value;
                RetailerProcessor.Error += value;
            }
            remove
            {
                InvoiceProcessor.Error -= value;
                RetailerProcessor.Error -= value;
            }
        }

        public volatile bool FirstRun;

        public void Start()
        {
            Trace.WriteLine("Begin", Strings.DEMAT);

            var processor = new InvoiceProcessor(Iso, ExportDirectory);
            if (FirstRun)
                processor.ZeroExportNumbers();

            processor.ProcessAll();

            if (FirstRun)
            {
                var rprocessor = new RetailerProcessor(ExportDirectory);
                rprocessor.Process(DateTime.Now);
            }

            Trace.WriteLine("End", Strings.DEMAT);
        }

        public static void FirceRetailerExport()
        {
            var rprocessor = new RetailerProcessor(ExportDirectory);
            rprocessor.Process(DateTime.Now);
        }
    }
}
