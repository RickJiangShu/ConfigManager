/*
 * Author:  Rick
 * Create:  2017/7/10 15:04:49
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// 序列化集合 生产
    /// </summary>
    public class SerializableSetGenerator
    {
        private const string template =
@"[System.Serializable]
public class SerializableSet : UnityEngine.ScriptableObject
{
/*ConfigDeclarations*/
/*JsonDeclarations*/
}
";
        private const string template2 = 
@"    public /*ConfigName*/[] /*SourceName*/s;
";
        private const string template3 =
@"    public /*JsonName*/ /*SourceName*/;
";

        public static void Generate(List<SheetSource> sheets,List<StructSource> jsons, string outputFolder)
        {
            string outputPath = outputFolder + "/SerializableSet.cs";
            string content = template;

            //Sheet声明
            string configDeclarations = "";
            foreach (SheetSource sheet in sheets)
            {
                string declaration = template2;
                declaration = declaration.Replace("/*ConfigName*/", sheet.className);
                declaration = declaration.Replace("/*SourceName*/", sheet.sourceName);
                configDeclarations += declaration;
            }
            content = content.Replace("/*ConfigDeclarations*/", configDeclarations);

            //Json声明
            string jsonDeclarations = "";
            foreach (StructSource json in jsons)
            {
                string declaration = template3;
                declaration = declaration.Replace("/*JsonName*/", json.className);
                declaration = declaration.Replace("/*SourceName*/", json.sourceName);
                jsonDeclarations += declaration;
            }
            content = content.Replace("/*JsonDeclarations*/", jsonDeclarations);

            ConfigTools.WriteFile(outputPath, content);
        }
    }
}
