/*
 * Author:  Rick
 * Create:  2017/7/6 18:01:59
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using UnityEngine;
using UnityEditor;

/// <summary>
/// ConfigManager窗口
/// </summary>
public class ConfigWindow : ScriptableObject
{
    [MenuItem("Windows/ConfigManager")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }
}