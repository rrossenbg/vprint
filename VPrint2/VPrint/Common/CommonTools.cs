/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime;
using System.Text;
using DTKBarReader;
using Microsoft.CSharp;
using VPrinting.Extentions;

namespace VPrinting.Common
{
    public static class CommonTools
    {
        public static Guid ToGuid(int value1 = 0, int value2 = 0, int value3 = 0, int value4 = 0)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(value1));
            bytes.AddRange(BitConverter.GetBytes(value2));
            bytes.AddRange(BitConverter.GetBytes(value3));
            bytes.AddRange(BitConverter.GetBytes(value4));
            return new Guid(bytes.ToArray());
        }

        public static void FromGuid(Guid guid, out int value1, out int value2, out int value3, out int value4)
        {
            var bytes = guid.ToByteArray();
            value1 = BitConverter.ToInt32(bytes, 0);
            value2 = BitConverter.ToInt32(bytes, 4);
            value3 = BitConverter.ToInt32(bytes, 8);
            value4 = BitConverter.ToInt32(bytes, 12);
        }

        public static object Eval(string sCSCode)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            ICodeCompiler icc = c.CreateCompiler();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.windows.forms.dll");
            cp.ReferencedAssemblies.Add("system.drawing.dll");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using System.Windows.Forms;\n");
            sb.Append("using System.Drawing;\n");

            sb.Append("namespace CSCodeEvaler{ \n");
            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("public object EvalCode(){\n");
            sb.Append("return ");sb.Append(sCSCode );sb.Append("; \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
                throw new Exception(cr.Errors[0].ErrorText);
            Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");

            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");

            object s = mi.Invoke(o, null);
            return s;
        }

        public static string ToBarcode(int countryId, int type, int retailerId, int voucherId)
        {
            //Debug.Assert(countryId >= 0 && countryId < Consts.MAX_COUNTRYID);
            //Debug.Assert(type == 10 || type == 20);
            //Debug.Assert(retailerId > 0 && retailerId < Consts.MAX_RETAILERID);

            var value = string.Format("{0:000}{1:00}{2:000000}{3:00000000}", countryId, type, retailerId, voucherId);
            return string.Concat(value, value.CheckDigit());
        }

        public static bool ParseVoucherImage(ref Bitmap bmp, ref Bitmap bmpBarcode, out Rectangle rect, ref string barcode, BarcodeTypeEnum barcodeType = BarcodeTypeEnum.BT_All)
        {
            lock (typeof(BarcodeReader))
            {
                rect = Rectangle.Empty;
                bmpBarcode = null;
                barcode = null;

                BarcodeReader reader = new BarcodeReader(Strings.VScan_BarcodeReaderSDKDeveloperLicenseKey);
                reader.LicenseManager.AddLicenseKey(Strings.VScan_BarcodeReaderSDKUnlimitedRuntimeLicenseKey);
                reader.BarcodesToRead = 1;
                reader.BarcodeTypes = barcodeType;
                var barcodes = reader.ReadFromBitmapRotateAll(ref bmp, out bmpBarcode, out rect);
                if (barcodes != null && barcodes.Length > 0)
                {
                    barcode = barcodes[0].BarcodeString;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protectedBlock"></param>
        /// <param name="faultHandler"></param>
        /// <returns></returns>
        /// <example>
        /// var f = Fault(() => Console.WriteLine("Okay"),
        /// () => Console.WriteLine("Fault"));
        /// f();
        /// var g = Fault(() => { throw new Exception("Oops"); },
        ///              () => Console.WriteLine("Fault")); 
        /// </example>
        public static Action Fault(Action protectedBlock, Action faultHandler = null)
        {
            return () =>
            {
                bool success = false;
                try
                {
                    protectedBlock();
                    success = true;
                }
                finally
                {
                    if (faultHandler != null && !success)
                        faultHandler();
                }
            };
        }
    }
}
  
