using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;
/// <summary>
/// 配置工具
/// </summary>
public class ConfigEditor : Editor
{
    
    [MenuItem("Config/Clear Output")]
    static void ClearOutput()
    {
        FileUtils.DeleteFolder(ConfigManagerSettings.OutputURL);

        //写入一个空的Loader
        string loaderTempletePath = Application.dataPath + "/ConfigManager/LoaderTemplete";
        string loaderTemplete = File.ReadAllText(loaderTempletePath);
        string loaderCode = loaderTemplete;

        //文件写入
        string loaderPath = ConfigManagerSettings.OutputURL + "/ConfigLoader.cs";
        FileUtils.CreateFile(loaderPath);
        FileUtils.Write(loaderPath, loaderCode);
    }
   

    /// <summary>
    /// 输出配置文件和ConfigLoader
    /// </summary>
    [MenuItem("Config/Output")]
    static void Output()
    {
        //读取模板文件
        string getterTempletePath = Application.dataPath + "/ConfigManager/GetterTemplete";
        string getterTemplete = File.ReadAllText(getterTempletePath);

        string loaderTempletePath = Application.dataPath + "/ConfigManager/LoaderTemplete";
        string loaderTemplete = File.ReadAllText(loaderTempletePath);
        string LoaderParse = "";//加载解析相关代码

        switch (ConfigManagerSettings.Mode)
        {
            case ConfigLoaderMode.AssetBundle:
                LoaderParse += LoadAssetBundleTemplete;
                break;
            case ConfigLoaderMode.Resources:
                LoaderParse += RelativeURLTemplete;
                break;
        }
            

        //获取所有配置文件
        DirectoryInfo directory = new DirectoryInfo(ConfigManagerSettings.FilesURL);
        FileInfo[] files = directory.GetFiles("*.txt", SearchOption.AllDirectories);

        for (int i = 0, l = files.Length; i < l; i++)
        {
            FileInfo file = files[i];
            StreamReader stream = file.OpenText();
            string FileName = file.Name.Replace(file.Extension, "");
            string ClassName = FileName + "Config";
            string outputFileName = ConfigManagerSettings.OutputURL + "/" + ClassName + ".cs";//输出文件名
            string fileString = stream.ReadToEnd();
            string config = String2Config(fileString, getterTemplete, ClassName);//C#代码
            LoaderParse += String2Loader(ClassName, FileName) + "\r\n";//Loader解析代码

            stream.Close();//关闭txt文件流

            //文件写入
            FileUtils.CreateFile(outputFileName);
            FileUtils.Write(outputFileName, config);
        }

        //写入Loader
        string loaderCode = loaderTemplete.Replace("/*LoaderParse*/", LoaderParse);

        //文件写入
        string loaderPath = ConfigManagerSettings.OutputURL + "/ConfigLoader.cs";
        FileUtils.CreateFile(loaderPath);
        FileUtils.Write(loaderPath, loaderCode);
    }

    /// <summary>
    /// 配置文本->配置代码
    /// </summary>
    /// <returns></returns>
    private static string String2Config(string str,string textTemplete,string ClassName)
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
        for(int i = 0,l = propertiesId.Length;i<l;i++)
        {
            string baseType;
            if (IsArrayType(propertiesType[i],out baseType))
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


    //加载AssetBundle模板
    private static string LoadAssetBundleTemplete = "\t\tAssetBundle bundle = AssetBundle.LoadFromFile(ConfigManagerSettings.AssetBundleURL);\r\n\r\n";

    //解析Resources相对路径目录
    private static string RelativeURLTemplete = 
        "\t\tint startIndex = ConfigManagerSettings.FilesURL.LastIndexOf(\"Resources/\") + 10;\r\n" +
        "\t\tint length = ConfigManagerSettings.FilesURL.Length - startIndex;\r\n" +
        "\t\tstring relativeURL = ConfigManagerSettings.FilesURL.Substring(startIndex, length);\r\n\r\n";

    //AssetBundle加载解析模板
    private static string AssetBundleParseTemplete = "\t\tstring /*FileName*/Text = bundle.LoadAsset<TextAsset>(\"/*FileName*/\").text;\r\n" +
           "\t\t/*ClassName*/.Parse(/*FileName*/Text);\r\n";

    //Resources加载解析模板
    private static string ResroucesParseTemplete = "\t\tstring /*FileName*/Text = Resources.Load<TextAsset>(relativeURL + \"/\" + \"/*FileName*/\").text;\r\n" +
           "\t\t/*ClassName*/.Parse(/*FileName*/Text);\r\n";

    private static string String2Loader(string ClassName,string FileName)
    {
        switch (ConfigManagerSettings.Mode)
        {
            case ConfigLoaderMode.AssetBundle:
                return AssetBundleParseTemplete.Replace("/*ClassName*/", ClassName).Replace("/*FileName*/", FileName);
            case ConfigLoaderMode.Resources:
                return ResroucesParseTemplete.Replace("/*ClassName*/", ClassName).Replace("/*FileName*/", FileName);
            default:
                return ResroucesParseTemplete.Replace("/*ClassName*/", ClassName).Replace("/*FileName*/", FileName);
        }
    }

    /// <summary>
    /// 将类型字符串转换为C#类型字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string String2Type(string str)
    {
        string baseType;
        if (IsArrayType(str,out baseType))
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
    private static bool IsArrayType(string str,out string baseType)
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

    
}


#region FileUtils
internal class FileUtils
{
    /// 创建文件夹
    public static void CreateFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }
    }
    public static void DeleteFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
            //删除.meta
            DeleteFile(folderPath + ".meta");

            
            AssetDatabase.Refresh();
        }
    }

    public static void CreateFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            CreateFolder(filePath.Substring(0, filePath.LastIndexOf('/')));

            FileStream stream = File.Create(filePath);
            stream.Close();
        }
    }

    public static void DeleteFile(string filePath)
    {
     //   Debug.Log("DeleteFile：" + filePath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// 写入数据到对应文件
    public static void Write(string fileName, string contents)
    {
        TextWriter tw = new StreamWriter(fileName, false);
        tw.Write(contents);
        tw.Close();

        AssetDatabase.Refresh();
    }
}
#endregion