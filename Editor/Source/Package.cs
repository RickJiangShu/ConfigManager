/*
 * Author:  Rick
 * Create:  2017/8/10 14:09:50
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System.Collections.Generic;

    /// <summary>
    /// 配置包
    /// </summary>
    [System.Serializable]
    public class Package
    {
        public string name;
        public List<string> paths;
    }
}
