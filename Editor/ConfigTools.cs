/*
 * Author:  Rick
 * Create:  2017/7/10 11:57:52
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 配置工具集
    /// </summary>
    public class ConfigTools
    {
        public static bool IsArrayType(string sourceType)
        {
            int idx = sourceType.LastIndexOf('[');
            return idx != -1;
        }

        /// <summary>
        /// 判断是否是数组类型
        /// </summary>
        /// <returns></returns>
        public static bool IsArrayType(string sourceType,out string sourceBaseType)
        {
            int idx = sourceType.LastIndexOf('[');
            if (idx != -1)
                sourceBaseType = SourceBaseType2CSharpBaseType(sourceType.Substring(0, idx));
            else
                sourceBaseType = sourceType;
            return idx != -1;
        }

        #region 将SourceType转换为CSharp
        /// <summary>
        /// 将类型字符串转换为C#基础类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SourceType2CSharpType(int column ,string[,] matrix)
        {
            string sourceType = matrix[1, column];

            if (string.IsNullOrEmpty(sourceType) || sourceType == "*")
            {
                return Smart2CSharpType(column, matrix);
            }
            else if (sourceType == "*[]")
            {
                return Smart2CSharpArrayType(column, matrix);
            }
            else
            {
                string sourceBaseType;
                if (IsArrayType(sourceType, out sourceBaseType))
                {
                    return sourceBaseType + "[]";
                }
                return SourceBaseType2CSharpBaseType(sourceType);
            }
        }

        /// <summary>
        /// 将SourceBaseType转换为C#基础类型
        /// </summary>
        /// <param name="sourceBaseType"></param>
        /// <returns></returns>
        private static string SourceBaseType2CSharpBaseType(string sourceBaseType)
        {
            string baseType;
            switch (sourceBaseType)
            {
                case "bool":
                    baseType = "bool";
                    break;
                case "byte":
                case "uint8":
                    baseType = "byte";
                    break;
                case "ushort":
                case "uint16":
                    baseType = "ushort";
                    break;
                case "uint":
                case "uint32":
                    baseType = "uint";
                    break;
                case "ulong":
                case "uint64":
                    baseType = "ulong";
                    break;
                case "sbyte":
                case "int8":
                    baseType = "sbyte";
                    break;
                case "short":
                case "int16":
                    baseType = "short";
                    break;
                case "int":
                case "int32":
                    baseType = "int";
                    break;
                case "long":
                case "int64":
                    baseType = "long";
                    break;
                case "float":
                    baseType = "float";
                    break;
                case "double":
                    baseType = "double";
                    break;
                case "string":
                    baseType = "string";
                    break;
                default:
                    baseType = sourceBaseType;
                    break;
            }
            return baseType;
        }
        #endregion

        #region 智能判断CSharp类型
        private static string Smart2CSharpType(int column, string[,] matrix)
        {
            int row = matrix.GetLength(0);
            string valid = FindValidValue(column, matrix);

            if (IsNumber(valid))
            {
                string[] numbersOfColumn = new string[row - 3];//这行所有的Number
                for(int y = 3,i = 0;y<row;y++,i++)
                    numbersOfColumn[i] = matrix[y,column];

                Type minimalType = FindMinimalNumberType(numbersOfColumn);//最小的数字类型
                return Type2SourceBaseType(minimalType);
            }
            else if (IsBool(valid))
            {
                return Type2SourceBaseType(typeof(bool));
            }
            else
            {
                return "string";
            }
        }
        private static string Smart2CSharpArrayType(int column, string[,] matrix)
        {
            int row = matrix.GetLength(0);
            string valid = FindValidValue(column, matrix);

            string[] validArray = valid.Split(',');
            if (IsNumber(validArray[0]))
            {
                List<object> numbersOfColumn = new List<object>();//不仅要遍历行，还要遍历数组中的值
                for (int y = 3, i = 0; y < row; y++, i++)
                {
                    string rowData = matrix[y, column];
                    if (string.IsNullOrEmpty(rowData))
                        continue;

                    numbersOfColumn.AddRange(rowData.Split(','));
                }

                Type minimalType = FindMinimalNumberType(numbersOfColumn);
                return Type2SourceBaseType(minimalType) + "[]";
            }
            else if (IsBool(validArray[0]))
            {
                return Type2SourceBaseType(typeof(bool)) + "[]";
            }
            else
                return "string[]";
        }

        private static string FindValidValue(int column, string[,] matrix)
        {
            string valid = "";
            int row = matrix.GetLength(0);//行总数
            for (int y = 3; y < row; y++)
            {
                valid = matrix[y, column];
                if (!string.IsNullOrEmpty(valid))
                    break;
            }
            return valid;
        }
        #endregion

        #region 将SourceType <-> SystemType
        private static Type SourceBaseType2Type(string sourceBaseType)
        {
            string csharpBaseType = SourceBaseType2CSharpBaseType(sourceBaseType);
            switch (csharpBaseType)
            {
                case "bool":
                    return typeof(bool);
                case "byte":
                    return typeof(byte);
                case "ushort":
                    return typeof(ushort);
                case "uint":
                    return typeof(uint);
                case "sbyte":
                    return typeof(sbyte);
                case "short":
                    return typeof(short);
                case "int":
                    return typeof(int);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                case "string":
                    return typeof(string);
                default:
                    return null;
            }
        }
        private static string Type2SourceBaseType(Type type)
        {
            if (type == typeof(bool))
                return "bool";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(ushort))
                return "ushort";
            else if (type == typeof(uint))
                return "uint";
            else if (type == typeof(sbyte))
                return "sbyte";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(int))
                return "int";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(ulong))
                return "ulong";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(string))
                return "string";

            return "";
        }
        #endregion

        #region 将SourceValue转换为Object
        /// <summary>
        /// 解析源值
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static object SourceValue2Object(string sourceType, string sourceValue)
        {
            string sourceBaseType;
            if (IsArrayType(sourceType,out sourceBaseType))
            {
                return SourceValue2ArrayObject(sourceType, sourceValue, sourceBaseType);
            }
            else
            {
                //返回独值
                return SourceValue2BaseObject(sourceType, sourceValue);
            }
        }

        private static object SourceValue2BaseObject(string sourceBaseType,string sourceValue)
        {
            string csharpType = SourceBaseType2CSharpBaseType(sourceBaseType);
            switch (csharpType)
            {
                case "bool":
                    return sourceValue != "0" && sourceValue != "false" && sourceValue != "False" && sourceValue != "FALSE";
                case "byte":
                    return byte.Parse(sourceValue);
                case "ushort":
                    return ushort.Parse(sourceValue);
                case "uint":
                    return uint.Parse(sourceValue);
                case "sbyte":
                    return sbyte.Parse(sourceValue);
                case "short":
                    return short.Parse(sourceValue);
                case "int":
                    return int.Parse(sourceValue);
                case "long":
                    return long.Parse(sourceValue);
                case "ulong":
                    return ulong.Parse(sourceValue);
                case "float":
                    return float.Parse(sourceValue);
                case "double":
                    return double.Parse(sourceValue);
                case "string":
                    return sourceValue;
                default:
                    return sourceValue;
            }
        }

        /// <summary>
        /// 将配置的数组转换为C#数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cValue"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static Array SourceValue2ArrayObject(string sourceType,string sourceValue,string sourceBaseType)
        {
            //解析数组
            if (string.IsNullOrEmpty(sourceValue) || sourceValue == "0") return null;

            string[] values = sourceValue.Split(',');
            Type type = SourceBaseType2Type(sourceBaseType);
            Array array = Array.CreateInstance(type,values.Length);
            for(int i = 0,l = array.Length;i<l;i++)
            {
                object value = SourceValue2BaseObject(sourceBaseType, values[i]);
                array.SetValue(value,i);
            }
            return array;
        }
        #endregion

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteFile(string path,string content)
        {
            //文件写入
            FileStream fileStream;
            if (!File.Exists(path))
            {
                fileStream = File.Create(path);
            }
            else
            {
                fileStream = File.Open(path, FileMode.Truncate);
            }

            StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            writer.Write(content);
            writer.Close();
        }


        /// <summary>
        /// 读取文件内容 
        /// Fork:
        /// https://stackoverflow.com/questions/1389155/easiest-way-to-read-text-file-which-is-locked-by-another-application
        /// https://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile(FileStream fileStream,out byte[] bytes)
        {
            BinaryReader br = new BinaryReader(fileStream);
            bytes = br.ReadBytes((int)fileStream.Length);
            StreamReader renderer = new StreamReader(fileStream);
            string content = renderer.ReadToEnd();
            fileStream.Close();
            return content;
        }

        private static Encoding gb2312 = Encoding.GetEncoding("gb2312");
        // Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
        // & big endian), and local default codepage, and potentially other codepages.
        // 'taster' = number of bytes to check of the file (to save processing). Higher
        // value is slower, but more reliable (especially UTF-8 with special characters
        // later on may appear to be ASCII initially). If taster = 0, then taster
        // becomes the length of the file (for maximum reliability). 'text' is simply
        // the string with the discovered encoding applied to the file.
        public static Encoding DetectTextEncoding(byte[] b, out String text, int taster = 1000)
        {
            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { text = Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4); return Encoding.GetEncoding("utf-32BE"); }  // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { text = Encoding.UTF32.GetString(b, 4, b.Length - 4); return Encoding.UTF32; }    // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { text = Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2); return Encoding.BigEndianUnicode; }     // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { text = Encoding.Unicode.GetString(b, 2, b.Length - 2); return Encoding.Unicode; }              // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { text = Encoding.UTF8.GetString(b, 3, b.Length - 3); return Encoding.UTF8; } // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { text = Encoding.UTF7.GetString(b, 3, b.Length - 3); return Encoding.UTF7; } // UTF-7


            //////////// If the code reaches here, no BOM/signature was found, so now
            //////////// we need to 'taste' the file to see if can manually discover
            //////////// the encoding. A high taster value is desired for UTF-8
            if (taster == 0 || taster > b.Length) taster = b.Length;    // Taster size can't be bigger than the filesize obviously.


            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: https://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: https://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            int i = 0;
            bool utf8 = false;
            while (i < taster - 4)
            {
                if (b[i] <= 0x7F) { i += 1; continue; }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false; break;
            }
            if (utf8 == true)
            {
                text = Encoding.UTF8.GetString(b);
                return Encoding.UTF8;
            }


            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            int count = 0;
            for (int n = 0; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.BigEndianUnicode.GetString(b); return Encoding.BigEndianUnicode; }
            count = 0;
            for (int n = 1; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.Unicode.GetString(b); return Encoding.Unicode; } // (little-endian)


            // Finally, a long shot - let's see if we can find "charset=xyz" or
            // "encoding=xyz" to identify the encoding:
            for (int n = 0; n < taster - 9; n++)
            {
                if (
                    ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
                    ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
                    )
                {
                    if (b[n + 0] == 'c' || b[n + 0] == 'C') n += 8; else n += 9;
                    if (b[n] == '"' || b[n] == '\'') n++;
                    int oldn = n;
                    while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
                    { n++; }
                    byte[] nb = new byte[n - oldn];
                    Array.Copy(b, oldn, nb, 0, n - oldn);
                    try
                    {
                        string internalEnc = Encoding.ASCII.GetString(nb);
                        text = Encoding.GetEncoding(internalEnc).GetString(b);
                        return Encoding.GetEncoding(internalEnc);
                    }
                    catch { break; }    // If C# doesn't recognize the name of the encoding, break.
                }
            }

            // If all else fails, the encoding is probably (though certainly not
            // definitely) the user's local codepage! One might present to the user a
            // list of alternative encodings as shown here: https://stackoverflow.com/questions/8509339/what-is-the-most-common-encoding-of-each-language
            // A full list can be found using Encoding.GetEncodings();
            text = gb2312.GetString(b);
            return gb2312;
        }

        #region 转换类型方法
        public static object ConvertBaseObject(object obj)
        {
            return ConvertBaseObject(obj.ToString());
        }

        /// <summary>
        /// 将字符串转换成 Number,Bool Or string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static object ConvertBaseObject(string content)
        {
            if (IsNumber(content))
                return ConvertNumber(content);

            if (IsBool(content))
                return ConvertBool(content);

            return content;
        }

        public static object ConvertNumber(string content)
        {
            //byte
            byte bNum;
            if (byte.TryParse(content, out bNum))
                return bNum;

            //sbyte
            sbyte sbNum;
            if (sbyte.TryParse(content, out sbNum))
                return sbNum;

            //ushort
            ushort usNum;
            if (ushort.TryParse(content, out usNum))
                return usNum;

            //short
            short sNum;
            if (short.TryParse(content, out sNum))
                return sNum;

            //uint
            uint uiNum;
            if (uint.TryParse(content, out uiNum))
                return uiNum;

            //int
            int iNum;
            if (int.TryParse(content, out iNum))
                return iNum;

            //ulong
            ulong ulNum;
            if (ulong.TryParse(content, out ulNum))
                return ulNum;

            //long
            long lNum;
            if (long.TryParse(content, out lNum))
                return lNum;

            //float
            float fNum;
            if (float.TryParse(content, ConfigConstants.NUMBER_STYPLES, CultureInfo.CurrentCulture, out fNum))
                return fNum;

            //double
            double dNum;
            if (double.TryParse(content, ConfigConstants.NUMBER_STYPLES, CultureInfo.CurrentCulture, out dNum))
                return dNum;

            return content;
        }
        /// <summary>
        /// 转换Bool
        /// </summary>
        /// <returns></returns>
        public static bool ConvertBool(string content)
        {
            return content == "true" || content == "True" || content == "TRUE";
        }
        #endregion

        #region 判断类型方法
        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumber(string content)
        {
            double retNum;
            bool isNum = Double.TryParse(content, ConfigConstants.NUMBER_STYPLES, CultureInfo.CurrentCulture, out retNum);
            return isNum;
        }

        public static bool IsNumber(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断字符串是否是Bool
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsBool(string content)
        {
            return content == "true" || content == "false" || content == "True" || content == "False" || content == "TRUE" || content == "FALSE";
        }

        /// <summary>
        /// ObjectDictionary是否相同
        /// </summary>
        /// <returns></returns>
        public static bool IsSameObjectDictionary(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            int al = a.Count;
            int bl = b.Count;
            if (al != bl)
                return false;

            foreach (string akey in a.Keys)
            {
                if (!b.ContainsKey(akey))
                    return false;

                if (!IsSameObject(a[akey], b[akey]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 判断ObjectArray是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsSameObjectArray(object[] a, object[] b)
        {
            return a[0].GetType() == b[0].GetType();
            /*
            int al = a.Length;
            int bl = b.Length;
            if (al != bl)
                return false;

            for (int i = 0; i < al; i++)
            {
                if (!IsSameObject(a[i], b[i]))
                    return false;
            }

            return true;
             */
        }

        public static bool IsSameObject(object a, object b)
        {
            Type atype = a.GetType();
            Type btype = b.GetType();
            if (atype != btype)
                return false;

            if (atype == ConfigConstants.OBJECT_DICTIONARY_TYPE)
            {
                return IsSameObjectDictionary((Dictionary<string, object>)a, (Dictionary<string, object>)b);
            }
            else if (atype == ConfigConstants.OBJECT_ARRAY_TYPE)
            {
                return IsSameObjectArray((object[])a, (object[])b);
            }
            return true;
        }
        #endregion
    
        /// <summary>
        /// 在数组中找到最小的数字类型
        /// </summary>
        /// <returns></returns>
        public static Type FindMinimalNumberType(IEnumerable<object> array)
        {
            if (FindParseSuccess<byte>(array, byte.TryParse))
                return typeof(byte);

            if (FindParseSuccess<sbyte>(array, sbyte.TryParse))
                return typeof(sbyte);

            if (FindParseSuccess<ushort>(array, ushort.TryParse))
                return typeof(ushort);

            if (FindParseSuccess<short>(array, short.TryParse))
                return typeof(short);

            if (FindParseSuccess<uint>(array, uint.TryParse))
                return typeof(uint);

            if (FindParseSuccess<int>(array, int.TryParse))
                return typeof(int);

            if (FindParseSuccess<float>(array, float.TryParse))
                return typeof(float);

            return typeof(double);
        }

        public delegate bool TryParseHandler<T>(string value, out T result);
        public static bool FindParseSuccess<T>(IEnumerable<object> array, TryParseHandler<T> tryParse)
        {
            foreach (object item in array)
            {
                T v;
                if (!tryParse(item.ToString(), out v))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
