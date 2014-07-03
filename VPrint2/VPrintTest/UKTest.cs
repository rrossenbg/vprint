using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Common;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class UKTest
    {
        static UKTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public UKTest()
        {

        }

        [TestMethod]
        public void uk_print_format_Demo()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.Tally_T2365_2T;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout826";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print826.xml";
            printer.PrintAllocation(246244, true);
        }

        [TestMethod]
        public void SpeekerTest()
        {
            ManualResetEvent m = new ManualResetEvent(false);

            new Thread(() => { Application.Run(new Form()); m.Set(); }).Start();

            for (int i = 0; i < 100; i++)
                Speeker.SpeakAsynchSf("test " + i);

            m.WaitOne();
        }

        [Flags]
        public enum Names
        {
            None = 0,
            Susan = 1,
            Bob = 2,
            Karen = 4,
            John = 8,
            All = Susan | Bob | Karen | John
        }

        [TestMethod]
        public void TestEnums()
        {
            bool value1 = Names.All.IsSet(Names.Bob);
            Debug.Assert(value1);

            Names friends = Names.Bob | Names.Karen;
            bool value2 = friends.IsSet(Names.Bob);
            Debug.Assert(value2);

            friends = friends.UnSet(Names.Karen);
            Debug.Assert(friends == Names.Bob);
        }

        [TestMethod]
        public void Test_BarcodeConfigigurations()
        {
            var bg = new BarcodeConfig()
                {
                    Name = "Default",
                    Length = 19,
                    HasCheckDigit = true,
                    //iso, ty, br, voucher
                    Template = "{0:000}{1:00}{2:000000}{3:00000000}",
                    Sample = "826 20 188025 33359669 9",
                    CountryID = new Tuple<int, int>(0, 3),
                    BuzType = new Tuple<int, int>(3, 2),
                    RetailerID = new Tuple<int, int>(5, 6),
                    VoucherID = new Tuple<int, int>(11, -1),
                };
            BarcodeData data = null;
            bg.ParseBarcode("001977684056100353", ref data);
        }



        [TestMethod]
        public void ConvertFromUnicodeToAscii()
        {
            ///
            ///http://www.w3.org/TR/encoding/
            ///http://msdn.microsoft.com/en-us/library/system.text.encodinginfo.getencoding(v=vs.110).aspx
            ///

            string convertMe1 = "ΑΝΤΩΝΗΣ ΧΑΤΖΗΠΕΡΟΓΛΟΥ & ΣΙΑ Ε.Ε.";

            var latin1 = Encoding.GetEncoding(1252);//ISO 8859-1
            var gr = Encoding.GetEncoding(1253); //ISO 8859-7
            
            var stt = latin1.GetString(Encoding.Convert(gr, latin1, gr.GetBytes(convertMe1)));

            string convertMe2 = "âìéåï_1".RemoveDiacritics();

            var convertMe3 = "La introducción masiva de las nuevas tecnologías de la información".RemoveDiacritics();

            string convertMe4 = "an arabic character ï»’ in a string".RemoveDiacritics();

            //this encoding will use a blank space for unconvertable characters

            //var uniBytes = Encoding.Unicode.GetBytes(convertMe1);

            ////"ISO646-US"
            ////
            //Encoding grEncoding = System.Text.Encoding.GetEncoding("iso-8859-7", new EncoderReplacementFallback(""), new DecoderReplacementFallback());
            //Encoding utf8Encoding = System.Text.Encoding.GetEncoding("UTF-8");
            //Encoding asciiEncoding = System.Text.Encoding.ASCII;
            //byte[] grBytes = grEncoding.GetBytes(convertMe1);
            ////byte[] resultBytes = Encoding.Convert(Encoding.Unicode, grEncoding, grBytes);
            //byte[] result2Bytes = Encoding.Convert(grEncoding, asciiEncoding, grBytes);
            //var str = asciiEncoding.GetString(result2Bytes);

            System.Text.Encoding iso_8859 = System.Text.Encoding.GetEncoding("iso-8859-7");
            System.Text.Encoding cp869 = System.Text.Encoding.GetEncoding(869); //437);1253

            byte[] iso8859Bytes = iso_8859.GetBytes(convertMe1);
            byte[] cp869Bytes = System.Text.Encoding.Convert(iso_8859, cp869, iso8859Bytes);
            byte[] asciiBytes = System.Text.Encoding.Convert(cp869, Encoding.ASCII, iso8859Bytes);

            var result = cp869.GetString(asciiBytes);

        }

        class Test
        {
            int[] _array;

            public Test()
            {
                Debug.WriteLine("Test()");
                _array = new int[10];
            }

            public int Length
            {
                get
                {
                    return _array.Length;
                }
            }
        }


        /// <summary>
        /// IsValueCreated = False
        /// Test()
        /// IsValueCreated = True
        /// Length = 10
        /// </summary>
        [TestMethod]
        public void TestLazy()
        {
            // Create Lazy.
            Lazy<Test> lazy = new Lazy<Test>();

            Debug.WriteLine("IsValueCreated = {0}", lazy.IsValueCreated);

            // Get the Value.
            // ... This executes Test().
            Test test = lazy.Value;

            // Show the IsValueCreated is true.
            Debug.WriteLine("IsValueCreated = {0}", lazy.IsValueCreated);

            // The object can be used.
            Debug.WriteLine("Length = {0}", test.Length);
            Console.Read();
        }

        public static class GreekEncoding
        {
            public static string GetString(byte[] bytes)
            {
                return GetString(bytes, 0);
            }
            public static string GetString(byte[] bytes, int startposition)
            {
                return GetString(bytes, startposition, bytes.Length);
            }
            public static string GetString(byte[] bytes, int startposition, int count)
            {
                var answer = new StringBuilder();

                for (var i = startposition; i < count + startposition; i++)
                {
                    var c = bytes[i];
                    if (Convert.ToInt32(c) > 127)
                        answer.Append(GetChar(c));
                    else
                        answer.Append(Convert.ToChar(c));
                }

                return answer.ToString();
            }
            public static char GetChar(byte value)
            {
                return GetChar(Convert.ToInt32(value));
            }
            public static char GetChar(int value)
            {
                switch (value)
                {
                    case 160:
                        return ' ';

                    case 161:
                        return '\'';

                    case 162:
                        return '\'';

                    case 163:
                        return '£';

                    case 164:
                        return '€';

                    case 165:
                        return '₯';

                    case 166:
                        return '¦';

                    case 167:
                        return '§';

                    case 168:
                        return '¨';

                    case 169:
                        return '©';

                    case 170:
                        return 'ͺ';

                    case 171:
                        return '«';

                    case 172:
                        return '¬';

                    case 173:
                        return '-';

                    case 175:
                        return '-';

                    case 176:
                        return '°';

                    case 177:
                        return '±';

                    case 178:
                        return '²';

                    case 179:
                        return '³';

                    case 180:
                        return '΄';

                    case 181:
                        return '΅';

                    case 182:
                        return 'Ά';

                    case 183:
                        return '·';

                    case 184:
                        return 'Έ';

                    case 185:
                        return 'Ή';

                    case 186:
                        return 'Ί';

                    case 187:
                        return '»';

                    case 188:
                        return 'Ό';

                    case 189:
                        return '½';

                    case 190:
                        return 'Ύ';

                    case 191:
                        return 'Ώ';

                    case 192:
                        return 'ΐ';

                    case 193:
                        return 'Α';

                    case 194:
                        return 'Β';

                    case 195:
                        return 'Γ';

                    case 196:
                        return 'Δ';

                    case 197:
                        return 'Ε';

                    case 198:
                        return 'Ζ';

                    case 199:
                        return 'Η';

                    case 200:
                        return 'Θ';

                    case 201:
                        return 'Ι';

                    case 202:
                        return 'Κ';

                    case 203:
                        return 'Λ';

                    case 204:
                        return 'Μ';

                    case 205:
                        return 'Ν';

                    case 206:
                        return 'Ξ';

                    case 207:
                        return 'Ο';

                    case 208:
                        return 'Π';

                    case 209:
                        return 'Ρ';

                    case 211:
                        return 'Σ';

                    case 212:
                        return 'Τ';

                    case 213:
                        return 'Υ';
                    case 214:
                        return 'Φ';
                    case 215:
                        return 'Χ';
                    case 216:
                        return 'Ψ';
                    case 217:
                        return 'Ω';
                    case 218:
                        return 'Ϊ';
                    case 219:
                        return 'Ϋ';
                    case 220:
                        return 'ά';
                    case 221:
                        return 'έ';
                    case 222:
                        return 'ή';
                    case 223:
                        return 'ί';
                    case 224:
                        return 'ΰ';
                    case 225:
                        return 'α';
                    case 226:
                        return 'β';
                    case 227:
                        return 'γ';
                    case 228:
                        return 'δ';
                    case 229:
                        return 'ε';
                    case 230:
                        return 'ζ';
                    case 231:
                        return 'η';
                    case 232:
                        return 'θ';
                    case 233:
                        return 'ι';
                    case 234:
                        return 'κ';
                    case 235:
                        return 'λ';
                    case 236:
                        return 'μ';
                    case 237:
                        return 'ν';
                    case 238:
                        return 'ξ';
                    case 239:
                        return 'ο';
                    case 240:
                        return 'π';
                    case 241:
                        return 'ρ';
                    case 242:
                        return 'ς';
                    case 243:
                        return 'σ';
                    case 244:
                        return 'τ';
                    case 245:
                        return 'υ';
                    case 246:
                        return 'φ';
                    case 247:
                        return 'χ';
                    case 248:
                        return 'ψ';
                    case 249:
                        return 'ω';
                    case 250:
                        return 'ϊ';
                    case 251:
                        return 'ϋ';
                    case 252:
                        return 'ό';
                    case 253:
                        return 'ύ';
                    case 254:
                        return 'ώ';

                }
                return Convert.ToChar(value);
            }
        }
    }

    public static class Ext
    {
        public static bool IsSet<T>(this T flags, T flag) where T : struct
        {
            Debug.Assert(typeof(T).IsEnum, "Wrong type");

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        public static T Set<T>(this T flags, T flag) where T : struct
        {
            Debug.Assert(typeof(T).IsEnum, "Wrong type");

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (T)(object)(flagsValue | flagValue);
        }

        public static T UnSet<T>(this T flags, T flag) where T : struct
        {
            Debug.Assert(typeof(T).IsEnum, "Wrong type");

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (T)(object)(flagsValue & (~flagValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>convert é to e and etc.</returns>
        /// <see cref="http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net"/>
        public static string RemoveDiacritics(this string text)
        {
            if (text == null)
                throw new ArgumentNullException();

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var b = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    b.Append(c);
            }

            return b.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
