/*
 * Author:  Rick
 * Create:  2017/7/11 10:05:46
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ConfigManagerEditor
{
    /// <summary>
    /// 用于生成解析器
    /// </summary>
    public class DeserializerGenerator : ScriptableObject
    {

        private const string templete =
 @"public class Deserializer
{
    public static void Deserialize(SerializableSet set)
    {
/*SetDictionaries*/
    }
}
";
        private const string templete2 =
@"        for (int i = 0, l = set./*SourceName*/s.Length; i < l; i++)
        {
            /*ConfigName*/.dict.Add(set./*SourceName*/s[i]./*IDField*/, set./*SourceName*/s[i]);
        }
";

        public static void Generate(List<Source> sources,string outputFolder)
        {
            string outputPath = outputFolder + "/Deserializer.cs";
            string content = templete;


            string setDictionaries = "";
            foreach (Source src in sources)
            {
                string idField = src.matrix[2, 0];
                string setScript = templete2;

                setScript = setScript.Replace("/*ConfigName*/", src.configName);
                setScript = setScript.Replace("/*SourceName*/", src.sourceName);
                setScript = setScript.Replace("/*IDField*/", idField);

                setDictionaries += setScript;
            }

            content = content.Replace("/*SetDictionaries*/", setDictionaries);
            ConfigTools.WriteFile(outputPath, content);
        }
    }
}
