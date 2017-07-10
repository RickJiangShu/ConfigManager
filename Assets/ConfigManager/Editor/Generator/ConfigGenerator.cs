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
        public static void Generate(List<Source> sources,string outputFolder)
        {
            foreach (Source src in sources)
            {
                string content = templete;
                string outputPath = outputFolder + "/" + src.configName + ".cs";

                //属性声明
                string declareProperties = "";
                for (int x = 0; x < src.column; x++)
                {
                    string comment = src.matrix[0, x];
                    string csType = ConfigTools.SourceType2ConfigType(src.matrix[1, x]);
                    string field = src.matrix[2, x];
                    string declare = string.Format(templete2, comment, csType, field);
                    declareProperties += declare;
                }
                
                //替换
                content = content.Replace("/*ClassName*/", src.configName);
                content = content.Replace("/*DeclareProperties*/", declareProperties);

                //写入
                ConfigTools.WriteFile(outputPath, content);
            }
        }

    }

}
