/*
 * Author:  Rick
 * Create:  2017/7/7 15:45:53
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Config解析器，负责将txt解析成*.cs
/// </summary>
public class ConfigParser
{
    private const string fieldDeclaration = "public {0} {1};//{2}";//属性声明模板
    private const string propertyParse = "item.{0} = {1}(args[{2}]);";//属性解析

    /// <summary>
    /// 解析Getter
    /// </summary>
    /// <returns></returns>
    public static string ParseGetter(string input, string textTemplete, string ClassName, string sv, string lf)
    {
        int row; int col;
        string[,] matrix = Config2Matrix(input, sv, lf, out row, out col);

        //属性声明
        string propertiesDeclaration = "";
        for (int x = 0; x < col; x++)
        {
            string comment = matrix[0,x];
            string field = matrix[1, x];
            string csType = ConfigType2CSharpType(matrix[2, x]);
            string declaration = string.Format(fieldDeclaration, csType, field, comment);
            propertiesDeclaration += declaration + lf;
        }

        //属性解析过程
        string propertiesParseProcess;

        
    }

    /// <summary>
    /// 解析成Manager
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ParseManager(string input)
    {
        return "";
    }

    /// <summary>
    /// 将类型字符串转换为C#基础类型
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string ConfigType2CSharpType(string configType)
    {
        string csharpType;
        switch (configType)
        {
            case "bool":
                csharpType = "bool";
                break;
            case "uint8":
                csharpType = "byte";
                break;
            case "uint16":
                csharpType = "ushort";
                break;
            case "uint":
            case "uint32":
                csharpType = "uint";
                break;
            case "int8":
                csharpType = "sbyte";
                break;
            case "int16":
                csharpType = "short";
                break;
            case "int32":
            case "int":
                csharpType = "int";
                break;
            case "long":
                csharpType = "long";
                break;
            case "ulong":
                csharpType = "ulong";
                break;
            case "float":
                csharpType = "float";
                break;
            case "double":
                csharpType = "double";
                break;
            case "string":
                csharpType = "string";
                break;
            default:
                csharpType = configType;
                break;
        }
        if (IsArrayType(configType))
        {
            csharpType += "[]";
        }
        return csharpType;
    }

    /// <summary>
    /// 判断是否是数组类型
    /// </summary>
    /// <returns></returns>
    private static bool IsArrayType(string str)
    {
        int idx = str.LastIndexOf('[');
        return idx != -1;
    }

    /// <summary>
    /// 从配置转换成矩阵数组（0行1列）
    /// </summary>
    /// <param name="config">配置文件</param>
    /// <param name="sv">分隔符 Separated Values</param>
    /// <param name="lf">换行符 Line Feed</param>
    /// <returns></returns>
    private static string[,] Config2Matrix(string config, string sv, string lf,out int row,out int col)
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
                matrix[i, j] = line[k];
            }
        }
        return matrix;
    }

    /// <summary>
    /// 将配置的数组转换为C#数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cValue"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    private static T[] ConfigValue2Array<T>(string cValue, Func<string, T> converter)
    {
        if (string.IsNullOrEmpty(cValue) || cValue == "0") return null;

        if (cValue.Contains(","))
        {
            return Array.ConvertAll<string, T>(cValue.Split(','), s => converter(s));
        }
        else
        {
            return new T[1] { converter(cValue) };
        }
    }
}
