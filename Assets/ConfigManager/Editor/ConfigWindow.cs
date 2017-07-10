/*
 * Author:  Rick
 * Create:  2017/7/6 18:01:59
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;
    using System.Collections.Generic;

    /// <summary>
    /// ConfigManager窗口
    /// </summary>
    public class ConfigWindow : EditorWindow
    {
        private static string cacheKey = "ConfigManagerCache_";

        private static bool justRecompiled;
        static ConfigWindow()
        {
            justRecompiled = true;
        }

        [MenuItem("Window/ConfigManager")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<ConfigWindow>("ConfigManager");
        }

        //
        private string[] configTypeOptions = new string[] { "txt(tsv)", "csv" };
        private string[] lfOptions = new string[] { "CR LF", "LF" };

        /// <summary>
        /// 对应ConfigType的分隔符
        /// </summary>
        private string[] separatedValues = new string[] { "\t", "," };
        /// <summary>
        /// 对应lfOptions的换行符
        /// </summary>
        private string[] lineFeed = new string[] { "\r\n", "\n" };

        //
        private string sourceFolder;
        private string outputFolder;
        private int configTypeIndex;
        private int lfIndex;

        void Awake()
        {
            LoadCacheOrInit();
        }


        void OnGUI()
        {
            //Base Settings
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            sourceFolder = EditorGUILayout.TextField("Source Folder", sourceFolder);
            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

            //Config Type
            configTypeIndex = EditorGUILayout.Popup("Config Type", configTypeIndex, configTypeOptions);

            //LF
            lfIndex = EditorGUILayout.Popup("End of Line", lfIndex, lfOptions);

            //Operation
            EditorGUILayout.Space();
            GUILayout.Label("Operation", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear Output"))
            {
                ClearOutput();
            }

            if (GUILayout.Button("Output"))
            {
                Output();
            }


            //缓存设置
            if (GUI.changed)
            {
                SaveCache();
            }
        }

        /// <summary>
        /// 清空输出目录
        /// </summary>
        private void ClearOutput()
        {
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
                File.Delete(outputFolder + ".meta");
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 输出文件
        /// </summary>
        private void Output()
        {
            //检出输出目录
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            //读取模板文件
            string getterTempletePath = Application.dataPath + "/ConfigManager/GetterTemplete";
            string getterTemplete = File.ReadAllText(getterTempletePath);

            //源
            List<Source> sources = GetSources();

            //生产Configs
            ConfigGenerator.Generate(sources, outputFolder);

            //生产SerializableSet
            SerializableSetGenerator.Generate(sources, outputFolder);

            //生产Deserializer


            //刷新
            AssetDatabase.Refresh();

            //等待序列化
            waitingForSerialize = true;
        }


        /// <summary>
        /// 是否正在等待序列化（新生成脚本需要编译）
        /// </summary>
        private bool waitingForSerialize = false;

        void Update()
        {
            if (justRecompiled && waitingForSerialize)
            {
                UnityEngine.Debug.Log("配置开始序列化！");

                waitingForSerialize = false;

                //无法缓存只能重新获取
                List<Source> sources = GetSources();

                //通过反射序列化
                UnityEngine.Object set = (UnityEngine.Object)Serializer.Serialize(sources);
                string o = outputFolder + "/SerializableSet.asset";
                AssetDatabase.CreateAsset(set, o);
            }
            justRecompiled = false;
        }

        /// <summary>
        /// 加载缓存或初始化
        /// </summary>
        private void LoadCacheOrInit()
        {
            //取缓存
            sourceFolder = PlayerPrefs.GetString(cacheKey + "SourceFolder");
            outputFolder = PlayerPrefs.GetString(cacheKey + "OutputFolder");
            configTypeIndex = PlayerPrefs.GetInt(cacheKey + "ConfigTypeIndex");
            lfIndex = PlayerPrefs.GetInt(cacheKey + "LFIndex");

            //设置缺省值
            if (string.IsNullOrEmpty(sourceFolder))
                sourceFolder = "Assets/Resources/Config";
            if (string.IsNullOrEmpty(outputFolder))
                outputFolder = "Assets/Resources/ConfigOutput";
        }

        /// <summary>
        /// 保存缓存
        /// </summary>
        private void SaveCache()
        {
            PlayerPrefs.SetString(cacheKey + "InputPath", sourceFolder);
            PlayerPrefs.SetString(cacheKey + "OutputPath", outputFolder);
            PlayerPrefs.SetInt(cacheKey + "ConfigTypeIndex", configTypeIndex);
            PlayerPrefs.SetInt(cacheKey + "LFIndex", lfIndex);
        }


        /// <summary>
        /// 获取所有源
        /// </summary>
        /// <returns></returns>
        private List<Source> GetSources()
        {
            //获取所有配置文件
            DirectoryInfo directory = new DirectoryInfo(sourceFolder);
            FileInfo[] files = directory.GetFiles("*.txt", SearchOption.AllDirectories);

            //设置
            string sv = separatedValues[configTypeIndex];
            string lf = lineFeed[lfIndex];

            //源
            List<Source> sources = new List<Source>();
            for (int i = 0, l = files.Length; i < l; i++)
            {
                Source source = new Source();
                FileInfo file = files[i];
                StreamReader contentStream = file.OpenText();

                source.content = contentStream.ReadToEnd();//内容
                source.sourceName = file.Name.Replace(file.Extension, "");//文件名
                source.configName = source.sourceName + "Config";//类名
                source.matrix = ConfigTools.Content2Matrix(source.content, sv, lf, out source.row, out source.column);

                contentStream.Close();//关闭txt文件流
                sources.Add(source);
            }
            return sources;
        }
    }
}
