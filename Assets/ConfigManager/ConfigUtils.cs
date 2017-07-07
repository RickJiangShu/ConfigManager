using System;
using System.Collections;
using System.Text.RegularExpressions;

public class ConfigUtils
{
    /**
     * 将配置文件解析为 [y,x] 的二维数组
     */
    public static string[][] ParseConfig(string cfg)
    {
        cfg = cfg.Trim();//清空末尾的空白
        string[] line = Regex.Split(cfg, "\r\n");
        int start = 0;
        int end = line.Length;
        int len = end - start;
        string[][] configArray = new string[len][];
        for (int y = start, idx = 0; y < end; y++, idx++)
        {
            string[] args = line[y].Split('\t');
            int argsLen = args.Length;
            configArray[idx] = new string[argsLen];
            for (int x = 0; x < argsLen; x++)
            {
                configArray[idx][x] = args[x];
            }
        }
        return configArray;
    }

    /// <summary>
    /// 将字符串解析成对应类型数组
    /// </summary>
    /// <returns></returns>
    public static T[] ParseArray<T>(string cfg, Func<string, T> ParseFunc)
    {

        if (cfg.Contains(","))
        {
            return Array.ConvertAll<string, T>(cfg.Split(','), s => ParseFunc(s));
        }
        else
        {
            return new T[1] { ParseFunc(cfg) };
        }
    }
}