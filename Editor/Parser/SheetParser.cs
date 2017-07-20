/*
 * Author:  Rick
 * Create:  2017/7/18 16:40:02
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System.Text.RegularExpressions;
    /// <summary>
    /// SheetParser
    /// </summary>
    public class SheetParser
    {
        public static SheetSource Parse(string content,string fileName)
        {
            SheetSource source = new SheetSource();
            string lf;
            string sv;

            //判断换行符
            if (content.IndexOf("\r\n") != -1)
                lf = "\r\n";
            else
                lf = "\n";

            //判断分割符
            if (content.IndexOf("\t") != -1)
                sv = "\t";
            else
                sv = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";//Fork:https://stackoverflow.com/questions/6542996/how-to-split-csv-whose-columns-may-contain


            //写入源
            source.content = content;
            source.sourceName = fileName.Substring(0,fileName.LastIndexOf('.')); ;//文件名
            source.className = source.sourceName + "Config";//类名
            source.matrix = Content2Matrix(source.content, sv, lf, out source.row, out source.column);
           
            return source;
        }
        /// <summary>
        /// 从配置转换成矩阵数组（0行1列）
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="sv">分隔符 Separated Values</param>
        /// <param name="lf">换行符 Line Feed</param>
        /// <returns></returns>
        internal static string[,] Content2Matrix(string config, string sv, string lf, out int row, out int col)
        {
            config = config.Trim();//清空末尾的空白

            //分割
            string[] lines = Regex.Split(config, lf);
            string[] firstLine = Regex.Split(lines[0], sv, RegexOptions.Compiled);

            row = lines.Length;
            col = firstLine.Length;
            string[,] matrix = new string[row, col];
            //为第一行赋值
            for (int i = 0, l = firstLine.Length; i < l; i++)
            {
                matrix[0, i] = firstLine[i];
            }
            //为其他行赋值
            for (int i = 1, l = lines.Length; i < l; i++)
            {
                string[] line = Regex.Split(lines[i], sv);
                for (int j = 0, k = line.Length; j < k; j++)
                {
                    matrix[i, j] = line[j];
                }
            }
            return matrix;
        }
    }
}

