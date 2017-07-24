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
    public class SheetGenerator
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
        public static void Generate(List<SheetSource> sheets, string outputFolder)
        {
            foreach (SheetSource src in sheets)
            {
                string content = templete;
                string outputPath = outputFolder + "/" + src.className + ".cs";

                string idType = src.matrix[1, 0];
                string idField = src.matrix[2, 0];

                //属性声明
                string declareProperties = "";
                for (int x = 0; x < src.column; x++)
                {
                    string comment = src.matrix[0, x];
                    string csType = src.matrix[1, x];
                    string field = src.matrix[2, x];
                    string declare = string.Format(templete2, comment, csType, field);
                    declareProperties += declare;
                }

                //替换
                content = content.Replace("/*ClassName*/", src.className);
                content = content.Replace("/*DeclareProperties*/", declareProperties);
                content = content.Replace("/*IDType*/", idType);
                content = content.Replace("/*IDField*/", idField);

                //写入
                ConfigTools.WriteFile(outputPath, content);
            }
        }

    }

}
