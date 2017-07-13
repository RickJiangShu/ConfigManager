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
    using System.Text;

    /// <summary>
    /// ConfigManager窗口
    /// </summary>
    public class ConfigWindow : EditorWindow
    {
        private const string cacheKey = "ConfigManagerCache";
        private const string assetName = "SerializableSet.asset";

        private static bool justRecompiled;

        static ConfigWindow()
        {
            justRecompiled = true;
        }

        [MenuItem("Window/Config Manager")]
        public static ConfigWindow Get()
        {
            return EditorWindow.GetWindow<ConfigWindow>("Config Manager");
        }

        //
        private string[] sourceTypeOptions = new string[] { "txt(tsv)", "csv" };
        private string[] lfOptions = new string[] { "as Source", "CR LF", "LF" };

        /// <summary>
        /// 对应ConfigType的分隔符
        /// </summary>
        private string[] separatedValues = new string[] { "\t", "," };
        /// <summary>
        /// 对应lfOptions的换行符
        /// </summary>
        private string[] lineFeed = new string[] { "\r\n", "\n" };

        /// <summary>
        /// 缓存数据
        /// </summary>
        public Cache cache;

        void Awake()
        {
            LoadCache();
        }


        public void OnGUI()
        {
            //Base Settings
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            cache.sourceFolder = EditorGUILayout.TextField("Source Folder", cache.sourceFolder);
            cache.configOutputFolder = EditorGUILayout.TextField("Config Output", cache.configOutputFolder);
            cache.assetOutputFolder = EditorGUILayout.TextField("Asset Output", cache.assetOutputFolder);

            //Operation
            EditorGUILayout.Space();
            GUILayout.Label("Operation", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear Output"))
            {
                if (EditorUtility.DisplayDialog("Clear Output",
                "Are you sure you want to clear " + cache.configOutputFolder + " and " + cache.assetOutputFolder + "/" + assetName, 
                "Yes", "No"))
                {
                    ClearOutput();
                }

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
            //Clear Config
            if (Directory.Exists(cache.configOutputFolder))
            {
                Directory.Delete(cache.configOutputFolder, true);
                File.Delete(cache.configOutputFolder + ".meta");

            }

            ////Clear Asset
            string assetPath = cache.assetOutputFolder + "/" + assetName;
            if(File.Exists(assetPath))
            {
                File.Delete(assetPath);
                File.Delete(assetPath + ".meta");
            }
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 输出文件
        /// </summary>
        private void Output()
        {
            //检出输出目录
            if (!Directory.Exists(cache.configOutputFolder))
                Directory.CreateDirectory(cache.configOutputFolder);

            //源
            List<Source> sources = GetSources();

            //生产Configs
            ConfigGenerator.Generate(sources, cache.configOutputFolder);

            //生产SerializableSet
            SerializableSetGenerator.Generate(sources, cache.configOutputFolder);

            //生产Deserializer
            DeserializerGenerator.Generate(sources, cache.configOutputFolder);

            //刷新
            AssetDatabase.Refresh();

            //等待序列化
            if (EditorApplication.isCompiling)
            {
                waitingForSerialize = true;
                Debug.Log("输出完成，正在等待Unity编译后序列化数据...");
            }
            else
            {
                Serialize();
            }
        }


        /// <summary>
        /// 是否正在等待序列化（新生成脚本需要编译）
        /// </summary>
        private bool waitingForSerialize = false;

        void Update()
        {
            if (justRecompiled && waitingForSerialize)
            {
                waitingForSerialize = false;
                Serialize();
            }
            justRecompiled = false;
        }

        /// <summary>
        /// 加载缓存或初始化
        /// </summary>
        private void LoadCache()
        {
            if (PlayerPrefs.HasKey(cacheKey))
            {
                cache = JsonUtility.FromJson<Cache>(PlayerPrefs.GetString(cacheKey));
            }
            else
            {
                cache = new Cache();
            }
        }

        /// <summary>
        /// 保存缓存
        /// </summary>
        public void SaveCache()
        {
            string json = JsonUtility.ToJson(cache);
            PlayerPrefs.SetString(cacheKey, json);
        }


        /// <summary>
        /// 获取所有源
        /// </summary>
        /// <returns></returns>
        private List<Source> GetSources()
        {
            //获取所有配置文件
            DirectoryInfo directory = new DirectoryInfo(cache.sourceFolder);

            FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);

            //可选选项


            string sv;
            string lf;

            //源
            List<Source> sources = new List<Source>();
            for (int i = 0, l = files.Length; i < l; i++)
            {
                FileInfo file = files[i];
                if (file.Extension != ".txt" && file.Extension != ".csv")
                    continue;

                Source source = new Source();

                string content;
                byte[] bytes;
                ConfigTools.ReadFile(file.FullName, out bytes);
                ConfigTools.DetectTextEncoding(bytes, out content);//转换不同的编码格式

                //判断换行符
                if (content.IndexOf("\r\n") != -1)
                    lf = "\r\n";
                else
                    lf = "\n";

                //判断分割符
                if (content.IndexOf("\t") != -1)
                    sv = "\t";
                else
                    sv = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";//Fork:https://stackoverflow.com/questions/6542996/how-to-split-csv-whose-columns-may-contain

                //写入源
                source.content = content;
                source.sourceName = file.Name.Replace(file.Extension, "");//文件名
                source.configName = source.sourceName + "Config";//类名
                source.matrix = ConfigTools.Content2Matrix(source.content, sv, lf, out source.row, out source.column);

                sources.Add(source);
            }
            return sources;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        private void Serialize()
        {
            //如果输出文件夹不存在，先创建
            if (!Directory.Exists(cache.assetOutputFolder))
            {
                Directory.CreateDirectory(cache.assetOutputFolder);
            }

            //无法缓存只能重新获取
            List<Source> sources = GetSources();

            //通过反射序列化
            UnityEngine.Object set = (UnityEngine.Object)Serializer.Serialize(sources);
            string o = cache.assetOutputFolder + "/" + assetName;
            AssetDatabase.CreateAsset(set, o);

            Debug.Log("序列化完成！");
        }

        
    }

    [System.Serializable]
    public class Cache
    {
        public string sourceFolder = "Assets";
        public string configOutputFolder = "Assets/Scripts/Config";
        public string assetOutputFolder = "Assets/Resources";
    }
}

