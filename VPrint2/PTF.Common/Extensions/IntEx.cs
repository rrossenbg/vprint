/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class IntEx
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsValueInRange(this int value, int min, int max)
        {
            return (min <= value && value <= max);
        }

        [TargetedPatchingOptOut("na")]
        public static int NextInt(this Random r, int min, int max)
        {
            return r.Next(min, max);
        }

        [TargetedPatchingOptOut("na")]
        public static int SetValueInRange(this int value, int min, int max)
        {
            if (min <= value && value <= max)
                return value;
            else if (value > max)
                return max;
            else //if ( value < min)
                return min;
        }

        /// <summary>
        /// (value1 == value2) || (value1 == value2 * 10 + checkDigit2);
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="checkDigit2"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static bool EqualsWithCheckDigit(this int value1, int value2, int checkDigit2)
        {
            return (value1 == value2) || (value1 == value2 * 10 + checkDigit2);
        }

        private static readonly int[] WEIGHTS = new int[] { 2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Value + CheckDigit</returns>
        [TargetedPatchingOptOut("na")]
        public static int CheckDigit(this int value)
        {
            char[] base_val = value.ToString().ToCharArray();

            Array.Reverse(base_val);

            int i, sum;

            for (i = 0, sum = 0; i < base_val.Length; i++)
                sum += int.Parse(base_val[i].ToString()) * WEIGHTS[i];

            // Determine check digit.
            return (value * 10) + ((sum % 11) % 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>CheckDigit</returns>
        [TargetedPatchingOptOut("na")]
        public static int CheckDigit(this string value)
        {
            Debug.Assert(!value.IsNullOrEmpty());

            char[] base_val = value.ToCharArray();

            Array.Reverse(base_val);

            long i, sum;

            for (i = 0, sum = 0; i < base_val.Length; i++)
            {
                int v = 0;

                bool result = int.TryParse(base_val[i].ToString(), out v);

                Debug.Assert(result, "Cannot parse value");

                sum += v * WEIGHTS[i];
            }

            // Determine check digit.
            return (int)((sum % 11) % 10);
        }

        [TargetedPatchingOptOut("na")]
        public static bool CompareSmart(this int value1, int value1cd, int value2, int value2cd)
        {
            return value1 == value2 || value1 == value2cd || value1cd == value2 || value1cd == value2cd;
        }

        [TargetedPatchingOptOut("na")]
        public static float FromInch(this int value)
        {
            return Convert.ToSingle(PrinterUnitConvert.Convert(value, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display) * 1000);
        }

        [TargetedPatchingOptOut("na")]
        public static float FromInch(this float value)
        {
            return Convert.ToSingle(PrinterUnitConvert.Convert(value, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display) * 1000);
        }

        [TargetedPatchingOptOut("na")]
        public static float FromMm(this float value)
        {
            return Convert.ToSingle(PrinterUnitConvert.Convert(value, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.Display) * 100);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static DirectoryInfo ThrowIfNotExist(this DirectoryInfo info, string message)
        {
            Debug.Assert(info != null);

            info.Refresh();
            if (!info.Exists)
                throw new Exception(message);
            return info;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static DirectoryInfo CreateIfNotExist(this DirectoryInfo info)
        {
            Debug.Assert(info != null);

            info.Refresh();
            if (!info.Exists)
                info.Create();
            return info;
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsEmpty(this DirectoryInfo info, string filter = "*")
        {
            Debug.Assert(info != null);
            info.Refresh();
            Debug.Assert(info.Exists);
            var infos = info.GetFiles(filter);
            return infos.Length == 0;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static bool IsLocked(this FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    stream.Close();

                //file is not locked
                return false;
            }
            catch
            {
                return true;
            }
        }

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static DirectoryInfo Combine(this DirectoryInfo info, string subFolder)
        //{
        //    return new DirectoryInfo(Path.Combine(info.FullName, subFolder));
        //}

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static FileInfo CombineFileName(this DirectoryInfo info, string fileName)
        //{
        //    return new FileInfo(Path.Combine(info.FullName, fileName));
        //}

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void WriteAllBytes(this FileInfo file, byte[] bytes, int length = 0)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (bytes.Length == 0)
                throw new ArithmeticException("Buffer is empty");

            using (var stream = file.OpenWrite())
                stream.Write(bytes, 0, length > 0 ? length : bytes.Length);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string ReadAllText(this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            using (var reader = file.OpenText())
                return reader.ReadToEnd();
        }

        //[TargetedPatchingOptOut("na")]
        //[Obfuscation]
        //public static byte[] ReadAllBytes(this FileInfo file)
        //{
        //    if (file == null)
        //        throw new ArgumentNullException("file");

        //    return File.ReadAllBytes(file.FullName);
        //}

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string GetFileName(this FileInfo info)
        {
            Debug.Assert(info != null);
            return Path.GetFileNameWithoutExtension(info.Name);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string GetFileNameWithoutExtension(this FileInfo info)
        {
            Debug.Assert(info != null);
            return Path.GetFileNameWithoutExtension(info.FullName);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void MoveTo(this DirectoryInfo fromInfo, DirectoryInfo toInfo)
        {
            Debug.Assert(fromInfo != null);
            Debug.Assert(fromInfo.Exists);

            Debug.Assert(toInfo != null);
            Debug.Assert(toInfo.Exists);

            foreach (var file in fromInfo.GetFiles())
            {
                var newFile = toInfo.CombineFileName(file.Name);
                file.MoveTo(newFile.FullName);
            }

            fromInfo.Delete(true);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static void Read(this FileInfo file, int from, int length, byte[] buffer)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (file.Length - from < 0)
                throw new ArgumentOutOfRangeException("from");

            using (var reader = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                reader.Seek(from, SeekOrigin.Begin);
                reader.Read(buffer, 0, length);
            }
        }
    }
}
