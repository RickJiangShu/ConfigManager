/*
 * Author:  Rick
 * Create:  2017/7/10 11:57:52
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System;
    using System.IO;
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
        public static string SourceType2CSharpType(string sourceType)
        {
            string sourceBType;
            if (IsArrayType(sourceType, out sourceBType))
            {
                return sourceBType + "[]";
            }
            return SourceBaseType2CSharpBaseType(sourceType);
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

        #region 将SourceType转换为SystemType
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
                    //去除CSV的""
                    if (!string.IsNullOrEmpty(sourceValue) && sourceValue[0] == '"')
                    {
                        sourceValue = sourceValue.Remove(0, 1);
                        sourceValue = sourceValue.Remove(sourceValue.Length - 1, 1);
                    }
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

            //去除CSV的""
            if (sourceValue[0] == '"')
            {
                if (sourceValue.Length <= 2)
                    return null;

                sourceValue = sourceValue.Remove(0, 1);
                sourceValue = sourceValue.Remove(sourceValue.Length - 1, 1);
            }

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
        /// 从配置转换成矩阵数组（0行1列）
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="sv">分隔符 Separated Values</param>
        /// <param name="lf">换行符 Line Feed</param>
        /// <returns></returns>
        public static string[,] Content2Matrix(string config, string sv, string lf, out int row, out int col)
        {
            config = config.Trim();//清空末尾的空白

            //分割
            string[] lines = Regex.Split(config, lf);
            string[] firstLine = Regex.Split(lines[0], sv, RegexOptions.Compiled);
            
            row = lines.Length;
            col = firstLine.Length;
            string[,] matrix = new string[row, col];
            //为第一行赋值
            for (int i = 0, l = firstLine.Length; i < l; i++)
            {
                matrix[0, i] = firstLine[i];
            }
            //为其他行赋值
            for (int i = 1, l = lines.Length; i < l; i++)
            {
                string[] line = Regex.Split(lines[i], sv);
                for (int j = 0, k = line.Length; j < k; j++)
                {
                    matrix[i, j] = line[j];
                }
            }
            return matrix;
        }


        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteFile(string path,string content)
        {
            //文件写入
            FileStream stream;
            if (!File.Exists(path))
            {
                stream = File.Create(path);
            }
            else
            {
                stream = File.Open(path, FileMode.Truncate);
            }

            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
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
        public static string ReadFile(string path,out byte[] bytes)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    bytes = br.ReadBytes((int)fileStream.Length);
                }
                using (var textReader = new StreamReader(fileStream))
                {
                    return textReader.ReadToEnd();
                }
            }
        }
    }
}
