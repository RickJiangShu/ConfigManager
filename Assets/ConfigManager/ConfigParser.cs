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
    /// <summary>
    /// 解析Getter
    /// </summary>
    /// <returns></returns>
    public static string ParseGetter(string input, string textTemplete, string ClassName, string sv, string lf)
    {
        string[,] matrix = Config2Matrix(input, sv, lf);

        //属性声明
        string IdField = "";//ID或者索引的标识符
        string PropertiesDeclaration = "";
        string declarationTemplete = "public {0} {1};//{2}";
 //       string[] propertiesComments = matrix[0];//属性注释
 //       string[] propertiesType = matrix[1];//属性的类型

        //数组类型
     //   propertiesType = Array.ConvertAll<string, string>(propertiesType, s => String2Type(s));//将Type标识符转换为C#对应的类型
   //     string[] propertiesId = configArray[2];//属性的标识符
    //    string IdType = propertiesType[0];

        //属性解析过程
        string propertiesParse = "";
        string parseTemplete = "cfg.{0} = {1}(args[{2}]);";
        string parseTemplete_Array = "cfg.{0} = ConfigUtils.ParseArray<{1}>(args[{2}], {3});";//0 field 1 baseType 2 idx 3 ParseFunc
        for (int i = 0, l = propertiesId.Length; i < l; i++)
        {
            string baseType;
            if (IsArrayType(propertiesType[i], out baseType))
            {
                propertiesParse += string.Format(parseTemplete_Array, propertiesId[i], baseType, i, BaseType2ParseFunc(baseType)) + "\r\n\t\t\t";
            }
            else
            {
                propertiesParse += string.Format(parseTemplete, propertiesId[i], BaseType2ParseFunc(propertiesType[i]), i) + "\r\n\t\t\t";
            }
        }
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
    /// 从配置转换成矩阵数组（0行1列）
    /// </summary>
    /// <param name="config">配置文件</param>
    /// <param name="sv">分隔符 Separated Values</param>
    /// <param name="lf">换行符 Line Feed</param>
    /// <returns></returns>
    private static string[,] Config2Matrix(string config, string sv, string lf)
    {
        config = config.Trim();//清空末尾的空白
        
        string[] lines = Regex.Split(config, lf);
        string[] firstLine = Regex.Split(lines[0], sv);
        int rowCount = lines.Length;
        int columnCount = firstLine.Length;
        string[,] matrix = new string[rowCount, columnCount];
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
    private static T[] CValue2Array<T>(string cValue, Func<string, T> converter)
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
