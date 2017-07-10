/*
 * Author:  Rick
 * Create:  2017/7/10 13:49:46
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    /// <summary>
    /// 源数据
    /// </summary>
    public class Source
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string content;

        /// <summary>
        /// 源文件的文件名
        /// </summary>
        public string sourceName;

        /// <summary>
        /// 配置的文件名
        /// </summary>
        public string configName;

        /// <summary>
        /// 解析出来的矩阵
        /// </summary>
        public string[,] matrix;

        /// <summary>
        /// 行
        /// </summary>
        public int row;

        /// <summary>
        /// 列
        /// </summary>
        public int column;
    }
}
