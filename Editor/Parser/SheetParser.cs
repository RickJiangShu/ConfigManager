/*
 * Author:  Rick
 * Create:  2017/7/18 16:40:02
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using Excel;
    using System.Data;
    using System.Text.RegularExpressions;
    /// <summary>
    /// SheetParser
    /// </summary>
    public class SheetParser
    {

        /// <summary>
        /// Excel
        /// </summary>
        /// <param name="tabel"></param>
        /// <returns></returns>
        public static SheetSource Parse(DataTable table,string fileName)
        {
            int column = table.Columns.Count;
            int row = table.Rows.Count;

            string[,] matrix = new string[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    string value = table.Rows[i][j].ToString();
                    matrix[i, j] = value;
                }
            }
            ConvertOriginalType(matrix);
            string originalName = fileName.Substring(0, fileName.LastIndexOf('.'));
            string className = originalName + "Sheet";
            return CreateSource(table, originalName, className, matrix, row, column);
        }

        /// <summary>
        /// 切割字符串（txt/csv）
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SheetSource Parse(string content,string fileName)
        {
            string lf;
            string sv;
            bool isCsv;

            //判断换行符
            if (content.IndexOf("\r\n") != -1)
                lf = "\r\n";
            else
                lf = "\n";

            //判断分割符
            if (content.IndexOf("\t") != -1)
            {
                sv = "\t";
                isCsv = false;
            }
            else
            {
                sv = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";//Fork:https://stackoverflow.com/questions/6542996/how-to-split-csv-whose-columns-may-contain
                isCsv = true;
            }

            string originalName = fileName.Substring(0, fileName.LastIndexOf('.'));
            string className = originalName + "Sheet";
            int row;
            int column;
            string[,] matrix = Content2Matrix(content, sv, lf, out row, out column);

            if (isCsv)
                ClearCsvQuotation(matrix);

            ConvertOriginalType(matrix);
            return CreateSource(content, originalName, className, matrix, row, column);
        }

        private static SheetSource CreateSource(object original,string originalName,string className,string[,] matrix,int row,int column)
        {
            SheetSource source = new SheetSource();
            source.original = original;
            source.originalName = originalName;//文件名
            source.className = className;//类名
            source.matrix = matrix;
            source.row = row;
            source.column = column;
            return source;
        }

        /// <summary>
        /// 从配置转换成矩阵数组（0行1列）
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="sv">分隔符 Separated Values</param>
        /// <param name="lf">换行符 Line Feed</param>
        /// <returns></returns>
        private static string[,] Content2Matrix(string config, string sv, string lf, out int row, out int col)
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

        /// <summary>
        /// 转换原始配置配型 -> CShartType
        /// </summary>
        private static void ConvertOriginalType(string[,] matrix)
        {
            int column = matrix.GetLength(1);
            for (int x = 0; x < column; x++)
            {
                matrix[1, x] = ConfigTools.SourceType2CSharpType(x, matrix);
            }
        }

        /// <summary>
        /// 去除CSV引号
        /// </summary>
        /// <param name="matrix"></param>
        private static void ClearCsvQuotation(string[,] matrix)
        {
            int row = matrix.GetLength(0);
            int column = matrix.GetLength(1);
            for (int y = 0; y < row; y++)
            {
                for (int x = 0; x < column; x++)
                {
                    string v = matrix[y, x];
                    if (string.IsNullOrEmpty(v) || v.Length < 2)
                        continue;

                    if (v[0] == '"')
                    {
                        v = v.Remove(0, 1);
                        v = v.Remove(v.Length - 1, 1);
                        matrix[y, x] = v;
                    }
                }
            }
        }
    }
}

