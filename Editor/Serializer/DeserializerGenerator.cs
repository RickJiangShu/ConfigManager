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
@"       
        for (int i = 0, l = set./*SourceName*/s.Length; i < l; i++)
        {
            /*ConfigName*/.GetDictionary().Add(set./*SourceName*/s[i]./*IDField*/, set./*SourceName*/s[i]);
        }
";

        public static void Generate(List<SheetSource> sheets,string outputFolder)
        {
            string outputPath = outputFolder + "/Deserializer.cs";
            string content = templete;


            string setDictionaries = "";
            foreach (SheetSource sheet in sheets)
            {
                string idField = sheet.matrix[2, 0];
                string setScript = templete2;

                setScript = setScript.Replace("/*ConfigName*/", sheet.configName);
                setScript = setScript.Replace("/*SourceName*/", sheet.sourceName);
                setScript = setScript.Replace("/*IDField*/", idField);

                setDictionaries += setScript;
            }

            content = content.Replace("/*SetDictionaries*/", setDictionaries);
            ConfigTools.WriteFile(outputPath, content);
        }
    }
}
