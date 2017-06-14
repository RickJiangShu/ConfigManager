using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ConfigManager设置
/// </summary>
public class ConfigManagerSettings
{
    public static ConfigLoaderMode Mode = ConfigLoaderMode.Resources;//配置加载模式

    public static string FilesURL = Application.dataPath + "/Resources/Config";//配置文件所在路径
    public static string OutputURL = Application.dataPath + "/Resources/ConfigOutput";//Getter *.cs文件输出目路径

    public static string AssetBundleURL = Application.streamingAssetsPath + "/config.ab";//配置包路径
}

public enum ConfigLoaderMode
{
    Resources,//从Resources里加载
    AssetBundle,//从AssetBundle加载
}
