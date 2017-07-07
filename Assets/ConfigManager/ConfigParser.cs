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
    public static string ParseGetter(string input)
    {
        return "";
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
    /// <param name="config"></param>
    /// <returns></returns>
    private static string[,] Config2Matrix(string config,string )
    {
     //   config = config.Trim();//清空末尾的空白
        /*
        string[] lines = Regex.Split(config, "\r\n");
        string firstLine = lines[0].Split('\t');

        string[,] matrix = new string[lines.Length]
    
         */
        return null;
    }
}
