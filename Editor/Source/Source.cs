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
        /// 原始内容(string/DataTable)
        /// </summary>
        public object original;

        /// <summary>
        /// 源文件的文件名
        /// </summary>
        public string originalName;

        /// <summary>
        /// 类名
        /// </summary>
        public string className;
    }

    /// <summary>
    /// 原始文件类型
    /// </summary>
    public enum OriginalType
    {
        Txt,
        Csv,
        Json,
        Xml,
        Xls,
        Xlsx,
     //   Sheet,//表格型数据 txt csv
     //   Struct,//结构型数据 json xml
    }

    /// <summary>
    /// 源类型
    /// </summary>
    public enum SourceType
    {
        Sheet,//表格
        Struct,//结构
    }
}
