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
        private const string templete =
@"[System.Serializable]
public class SerializableSet : UnityEngine.ScriptableObject
{
/*DeclareConfigs*/
}
";
        private const string templete2 = 
@"    public /*ConfigName*/[] /*SourceName*/s;
";

        public static void Generate(List<SheetSource> sheets, string outputFolder)
        {
            string outputPath = outputFolder + "/SerializableSet.cs";
            string content = templete;

            string declareConfigs = "";
            foreach (Source sheet in sheets)
            {
                string declare = templete2;
                declare = declare.Replace("/*ConfigName*/", sheet.configName);
                declare = declare.Replace("/*SourceName*/", sheet.sourceName);
                declareConfigs += declare;
            }

            content = content.Replace("/*DeclareConfigs*/", declareConfigs);

            ConfigTools.WriteFile(outputPath, content);
        }
    }
}
