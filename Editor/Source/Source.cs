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
        /// 类名
        /// </summary>
        public string className;
    }

    public enum SourceType
    {
        Txt,
        Csv,
        Json,
        Xml,
        Xlsx,
     //   Sheet,//表格型数据 txt csv
     //   Struct,//结构型数据 json xml
    }
}
