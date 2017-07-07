/*
 * Author:  Rick
 * Create:  2017/7/6 18:01:59
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// ConfigManager窗口
/// </summary>
public class ConfigWindow : EditorWindow
{
    private static string cacheKey = "ConfigManagerCache_";

    [MenuItem("Window/ConfigManager")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<ConfigWindow>("ConfigManager");
    }


    private string inputPath;
    private string outputPath;

    void Awake()
    {
        //取缓存
        inputPath = PlayerPrefs.GetString(cacheKey + "InputPath");
        outputPath = PlayerPrefs.GetString(cacheKey + "OutputPath");

        //设置缺省值
        if (string.IsNullOrEmpty(inputPath))
            inputPath = "Assets/Resources/Config";
        if (string.IsNullOrEmpty(outputPath))
            outputPath = "Assets/Resources/ConfigOutput";
    }


    void OnGUI()
    {
        //Base Settings
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        string inputText = EditorGUILayout.TextField("Input Path", inputPath);
        string outputText = EditorGUILayout.TextField("Output Path", outputPath);

        //设置路径并缓存
        if (inputText != inputPath)
        {
            inputPath = inputText;
            PlayerPrefs.SetString(cacheKey + "InputPath", inputPath);
        }
        if (outputText != outputPath)
        {
            outputPath = outputText;
            PlayerPrefs.SetString(cacheKey + "OutputPath", outputPath);
        }

        //Operation
        EditorGUILayout.Space();
        GUILayout.Label("Operation", EditorStyles.boldLabel);

        if (GUILayout.Button("Clear Output"))
        {
            ClearOutput();
        }

        if(GUILayout.Button("Output"))
        {
            Output();
        }
    }

    /// <summary>
    /// 清空输出目录
    /// </summary>
    private void ClearOutput()
    {
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 输出文件
    /// </summary>
    private void Output()
    {
        //检出输出目录
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        //读取模板文件
        string getterTempletePath = Application.dataPath + "/ConfigManager/GetterTemplete";
        string getterTemplete = File.ReadAllText(getterTempletePath);

        //获取所有配置文件
        DirectoryInfo directory = new DirectoryInfo(inputPath);
        FileInfo[] files = directory.GetFiles("*.txt", SearchOption.AllDirectories);

        for (int i = 0, l = files.Length; i < l; i++)
        {
            FileInfo file = files[i];
            StreamReader txtStream = file.OpenText();
            string FileName = file.Name.Replace(file.Extension, "");
            string ClassName = FileName + "Config";
            string outputFilePath = outputPath + "/" + ClassName + ".cs";//输出文件名
            string fileString = txtStream.ReadToEnd();
            string config = String2Config(fileString, getterTemplete, ClassName);//C#代码

            txtStream.Close();//关闭txt文件流

            //文件写入
            FileStream stream;
            if (!File.Exists(outputFilePath))
            {
                stream = File.Create(outputFilePath);
            }
            else
            {
                stream = File.OpenWrite(outputFilePath);
            }
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(config);
            writer.Close();
        }

        AssetDatabase.Refresh();
    }

    #region 字符转换

    /// <summary>
    /// 配置文本->配置代码
    /// </summary>
    /// <returns></returns>
    private static string String2Config(string str, string textTemplete, string ClassName)
    {
        string[][] configArray = ConfigUtils.ParseConfig(str);

        //属性声明
        string IdField = "";//ID或者索引的标识符
        string PropertiesDeclaration = "";
        string declarationTemplete = "public {0} {1};//{2}";
        string[] propertiesComments = configArray[0];
        string[] propertiesType = configArray[1];//属性的类型

        //数组类型
        propertiesType = Array.ConvertAll<string, string>(propertiesType, s => String2Type(s));//将Type标识符转换为C#对应的类型
        string[] propertiesId = configArray[2];//属性的标识符
        string IdType = propertiesType[0];

        for (int i = 0, l = propertiesId.Length; i < l; i++)
        {
            if (i == 0) IdField = propertiesId[i];
            PropertiesDeclaration += string.Format(declarationTemplete, propertiesType[i], propertiesId[i], propertiesComments[i]) + "\r\n\t";
        }

        //属性解析
        string PropertiesParse = "";
        string parseTemplete = "cfg.{0} = {1}(args[{2}]);";
        string parseTemplete_Array = "cfg.{0} = ConfigUtils.ParseArray<{1}>(args[{2}], {3});";//0 field 1 baseType 2 idx 3 ParseFunc
        for (int i = 0, l = propertiesId.Length; i < l; i++)
        {
            string baseType;
            if (IsArrayType(propertiesType[i], out baseType))
            {
                PropertiesParse += string.Format(parseTemplete_Array, propertiesId[i], baseType, i, BaseType2ParseFunc(baseType)) + "\r\n\t\t\t";
            }
            else
            {
                PropertiesParse += string.Format(parseTemplete, propertiesId[i], BaseType2ParseFunc(propertiesType[i]), i) + "\r\n\t\t\t";
            }
        }

        //写入模板文件
        string config = textTemplete;
        config = config.Replace("/*ClassName*/", ClassName);
        config = config.Replace("/*IdType*/", IdType);
        config = config.Replace("/*PropertiesDeclaration*/", PropertiesDeclaration);
        config = config.Replace("/*PropertiesParse*/", PropertiesParse);
        config = config.Replace("/*IdField*/", IdField);

        return config;
    }

    /// <summary>
    /// 将类型字符串转换为C#类型字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string String2Type(string str)
    {
        string baseType;
        if (IsArrayType(str, out baseType))
        {
            return String2BaseType(baseType) + "[]";
        }
        else
        {
            return String2BaseType(str);
        }
    }

    /// <summary>
    /// 将类型字符串转换为C#基础类型
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string String2BaseType(string str)
    {
        switch (str)
        {
            case "bool":
                return "bool";
            case "uint8":
                return "byte";
            case "uint16":
                return "ushort";
            case "uint":
            case "uint32":
                return "uint";
            case "int8":
                return "sbyte";
            case "int16":
                return "short";
            case "int32":
            case "int":
                return "int";
            case "long":
                return "long";
            case "ulong":
                return "ulong";
            case "float":
                return "float";
            case "double":
                return "double";
            case "string":
                return "string";
            default:
                return str;
        }
    }

    private static string BaseType2ParseFunc(string baseType)
    {
        if (baseType == "string")
            return "Convert.ToString";
        else
            return baseType + ".Parse";
    }

    /// <summary>
    /// 判断是否是数组类型
    /// </summary>
    /// <returns></returns>
    private static bool IsArrayType(string str)
    {
        int idx = str.LastIndexOf('[');
        return idx != -1;
    }
    private static bool IsArrayType(string str, out string baseType)
    {
        int idx = str.LastIndexOf('[');
        if (idx != -1)
        {
            baseType = str.Substring(0, idx);
        }
        else
        {
            baseType = "";
        }
        return idx != -1;
    }

    #endregion
}