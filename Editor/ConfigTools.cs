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
        public static bool IsArrayType(string sourceType,out string sourceBType)
        {
            int idx = sourceType.LastIndexOf('[');
            if (idx != -1)
                sourceBType = SourceBType2BaseBType(sourceType.Substring(0, idx));
            else
                sourceBType = sourceType;
            return idx != -1;
        }

        #region 将SourceType转换为ConfigType
        /// <summary>
        /// 将类型字符串转换为C#基础类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SourceType2ConfigType(string sourceType)
        {
            string sourceBType;
            if (IsArrayType(sourceType, out sourceBType))
            {
                return sourceBType + "[]";
            }
            return SourceBType2BaseBType(sourceType);
        }

        /// <summary>
        /// 将SourceType转换为C#基础类型
        /// </summary>
        /// <param name="sourceBType"></param>
        /// <returns></returns>
        private static string SourceBType2BaseBType(string sourceBType)
        {
            string baseType;
            switch (sourceBType)
            {
                case "bool":
                    baseType = "bool";
                    break;
                case "uint8":
                    baseType = "byte";
                    break;
                case "uint16":
                    baseType = "ushort";
                    break;
                case "uint":
                case "uint32":
                    baseType = "uint";
                    break;
                case "int8":
                    baseType = "sbyte";
                    break;
                case "int16":
                    baseType = "short";
                    break;
                case "int32":
                case "int":
                    baseType = "int";
                    break;
                case "long":
                    baseType = "long";
                    break;
                case "ulong":
                    baseType = "ulong";
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
                    baseType = sourceBType;
                    break;
            }
            return baseType;
        }
        #endregion

        #region 将SourceType转换为ObjectType
        private static Type SourceType2Type(string sourceType)
        {
            switch (sourceType)
            {
                case "bool":
                    return typeof(bool);
                case "uint8":
                    return typeof(byte);
                case "uint16":
                    return typeof(ushort);
                case "uint":
                case "uint32":
                    return typeof(uint);
                case "int8":
                    return typeof(sbyte);
                case "int16":
                    return typeof(short);
                case "int32":
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
            string baseType;
            if (IsArrayType(sourceType,out baseType))
            {
                return SourceValue2ArrayValue(sourceType, sourceValue, baseType);
            }
            else
            {
                //返回独值
                return SourceValue2BaseValue(sourceType, sourceValue);
            }
        }

        private static object SourceValue2BaseValue(string sourceType,string sourceValue)
        {
            switch (sourceType)
            {
                case "bool":
                    return sourceValue != "0" && sourceValue != "false" && sourceValue != "False" && sourceValue != "FALSE";
                   // return bool.Parse(sourceValue);
                case "uint8":
                    return byte.Parse(sourceValue);
                case "uint16":
                    return ushort.Parse(sourceValue);
                case "uint":
                case "uint32":
                    return uint.Parse(sourceValue);
                case "int8":
                    return sbyte.Parse(sourceValue);
                case "int16":
                    return short.Parse(sourceValue);
                case "int32":
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
        public static Array SourceValue2ArrayValue(string sourceType,string sourceValue,string baseType)
        {
            //解析数组
            if (string.IsNullOrEmpty(sourceValue) || sourceValue == "0") return null;

            string[] values = sourceValue.Split(',');

            Type type = SourceType2Type(baseType);
            Array array = Array.CreateInstance(type,values.Length);
            for(int i = 0,l = array.Length;i<l;i++)
            {
                object value = SourceValue2BaseValue(baseType, values[i]);
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

            string[] lines = Regex.Split(config, lf);
            string[] firstLine = Regex.Split(lines[0], sv);
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
            File.WriteAllText(path, content);
        }
    }
}
