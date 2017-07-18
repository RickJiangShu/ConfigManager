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

            //Source Type
            EditorGUILayout.Space();
            GUILayout.Label("Source Type", EditorStyles.boldLabel);

            cache.txtEnabled = EditorGUILayout.Toggle("*.txt", cache.txtEnabled);
            cache.csvEnabled = EditorGUILayout.Toggle("*.csv", cache.csvEnabled);
            cache.jsonEnabled = EditorGUILayout.Toggle("*.json", cache.jsonEnabled);
            cache.xmlEnabled = EditorGUILayout.Toggle("*.xml", cache.xmlEnabled);
            cache.xlEnabled = EditorGUILayout.Toggle("*.xls & *.xlsx", cache.xlEnabled);

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

            //获取源
            List<SheetSource> sheets;//表格
            List<JSONSource> jsons;//JSON

            GetSources(out sheets,out jsons);


            SheetGenerator.Generate(sheets, cache.configOutputFolder);//生产Configs
            //生成JSONs


            //生产SerializableSet
            SerializableSetGenerator.Generate(sheets, cache.configOutputFolder);

            //生产Deserializer
            DeserializerGenerator.Generate(sheets, cache.configOutputFolder);

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
        private void GetSources(out List<SheetSource> sheets,out List<JSONSource> jsons)
        {
            //获取所有配置文件
            DirectoryInfo directory = new DirectoryInfo(cache.sourceFolder);
            FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);


            //源
            sheets = new List<SheetSource>();
            jsons = new List<JSONSource>();

            for (int i = 0, l = files.Length; i < l; i++)
            {
                FileInfo file = files[i];
                SourceType type;
                if (!TypeEnabled(file.Extension,out type))
                    continue;

                string content;
                byte[] bytes;
                ConfigTools.ReadFile(file.FullName, out bytes);
                ConfigTools.DetectTextEncoding(bytes, out content);//转换不同的编码格式

                switch(type)
                {
                    case SourceType.Sheet:
                        SheetSource source = SheetParser.Parse(content, file.Name);
                        if (source != null) sheets.Add(source);
                        break;
                    case SourceType.JSON:
                        JSONSource json = JSONParser.Parse(content, file.Name);
                        if (json != null) jsons.Add(json);
                        break;
                }
            }
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
            List<SheetSource> sheets;
            List<JSONSource> jsons;
            GetSources(out sheets, out jsons);

            //通过反射序列化
            UnityEngine.Object set = (UnityEngine.Object)Serializer.Serialize(sheets);
            string o = cache.assetOutputFolder + "/" + assetName;
            AssetDatabase.CreateAsset(set, o);

            Debug.Log("序列化完成！");
        }

        /// <summary>
        /// 检测类型是否启用
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private bool TypeEnabled(string extension,out SourceType type)
        {
            type = SourceType.Sheet;
            switch(extension)
            {
                case ".txt":
                    return cache.txtEnabled;
                case ".csv":
                    return cache.csvEnabled;
                case ".json":
                    type = SourceType.JSON;
                    return cache.jsonEnabled;
                case ".xml":
                    type = SourceType.XML;
                    return cache.xmlEnabled;
                case ".xls":
                case ".xlsx":
                    return cache.xlEnabled;
            }
            return false;
        }
    }

    [System.Serializable]
    public class Cache
    {
        public string sourceFolder = "Assets/Config";
        public string configOutputFolder = "Assets/Scripts/Config";
        public string assetOutputFolder = "Assets/Resources";

        public bool txtEnabled = true;
        public bool csvEnabled = true;
        public bool jsonEnabled = true;
        public bool xmlEnabled = true;
        public bool xlEnabled = true;//*.xls & *.xlsx
    }
}

