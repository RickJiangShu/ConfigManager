/*
 * Author:  Rick
 * Create:  2017/7/18 17:08:58
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System;
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
    public static /*ClassName*/ ins;

/*Declarations*/
}";

        private static string tempDeclaration =
@"    public {0} {1};
";

        
        private const string templateSubObject =
@"[System.Serializable]
public class /*ClassName*/
{
/*Declarations*/
}
";

        
        //decalration

        internal static List<JSONSubObject> subObjects;
        internal static uint subID;

        public static void Generate(List<JSONSource> jsons, string outputFolder)
        {
            subObjects = new List<JSONSubObject>();
            subID = 0;

            //创建各个JSON
            foreach (JSONSource src in jsons)
            {
                string content = templateRoot;
                string outputPath = outputFolder + "/" + src.className + ".cs";

                //Decalre
                List<Declaration> declarations = DeclareObject(src.obj);


                string declarationStr = CombineDeclarations(declarations);

                //替换
                content = content.Replace("/*ClassName*/", src.className);
                content = content.Replace("/*Declarations*/", declarationStr);

                //写入
                ConfigTools.WriteFile(outputPath, content);
            }

            //创建JSONObject子类
            string allSubClasses = "";

            foreach (JSONSubObject subObj in subObjects)
            {
                string declarations = CombineDeclarations(subObj.declarations);
                string subClass = templateSubObject;
                subClass = subClass.Replace("/*ClassName*/", subObj.name);
                subClass = subClass.Replace("/*Declarations*/", declarations);

                allSubClasses += subClass;
            }

            //写入
            ConfigTools.WriteFile(outputFolder + "/" + "JSONObjects.cs",allSubClasses);

            subObjects = null;
        }

        /// <summary>
        /// 声明对象
        /// </summary>
        internal static List<Declaration> DeclareObject(Dictionary<string, object> obj)
        {
            List<Declaration> declarations = new List<Declaration>();

            //创建声明
            foreach (string field in obj.Keys)
            {
                object value = obj[field];
                Type type = value.GetType();

                string typeStr = "";
                //对象
                if (type == typeof(Dictionary<string, object>))
                {
                    string subObjName = GetSubObject((Dictionary<string, object>)value);
                    typeStr = subObjName;
                }
                //数组
                else if (type.IsArray)
                {
                    //检测是否需要添加子类
                    object[] array = (object[])value;
                    typeStr = DeclareArray(array);
                    
                }
                else
                {
                    //字符串、Number、Bool
                    typeStr = type.ToString();
                }

                Declaration declaration = new Declaration();
                declaration.type = typeStr;
                declaration.field = field;
                declarations.Add(declaration);
            }


            return declarations;
        }

        /// <summary>
        /// 检测数组中是否含有子对象
        /// </summary>
        /// <param name="array"></param>
        internal static string DeclareArray(object[] array)
        {
            /*
               注意：
               由于要考虑序列化的问题 http://answers.unity3d.com/questions/1322769/parsing-nested-arrays-with-jsonutility.html
               1.不支持多类型数组
               2.不支持嵌套数组
               可以通过配置层面解决这两个问题，即把“多类型数组”和“嵌套数组”配置成Object
             */

            //获取所有项的类型名称
            Type type = array[0].GetType();
            //对象
            if (type == typeof(Dictionary<string, object>))
            {
                return GetSubObject((Dictionary<string, object>)array[0]) + "[]";
            }
            //数组
            else if (type.IsArray)
            {
                Debug.LogError("不支持嵌套数组！");
                return "string";
            }
            else
            {
                return type.ToString() + "[]";
            }
        }

        
        /// <summary>
        /// 组合声明成字符串
        /// </summary>
        /// <param name="declarations"></param>
        /// <returns></returns>
        internal static string CombineDeclarations(List<Declaration> declarations)
        {
            string declarationsStr = "";
            foreach (Declaration declaration in declarations)
            {
                declarationsStr += string.Format(tempDeclaration, declaration.type, declaration.field);
            }
            return declarationsStr;
        }

        /// <summary>
        /// 获取/新建SubObject
        /// </summary>
        /// <returns></returns>
        internal static string GetSubObject(Dictionary<string, object> obj)
        {
            List<Declaration> declarations = DeclareObject(obj);//声明列表

            //和已有的子对象进行比对
            foreach (JSONSubObject subObj in subObjects)
            {
                if (subObj.Equals(declarations))
                    return subObj.name;
            }

            //新建子对象
            JSONSubObject subObject = new JSONSubObject();
            subObject.id = ++subID;
            subObject.name = "JSONObject" + subObject.id;
            subObject.declarations = declarations;
            subObjects.Add(subObject);

            return subObject.name;
        }


        /// <summary>
        /// JSONObject 子对象
        /// </summary>
        internal class JSONSubObject
        {
            public uint id;
            public string name;
            public List<Declaration> declarations;//声明

            public bool Equals(List<Declaration> ds)
            {
                int l = declarations.Count;
                int k = ds.Count;
                if (l != k)
                    return false;

                for (int i = 0; i < l; i++)
                {
                    if (declarations[i].type != ds[i].type || declarations[i].field != ds[i].field)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 声明
        /// </summary>
        internal class Declaration
        {
            public string type;
            public string field;
        }
    }

}

