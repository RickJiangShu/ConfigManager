/*
 * Author:  Rick
 * Create:  2017/7/18 17:08:58
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// JSONGenerator
    /// </summary>
    public class JSONGenerator
    {
        private const string templateRoot =
@"[System.Serializable]
public class /*ClassName*/
{
    /*StaticProperies*/

    public static /*ClassName*/ ins;
    
    /*Declarations*/
}";
        private const string templateObjContainer =
@"[System.Serializable]
public class JSONObject
{
    /*SubClasses*/
}";

        private const string tempalteSubObject =
@"[System.Serializable]
public class JSONObject{ID}
{
    /*Declarations*/
}";
        private static string tempFieldDeclaration =
@"public {1} {2};

";


        internal static List<SubClass> subClasses;
        internal static uint id;

        public static void Generate(List<JSONSource> jsons, string outputFolder)
        {
            subClasses = new List<SubClass>();
            id = 0;

            //创建各个JSON
            foreach (JSONSource src in jsons)
            {
                string content = templateRoot;
                string outputPath = outputFolder + "/" + src.configName + ".cs";

                //Decalre

            }

            //创建JSONObject子类

            subClasses = null;
        }

        internal static string[] GetFields(Dictionary<string, object> obj, out string[] types)
        {
            types = null;
            return null;
            //object[]
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string AddSubClass(Dictionary<string, object> obj)
        {
            string declarations = "";
            foreach (string field in obj.Keys)
            {
                object value = obj[field];
                string type = value.GetType().ToString();

                string declaration = string.Format(tempFieldDeclaration, type, field);
                declarations += declaration;
            }

            //通过检测声明检测是否与之前重复
            foreach (SubClass sub in subClasses)
            {
                if (sub.Equals(declarations))
                {
                    return sub.name;
                }
            }

            //创建新的SubClass
            return "";
        }

        internal class SubClass
        {
            public uint id;
            public string name;
            public string content;
            public string declareations;//声明

            public bool Equals(string s)
            {
                return declareations.Equals(s);
            }
        }
    }

}

