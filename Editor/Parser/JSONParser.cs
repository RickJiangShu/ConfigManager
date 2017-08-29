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
    /// Json解析器
    /// </summary>
    public class JsonParser
    {
        #region 正则表达式
        //基础类型
        private const string regexString = "\"([^\"]*)\"";//"[^"]*"
        private const string regexNumber = @"(-?(?=[1-9]|0(?!\d))\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)";
        private const string regexBool = "(true|false|null)";
        #endregion

        public static StructSource Parse(string content,string fileName)
        {
            StructSource source = new StructSource();

            source.original = content;
            source.originalName = fileName.Substring(0, fileName.LastIndexOf('.')); ;//文件名
            source.className = source.originalName + "Json";//类名

            source.obj = ParseObject(content);
            return source;
        }
        

        /// <summary>
        /// 解析对象
        /// </summary>
        /// <param name="contentOfObject"></param>
        /// <returns></returns>
        private static Dictionary<string, object> ParseObject(string contentOfObject)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            List<string> items = Split(contentOfObject);

            for (int i = 0, l = items.Count; i < l; i++)
            {
                string item = items[i];
                string name;
                string contentOfValue;
                SplitObject(item, out name, out contentOfValue);//切割对象

                int type = IsObjectOrArray(contentOfValue);
                object value = null;

                switch (type)
                {
                    case 0:
                        value = ParseBaseValue(contentOfValue);
                        break;
                    case 1:
                        value = ParseObject(contentOfValue);
                        break;
                    case 2:
                        value = ParseArray(contentOfValue);
                        break;
                }
                obj.Add(name, value);
            }
            return obj;
        }

        /// <summary>
        /// 解析数组
        /// </summary>
        /// <param name="contentOfArray"></param>
        /// <returns></returns>
        private static object[] ParseArray(string contentOfArray)
        {
            List<object> objects = new List<object>();
            List<string> items = Split(contentOfArray);

            for (int i = 0, l = items.Count; i < l; i++)
            {
                string item = items[i];
                int type = IsObjectOrArray(item);
                switch(type)
                {
                    case 0:
                        objects.Add(ParseBaseValue(item));
                        break;
                    case 1:
                        objects.Add(ParseObject(item));
                        break;
                    case 2:
                        objects.Add(ParseArray(item));
                        break;
                }
            }
            return objects.ToArray();
        }

        private static object ParseBaseValue(string contentOfValue)
        {
            Match result;
            result = Regex.Match(contentOfValue, regexString);
            if (result.Success)
            {
                return result.Groups[1].Value;
            }

            result = Regex.Match(contentOfValue, regexNumber);
            if (result.Success)
            {
                return ConfigTools.ConvertNumber(result.Groups[1].Value);
            }

            result = Regex.Match(contentOfValue, regexBool);
            if (result.Success)
            {
                return ConfigTools.ConvertBool(result.Groups[1].Value);
            }

            return null;
        }


        /// <summary>
        /// 切割对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contentOfValue"></param>
        private static void SplitObject(string item, out string name, out string contentOfValue)
        {
            int nameStart = -1;
            int nameEnd = -1;
            int colon = -1;

            for (int i = 0, l = item.Length; i < l; i++)
            {
                if (item[i] == '"')
                {
                    if (nameStart == -1)
                        nameStart = i + 1;
                    else
                        nameEnd = i;
                }

                if (item[i] == ':')
                {
                    colon = i;
                    break;
                }
            }
            name = item.Substring(nameStart, nameEnd - nameStart);
            contentOfValue = item.Substring(colon + 1);//值为冒号后面都为值
        }

        /// <summary>
        /// 以','切割数组或数组
        /// </summary>
        /// <param name="objectOrArray">格式 {p1,p2,p3} or [p1,p2,p3] </param>
        /// <returns></returns>
        private static List<string> Split(string objectOrArray)
        {
            List<string> items = new List<string>();

            int last = -1;
            int level = -1;
            bool inString = false;
            for (int i = 0, l = objectOrArray.Length; i < l; i++)
            {
                char c = objectOrArray[i];

                if (c == '"')
                {
                    inString = !inString;
                }
                else
                {
                    if (inString)
                        continue;

                    if (c == '{' || c == '[')
                    {
                        if (++level == 0)
                        {
                            last = i + 1;
                        }
                    }
                    else if (c == '}' || c == ']')
                    {
                        if (level-- == 0)
                        {
                            string item = objectOrArray.Substring(last, i - last);
                            items.Add(item);
                        }
                    }
                    else if (c == ',' && level == 0)
                    {
                        string item = objectOrArray.Substring(last, i - last);
                        items.Add(item);
                        last = i + 1;
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// 判断是否是对象或者数组
        /// </summary>
        /// <returns></returns>
        private static int IsObjectOrArray(string contentOfValue)
        {
            for (int i = 0, l = contentOfValue.Length; i < l; i++)
            {
                if (contentOfValue[i] == '{')
                    return 1;
                else if (contentOfValue[i] == '[')
                    return 2;
            }
            return 0;
        }
    }
}
