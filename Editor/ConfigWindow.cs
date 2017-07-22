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
       //     cache.xlEnabled = EditorGUILayout.Toggle("*.xls & *.xlsx", cache.xlEnabled);

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
            {
                Directory.CreateDirectory(cache.configOutputFolder);
            }
            if (!Directory.Exists(cache.serializerOutputFolder))
            {
                Directory.CreateDirectory(cache.serializerOutputFolder);
            }

            //获取源
            List<SheetSource> sheets;//表格
            List<StructSource> structs;//结构

            GetSources(out sheets, out structs);

            if (sheets.Count == 0 && structs.Count == 0)
            {
                Debug.LogError(cache.sourceFolder + "没有找到任何文件！");
                return;
            }

            SheetGenerator.Generate(sheets, cache.configOutputFolder);//生产Configs
            StructGenerator.Generate(structs, cache.configOutputFolder);//生成Jsons
            
            //生产SerializableSet
            SerializableSetGenerator.Generate(sheets, structs, cache.serializerOutputFolder);

            //生产Deserializer
            DeserializerGenerator.Generate(sheets, structs, cache.serializerOutputFolder);

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
        private void GetSources(out List<SheetSource> sheets,out List<StructSource> structs)
        {
            //获取所有配置文件
            DirectoryInfo directory = new DirectoryInfo(cache.sourceFolder);
            FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);


            //源
            sheets = new List<SheetSource>();
            structs = new List<StructSource>();

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

                if (string.IsNullOrEmpty(content))
                {
                    Debug.LogWarning(file.Name + "内容为空！");
                    continue;
                }

                switch(type)
                {
                    case SourceType.Txt:
                    case SourceType.Csv:
                        try
                        {
                            SheetSource source = SheetParser.Parse(content, file.Name);
                            sheets.Add(source);
                        }
                        catch(Exception e)
                        {
                            UnityEngine.Debug.LogError(file.Name + "解析失败！请检查格式是否正确，如果格式正确请联系作者：https://github.com/RickJiangShu/ConfigManager/issues" + "\n" + e);
                        }
                        break;
                    case SourceType.Json: 
                        try
                        {
                            StructSource st= JsonParser.Parse(content, file.Name);
                            structs.Add(st);
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(file.Name + "解析失败！请检查格式是否正确，如果格式正确请联系作者：https://github.com/RickJiangShu/ConfigManager/issues" + "\n" + e);
                        }
                        break;
                    case SourceType.Xml:
                        try
                        {
                            StructSource st = XmlParser.Parse(content, file.Name);
                            structs.Add(st);
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(file.Name + "解析失败！请检查格式是否正确，如果格式正确请联系作者：https://github.com/RickJiangShu/ConfigManager/issues" + "\n" + e);
                        }
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
            List<StructSource> structs;
            GetSources(out sheets, out structs);

            //通过反射序列化
            UnityEngine.Object set = (UnityEngine.Object)Serializer.Serialize(sheets, structs);
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
            switch(extension)
            {
                case ".txt":
                    type = SourceType.Txt;
                    return cache.txtEnabled;
                case ".csv":
                    type = SourceType.Csv;
                    return cache.csvEnabled;
                case ".json":
                    type = SourceType.Json;
                    return cache.jsonEnabled;
                case ".xml":
                    type = SourceType.Xml;
                    return cache.xmlEnabled;
                case ".xls":
                case ".xlsx":
                    type = SourceType.Xlsx;
                    return cache.xlEnabled;
            }
            type = SourceType.Txt;
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

        public string serializerOutputFolder { get { return configOutputFolder + "/Serializer"; } }
    }
}

