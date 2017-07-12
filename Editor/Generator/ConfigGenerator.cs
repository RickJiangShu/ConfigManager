/*
 * Author:  Rick
 * Create:  2017/7/7 15:45:53
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Config解析器，负责将txt解析成*.cs
    /// </summary>
    public class ConfigGenerator
    {
        private static string templete =
@"using System.Collections.Generic;

[System.Serializable]
public class /*ClassName*/
{
/*DeclareProperties*/
    private static Dictionary</*IDType*/, /*ClassName*/> dictionary = new Dictionary</*IDType*/, /*ClassName*/>();

    /// <summary>
    /// 通过/*IDField*/获取/*ClassName*/的实例
    /// </summary>
    /// <param name=""/*IDField*/"">索引</param>
    /// <returns>/*ClassName*/的实例</returns>
    public static /*ClassName*/ Get(/*IDType*/ /*IDField*/)
    {
        return dictionary[/*IDField*/];
    }
    
    /// <summary>
    /// 获取字典
    /// </summary>
    /// <returns>字典</returns>
    public static Dictionary</*IDType*/, /*ClassName*/> GetDictionary()
    {
        return dictionary;
    }

    /// <summary>
    /// 获取所有键
    /// </summary>
    /// <returns>所有键</returns>
    public static /*IDType*/[] GetKeys()
    {
        int count = dictionary.Keys.Count;
        /*IDType*/[] keys = new /*IDType*/[count];
        dictionary.Keys.CopyTo(keys,0);
        return keys;
    }

    /// <summary>
    /// 获取所有实例
    /// </summary>
    /// <returns>所有实例</returns>
    public static /*ClassName*/[] GetValues()
    {
        int count = dictionary.Values.Count;
        /*ClassName*/[] values = new /*ClassName*/[count];
        dictionary.Values.CopyTo(values, 0);
        return values;
    }
}
";
        private static string templete2 =
@"    /// <summary>
    /// {0}
    /// </summary>
    public {1} {2};

";

        /// <summary>
        /// 生成Config
        /// </summary>
        public static void Generate(List<Source> sources, string outputFolder)
        {
            foreach (Source src in sources)
            {
                string content = templete;
                string outputPath = outputFolder + "/" + src.configName + ".cs";

                string idType = ConfigTools.SourceType2CSharpType(src.matrix[1, 0]);
                string idField = src.matrix[2, 0];

                //属性声明
                string declareProperties = "";
                for (int x = 0; x < src.column; x++)
                {
                    string comment = src.matrix[0, x];
                    string csType = ConfigTools.SourceType2CSharpType(src.matrix[1, x]);
                    string field = src.matrix[2, x];
                    string declare = string.Format(templete2, comment, csType, field);
                    declareProperties += declare;
                }

                //替换
                content = content.Replace("/*ClassName*/", src.configName);
                content = content.Replace("/*DeclareProperties*/", declareProperties);
                content = content.Replace("/*IDType*/", idType);
                content = content.Replace("/*IDField*/", idField);

                //写入
                ConfigTools.WriteFile(outputPath, content);
            }
        }

    }

}
