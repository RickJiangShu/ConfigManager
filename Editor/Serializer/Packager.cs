/*
 * Author:  Rick
 * Create:  2017/8/10 10:48:04
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigManagerEditor
{
    /// <summary>
    /// 打包机（输入指定配置的路径，输出一个包）
    /// </summary>
    public class Packager
    {
        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="configPaths">选中的配置路径</param>
        public static void Pack(string packageName, List<string> configPaths)
        {
            for (int i = 0, c = configPaths.Count; i < c; i++)
            {
                string path = configPaths[i];
                string extension = Path.GetExtension(path);
            }
        }

        /// <summary>
        /// 检测类型是否启用
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static bool TypeEnabled(string extension, out OriginalType type)
        {
            switch (extension)
            {
                case ".txt":
                    type = OriginalType.Txt;
                    return ConfigSettings.ins.txtEnabled;
                case ".csv":
                    type = OriginalType.Csv;
                    return ConfigSettings.ins.csvEnabled;
                case ".json":
                    type = OriginalType.Json;
                    return ConfigSettings.ins.jsonEnabled;
                case ".xml":
                    type = OriginalType.Xml;
                    return ConfigSettings.ins.xmlEnabled;
                case ".xlsx":
                    type = OriginalType.Xlsx;
                    return ConfigSettings.ins.xlsxEnabled;
            }
            type = OriginalType.Txt;
            return false;
        }
    }
}
