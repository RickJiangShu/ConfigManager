/*
 * Author:  Rick
 * Create:  2017/7/18 16:38:31
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    /// <summary>
    /// 表格（.txt .csv .xls .xlsx）
    /// </summary>
    public class SheetSource : Source
    {
        /// <summary>
        /// 解析出来的矩阵
        /// </summary>
        public string[,] matrix;

        /// <summary>
        /// 行
        /// </summary>
        public int row
        {
            get { return matrix.GetLength(0); }
        }

        /// <summary>
        /// 列
        /// </summary>
        public int column
        {
            get { return matrix.GetLength(1); }
        }
    }
}

