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
    using System.Text.RegularExpressions;
    using OfficeOpenXml;

    /// <summary>
    /// ConfigManager窗口
    /// </summary>
    public class ConfigWindow : EditorWindow
    {
        private const string CACHE_DISK_NAME = "ConfigManagerCache.json";
        private const string ASSET_NAME = "SerializableSet.asset";

        private static bool justRecompiled;

        public static event Action serializeCompleted;//序列化完成

        static ConfigWindow()
        {
            justRecompiled = true;
        }

        [MenuItem("Window/Config Manager")]
        public static ConfigWindow Get()
        {
            return EditorWindow.GetWindow<ConfigWindow>("Config");
        }

        /// <summary>
        /// 缓存数据
        /// </summary>
        public Cache cache;

        /// <summary>
        /// 缓存磁盘路径
        /// </summary>
        private string cacheDiskPath;

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
            GUILayout.Label("Original Type", EditorStyles.boldLabel);

            cache.txtEnabled = EditorGUILayout.Toggle("*.txt", cache.txtEnabled);
            cache.csvEnabled = EditorGUILayout.Toggle("*.csv", cache.csvEnabled);
         //   cache.jsonEnabled = EditorGUILayout.Toggle("*.json", cache.jsonEnabled);
         //   cache.xmlEnabled = EditorGUILayout.Toggle("*.xml", cache.xmlEnabled);
            cache.xlsxEnabled = EditorGUILayout.Toggle("*.xlsx", cache.xlsxEnabled);

            //Operation
            EditorGUILayout.Space();
            GUILayout.Label("Operation", EditorStyles.boldLabel);


            if (GUILayout.Button("Serialize"))
            {
                Serialize();
            }

            if (GUILayout.Button("Clear Output"))
            {
                if (EditorUtility.DisplayDialog("Clear Output",
                "Are you sure you want to clear " + cache.configOutputFolder + " and " + cache.assetOutputFolder + "/" + ASSET_NAME, 
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
            string assetPath = cache.assetOutputFolder + "/" + ASSET_NAME;
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
        /// 获取磁盘中的
        /// </summary>
        /// <returns></returns>
        private bool TryGetDiskCache(out string content,out string path)
        {
            DirectoryInfo assetFolder = new DirectoryInfo(Application.dataPath);
            FileInfo[] files = assetFolder.GetFiles(CACHE_DISK_NAME, SearchOption.AllDirectories);
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

        /// <summary>
        /// 加载缓存或初始化
        /// </summary>
        private void LoadCache()
        {
            string content;
            string path;
            if (TryGetDiskCache(out content,out path))
            {
                cache = JsonUtility.FromJson<Cache>(content);
                cacheDiskPath = path;
            }
            else
            {
                cache = new Cache();
                cacheDiskPath = "Assets/" + CACHE_DISK_NAME;
            }
        }

        /// <summary>
        /// 保存缓存
        /// </summary>
        public void SaveCache()
        {
            string json = JsonUtility.ToJson(cache, true);
            ConfigTools.WriteFile(cacheDiskPath, json);
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
                if (IsTemporaryFile(file.Name))//临时文件
                    continue;

                OriginalType type;
                if(!TypeEnabled(file.Extension,out type))
                    continue;

                //可以同时读流
                FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                ExcelWorksheet excelData = null;
                string content = "";

                //读取Excel
                if (type == OriginalType.Xlsx)
                {
                    ExcelPackage package = new ExcelPackage(fileStream);
                    excelData = package.Workbook.Worksheets[1];
                    
                    fileStream.Close();
                }
                //其他
                else
                {
                    //读Byte
                    byte[] bytes;
                    BinaryReader br = new BinaryReader(fileStream);
                    bytes = br.ReadBytes((int)fileStream.Length);
                    StreamReader renderer = new StreamReader(fileStream);
                    content = renderer.ReadToEnd();

                    ConfigTools.DetectTextEncoding(bytes, out content);//转换不同的编码格式

                    if (string.IsNullOrEmpty(content))
                    {
                        Debug.LogWarning(file.Name + "内容为空！");
                        continue;
                    }
                }

                switch(type)
                {
                    case OriginalType.Txt:
                    case OriginalType.Csv:
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
                    case OriginalType.Json:
                        try
                        {
                            StructSource st = JsonParser.Parse(content, file.Name);
                            structs.Add(st);
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(file.Name + "解析失败！请检查格式是否正确，如果格式正确请联系作者：https://github.com/RickJiangShu/ConfigManager/issues" + "\n" + e);
                        }
                        break;
                    case OriginalType.Xml:
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
                    case OriginalType.Xlsx:
                        try
                        {
                            SheetSource st = SheetParser.Parse(excelData, file.Name);
                            sheets.Add(st);
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
            string o = cache.assetOutputFolder + "/" + ASSET_NAME;

            if (File.Exists(o))
            {
                UnityEngine.Object old = AssetDatabase.LoadMainAssetAtPath(o);
                EditorUtility.CopySerialized(set, old);
            }
            else
            {
                AssetDatabase.CreateAsset(set, o);
            }

            Debug.Log("序列化完成！");

            if (serializeCompleted != null)
                serializeCompleted();

        }

        /// <summary>
        /// 检测类型是否启用
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private bool TypeEnabled(string extension,out OriginalType type)
        {
            switch(extension)
            {
                case ".txt":
                    type = OriginalType.Txt;
                    return cache.txtEnabled;
                case ".csv":
                    type = OriginalType.Csv;
                    return cache.csvEnabled;
                    /*
                case ".json":
                    type = OriginalType.Json;
                    return cache.jsonEnabled;
                case ".xml":
                    type = OriginalType.Xml;
                    return cache.xmlEnabled;
                     */
                case ".xlsx":
                    type = OriginalType.Xlsx;
                    return cache.xlsxEnabled;
            }
            type = OriginalType.Txt;
            return false;
        }

        /// <summary>
        /// like ~$Equip
        /// </summary>
        /// <returns></returns>
        private bool IsTemporaryFile(string fileName)
        {
            return Regex.Match(fileName, @"^\~\$").Success;
        }
    }

    [System.Serializable]
    public class Cache
    {
        public string sourceFolder = "Assets";
        public string configOutputFolder = "Assets/Output";
        public string assetOutputFolder = "Assets/Resources";

        public bool txtEnabled = true;
        public bool csvEnabled = true;
    //    public bool jsonEnabled = true;
     //   public bool xmlEnabled = true;
      //  public bool xlsEnabled = true;
        public bool xlsxEnabled = true;

        public string serializerOutputFolder { get { return configOutputFolder + "/Serializer"; } }
    }
}

