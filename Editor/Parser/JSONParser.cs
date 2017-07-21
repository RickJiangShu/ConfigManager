/*
 * Author:  Rick
 * Create:  2017/7/18 11:14:54
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// JSON解析器
    /// </summary>
    public class JSONParser
    {
        #region 正则表达式
        //解析键值对
        internal const string regexPairOf = @"\s*" + regexString + @"\s*:\s*";//\s*"([^"]*)"\s*:\s*

        internal const string regexPairOfObject = regexPairOf + regexObject;
        internal const string regexPairOfArray = regexPairOf + regexArray;
        internal const string regexPairOfString = regexPairOf + regexString;
        internal const string regexPairOfNumber = regexPairOf + regexNumber;
        internal const string regexPairOfBool = regexPairOf + regexBool;


        //对象
        //internal const string regexObject = @"\{((?>\{(?<c>)|[^\{\}]+|\}(?<-c>))*(?(c)(?!)))\}";//Fork：https://stackoverflow.com/questions/546433/regular-expression-to-match-outer-brackets
        internal const string regexObject = @"(?<!\[[\s\S]*)\{((?>\{(?<c>)|[^\{\}]+|\}(?<-c>))*(?(c)(?!)))\}";//前面不含[的{}

        //数组
        internal const string regexArray = @"\[((?>\[(?<c>)|[^\[\]]+|\](?<-c>))*(?(c)(?!)))\]";

        //基础类型
        internal const string regexString = "\"([^\"]*)\"";//"[^"]*"
        internal const string regexNumber = @"(-?(?=[1-9]|0(?!\d))\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)";
        internal const string regexBool = "(true|false|null)";
        #endregion

        public static JSONSource Parse(string content,string fileName)
        {
            JSONSource source = new JSONSource();

            source.content = content;
            source.sourceName = fileName.Substring(0, fileName.LastIndexOf('.')); ;//文件名
            source.className = source.sourceName + "JSON";//类名

            source.obj = ParseRoot(content);
            return source;
        }

        /// <summary>
        /// 解析JSON入口
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static Dictionary<string, object> ParseRoot(string json)
        {
            Match match = Regex.Match(json, regexObject);
            if (match.Success)
            {
                string content = match.Groups[1].Value;
                return ParseObject(ref content);
            }
            return null;
        }
        /// <summary>
        /// 解析不带{} 为一个对象
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static Dictionary<string, object> ParseObject(ref string content)
        {
            //解析members
            Pair[] members = ParseMembers(ref content);

            //创建Object
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (members != null)
            {
                for (int i = 0, l = members.Length; i < l; i++)
                {
                    result.Add(members[i].key, members[i].value);
                }
            }
            return result;
        }

        /// <summary>
        /// 解析成员为键值对数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static Pair[] ParseMembers(ref string content)
        {
            List<Pair> membersList = new List<Pair>();

            string original = content.ToString();

            //判断Object类型
            Pair[] pairsOfObject = ParsePairs(ref content, regexPairOfObject);
            foreach (Pair pair in pairsOfObject)
            {
                pair.index = original.IndexOf(pair.content);
                pair.value = ParseObject(ref pair.content);
            }
            membersList.AddRange(pairsOfObject);

            //判断Array类型 
            Pair[] pairsOfArray = ParsePairs(ref content, regexPairOfArray);
            foreach (Pair pair in pairsOfArray)
            {
                pair.index = original.IndexOf(pair.content);
                pair.value = ParseArray(ref pair.content);
            }
            membersList.AddRange(pairsOfArray);

            //判断String类型
            Pair[] pairsOfString = ParsePairs(ref content, regexPairOfString);
            foreach (Pair pair in pairsOfString)
            {
                pair.index = original.IndexOf(pair.content);
                pair.value = pair.content;
            }
            membersList.AddRange(pairsOfString);

            //判断Number类型
            Pair[] pairsOfNumber = ParsePairs(ref content, regexPairOfNumber);
            foreach (Pair pair in pairsOfNumber)
            {
                pair.index = original.IndexOf(pair.content);
                pair.value = ConfigTools.ConvertNumber(pair.content);
            }
            membersList.AddRange(pairsOfNumber);

            //判断Bool类型
            Pair[] pairsOfBool = ParsePairs(ref content, regexPairOfBool);
            foreach (Pair pair in pairsOfBool)
            {
                pair.index = original.IndexOf(pair.content);
                pair.value = ConfigTools.ConvertBool(pair.content);
            }
            membersList.AddRange(pairsOfBool);

            //排序
            membersList.Sort(Pair.Compare);

            return membersList.ToArray();
        }

        /// <summary>
        /// 解析数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static object[] ParseArray(ref string content)
        {
            List<object> array = new List<object>();

            //排序
            string original = content.ToString();
            List<Sort> sorts = new List<Sort>();

            //Object
            MatchCollection matchObjects = MatchesAndRemove(ref content, regexObject);
            foreach (Match match in matchObjects)
            {
                string s = match.Groups[1].Value;
                sorts.Add(new Sort(original.IndexOf(s),ParseObject(ref s)));
            }

            //数组
            MatchCollection matchArraies = MatchesAndRemove(ref content, regexArray);
            foreach (Match match in matchArraies)
            {
                string s = match.Groups[1].Value;
                sorts.Add(new Sort(original.IndexOf(s), ParseArray(ref s)));
            }

            //String
            MatchCollection matchStrings = MatchesAndRemove(ref content, regexString);
            foreach (Match match in matchStrings)
            {
                string s = match.Groups[1].Value;
                sorts.Add(new Sort(original.IndexOf(s), s));
            }

            //Number
            MatchCollection matchNumbers = MatchesAndRemove(ref content, regexNumber);
            foreach (Match match in matchNumbers)
            {
                string s = match.Groups[1].Value;
                sorts.Add(new Sort(original.IndexOf(s), ConfigTools.ConvertNumber(s)));
            }

            //Bool
            MatchCollection matchBools = MatchesAndRemove(ref content, regexBool);
            foreach (Match match in matchBools)
            {
                string s = match.Groups[1].Value;
                sorts.Add(new Sort(original.IndexOf(s), ConfigTools.ConvertBool(s)));
            }

            //进行排序
            sorts.Sort(Sort.Compare);
            foreach (Sort s in sorts)
            {
                array.Add(s.data);
            }

            return array.ToArray();
        }


        /// <summary>
        /// 解析键值对基础（解析出key 和 content）
        /// </summary>
        /// <returns></returns>
        internal static Pair[] ParsePairs(ref string content, string regex)
        {
            MatchCollection matches = MatchesAndRemove(ref content, regex);
            int count = matches.Count;
            Pair[] pairs = new Pair[count];
            for (int i = 0; i < count; i++)
            {
                Match match = matches[i];
                string key = match.Groups[1].Value;
                string valueString = match.Groups[2].Value;
                pairs[i] = new Pair(key, valueString);
            }
            return pairs;
        }

        /// <summary>
        /// 匹配并删除字段
        /// </summary>
        /// <returns></returns>
        internal static MatchCollection MatchesAndRemove(ref string content, string regex)
        {
            MatchCollection matches = Regex.Matches(content, regex);
            int count = matches.Count;
            if (count > 0)
            {
                //删除解析过的字段（避免重复解析）
                content = Regex.Replace(content, regex, string.Empty);
            }
            return matches;
        }

        

        /// <summary>
        /// 键值对
        /// </summary>
        internal class Pair
        {
            public string key;//键
            public string content;//转换之前的文本
            public object value;//转换后的值
            public int index;//用于排序
            public Pair(string key, string content)
            {
                this.key = key;
                this.content = content;
            }

            public static int Compare(Pair a, Pair b)
            {
                if (a.index < b.index)
                    return -1;
                else if (a.index > b.index)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 用来排序正则取到的字符
        /// </summary>
        internal class Sort
        {
            public int index;
            public object data;

            public Sort(int i, object data)
            {
                this.index = i;
                this.data = data;
            }

            public static int Compare(Sort a, Sort b)
            {
                if (a.index < b.index)
                    return -1;
                else if (a.index > b.index)
                    return 1;
                else
                    return 0;
            }
        }
    }
}
