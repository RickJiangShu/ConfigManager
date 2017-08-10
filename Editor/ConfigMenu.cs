/*
 * Author:  Rick
 * Create:  7/10/2017 9:35:30 PM
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    /// <summary>
    /// ConfigMenu
    /// </summary>
    public class ConfigMenu : ScriptableObject
    {
        [MenuItem("Assets/Config Manager/Set to Source Path")]
        static void Set2SourcePath()
        {
            ConfigSetting.ins.sourceFolder = GetSelectedPathOrFallback();
            ConfigSetting.Save();
            ConfigWindow.Get().Repaint();
        }

        [MenuItem("Assets/Config Manager/Set to Config Output")]
        static void Set2ConfigOutput()
        {
            ConfigSetting.ins.configOutputFolder = GetSelectedPathOrFallback();
            ConfigSetting.Save();
            ConfigWindow.Get().Repaint();
        }

        [MenuItem("Assets/Config Manager/Set to Asset Output")]
        static void Set2AssetOutput()
        {
            ConfigSetting.ins.assetOutputFolder = GetSelectedPathOrFallback();
            ConfigSetting.Save();
            ConfigWindow.Get().Repaint();
        }


        /// <summary>
        /// Fork:https://gist.github.com/allanolivei/9260107
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
    }
}
