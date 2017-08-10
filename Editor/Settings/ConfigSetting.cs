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
    public class ConfigSettings
    {
        #region 单例与磁盘加载
        /// <summary>
        /// 缓存磁盘路径（默认在Assets/下）
        /// </summary>
        private static string SETTING_PATH = "Assets/ConfigSetting.json";

        /// <summary>
        /// 单例引用
        /// </summary>
        private static ConfigSettings setting;
        public static ConfigSettings ins
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
            
            //如果在Assets/目录下找到配置文件
            if (File.Exists(SETTING_PATH))
            {
                //FileStream stream = File.Open(SETTING_PATH, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(SETTING_PATH);
                content = reader.ReadToEnd();
                reader.Close();

                setting = JsonUtility.FromJson<ConfigSettings>(content);
            }
            else
            {
                setting = new ConfigSettings();
                Save();
            }
        }

        public static void Save()
        {
            string json = JsonUtility.ToJson(setting, true);
            ConfigTools.WriteFile(SETTING_PATH, json);
            UnityEditor.AssetDatabase.Refresh();
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
