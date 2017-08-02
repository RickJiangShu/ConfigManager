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
            ConfigWindow.Get().cache.sourceFolder = GetSelectedPathOrFallback();
            ConfigWindow.Get().SaveCache();
        }

        [MenuItem("Assets/Config Manager/Set to Config Output")]
        static void Set2ConfigOutput()
        {
            ConfigWindow.Get().cache.configOutputFolder = GetSelectedPathOrFallback();
            ConfigWindow.Get().SaveCache();
        }

        [MenuItem("Assets/Config Manager/Set to Asset Output")]
        static void Set2AssetOutput()
        {
            ConfigWindow.Get().cache.assetOutputFolder = GetSelectedPathOrFallback();
            ConfigWindow.Get().SaveCache();
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
