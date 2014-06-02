using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DTKBarReader;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Serialization;

namespace ReaderProc
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                Guid key = Guid.Parse(args[1]);
                var buffer = ClientDataAccess.SelectBarcodeInfoData(key);

                using(MemoryStream mem = new MemoryStream(buffer))
                using (Image img = Bitmap.FromStream(mem))
                {
                    BarcodeReader reader = new BarcodeReader();
                    reader.BarcodesToRead = 1;
                    reader.BarcodeTypes = BarcodeTypeEnum.BT_Inter2of5;
                    Barcode[] barcodes = reader.ReadFromImage(img);
                    var barArray = new BarcodeInfoArray(barcodes);
                    ObjectSerializer ser = new ObjectSerializer(true);
                    var buffer2 = ser.Serialize(barArray);
                    ClientDataAccess.UpdateBarcodeInfo(key, buffer2);
                }
            }
            Close();
        }
    }
}
