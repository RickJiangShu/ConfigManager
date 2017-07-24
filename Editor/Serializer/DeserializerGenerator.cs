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

        private const string template =
 @"public class Deserializer
{
    public static void Deserialize(SerializableSet set)
    {
/*SetDictionaries*/
/*SetJsons*/
    }
}
";
        private const string template2 =
@"       
        for (int i = 0, l = set./*SourceName*/s.Length; i < l; i++)
        {
            /*ConfigName*/.GetDictionary().Add(set./*SourceName*/s[i]./*IDField*/, set./*SourceName*/s[i]);
        }
";
        private const string template3 =
@"        /*ClassName*/.ins = set./*SourceName*/;
";

        public static void Generate(List<SheetSource> sheets,List<StructSource> jsons,string outputFolder)
        {
            string outputPath = outputFolder + "/Deserializer.cs";
            string content = template;

            //sheets
            string setDictionaries = "";
            foreach (SheetSource sheet in sheets)
            {
                string idField = sheet.matrix[2, 0];
                string setScript = template2;

                setScript = setScript.Replace("/*ConfigName*/", sheet.className);
                setScript = setScript.Replace("/*SourceName*/", sheet.originalName);
                setScript = setScript.Replace("/*IDField*/", idField);

                setDictionaries += setScript;
            }

            //jsons
            string setJsons = "";
            foreach (StructSource json in jsons)
            {
                string setScript = template3;
                setScript = setScript.Replace("/*ClassName*/", json.className);
                setScript = setScript.Replace("/*SourceName*/", json.originalName);

                setJsons += setScript;
            }


            content = content.Replace("/*SetDictionaries*/", setDictionaries);
            content = content.Replace("/*SetJsons*/", setJsons);

            ConfigTools.WriteFile(outputPath, content);
        }
    }
}
