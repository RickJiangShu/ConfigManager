/*
 * Author:  Rick
 * Create:  2017/8/10 11:08:44
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ConfigManagerEditor
{
    /// <summary>
    /// ConfigManager 配置文件
    /// </summary>
    [System.Serializable]
    public class ConfigSetting
    {
        #region 单例与磁盘加载
        /// <summary>
        /// 磁盘中的命名
        /// </summary>
        private const string SETTING_DISK_NAME = "ConfigSetting.json";

        /// <summary>
        /// 缓存磁盘路径（默认在Assets/下）
        /// </summary>
        private static string settingDiskPath;

        /// <summary>
        /// 单例引用
        /// </summary>
        private static ConfigSetting setting;
        public static ConfigSetting ins
        {
            get
            {
                if (setting == null)
                    Load();
                return setting;
            }
        }

        private static void Load()
        {
            string content;
            string path;
            
            //如果在Assets/目录下找到配置文件
            if (TryGetDiskCache(out content, out path))
            {
                setting = JsonUtility.FromJson<ConfigSetting>(content);
                settingDiskPath = path;
            }
            else
            {
                setting = new ConfigSetting();
                settingDiskPath = "Assets/" + SETTING_DISK_NAME;
            }
        }
        public static void Save()
        {
            string json = JsonUtility.ToJson(setting, true);
            ConfigTools.WriteFile(settingDiskPath, json);
        }

        /// <summary>
        /// 获取磁盘中的缓存
        /// </summary>
        /// <returns></returns>
        private static bool TryGetDiskCache(out string content, out string path)
        {
            DirectoryInfo assetFolder = new DirectoryInfo(Application.dataPath);
            FileInfo[] files = assetFolder.GetFiles(SETTING_DISK_NAME, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                StreamReader stream = files[0].OpenText();
                content = stream.ReadToEnd();
                path = files[0].FullName;
                stream.Close();
                return true;
            }
            content = "";
            path = "";
            return false;
        }
        #endregion

        public string sourceFolder = "Assets";
        public string configOutputFolder = "Assets/Output";
        public string assetOutputFolder = "Assets/Resources";

        public bool txtEnabled = true;
        public bool csvEnabled = true;
        public bool jsonEnabled = true;
        public bool xmlEnabled = true;
        //  public bool xlsEnabled = true;
        public bool xlsxEnabled = true;

        public string serializerOutputFolder { get { return configOutputFolder + "/Serializer"; } }
    }
}
