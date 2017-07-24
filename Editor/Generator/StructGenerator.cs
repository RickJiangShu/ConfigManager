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
    /// 结构化配置生成器
    /// </summary>
    public class StructGenerator
    {
        private const string templateRoot =
@"[System.Serializable]
public class /*ClassName*/
{
/*Declarations*/
    public static /*ClassName*/ instance;
    public static /*ClassName*/ Get()
    {
        return instance;
    }
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

        private static List<StructSubObject> subObjects;
        private static uint subID;

        public static void Generate(List<StructSource> structs, string outputFolder)
        {
            subObjects = new List<StructSubObject>();
            subID = 0;

            //创建各个Json
            foreach (StructSource src in structs)
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

            //创建JsonObject子类
            string allSubClasses = "";

            foreach (StructSubObject subObj in subObjects)
            {
                string declarations = CombineDeclarations(subObj.declarations);
                string subClass = templateSubObject;
                subClass = subClass.Replace("/*ClassName*/", subObj.name);
                subClass = subClass.Replace("/*Declarations*/", declarations);

                allSubClasses += subClass;
            }

            //写入
            ConfigTools.WriteFile(outputFolder + "/" + "StructObjects.cs",allSubClasses);

            subObjects = null;
        }

        /// <summary>
        /// 声明对象
        /// </summary>
        private static List<Declaration> DeclareObject(Dictionary<string, object> obj)
        {
            List<Declaration> declarations = new List<Declaration>();

            //创建声明
            foreach (string field in obj.Keys)
            {
                object value = obj[field];
                string typeStr = GetDeclarationType(value);

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
        private static string DeclareArray(object[] array)
        {
            //判断是否需要创建Array类
            bool arrayClass = false;
            Type lt = null;
            object li = null;
            foreach (object item in array)
            {
                Type t = item.GetType();
                if (t.IsArray)
                {
                    arrayClass = true;
                    break;
                }
                if (lt != null)
                {
                    //类型不同
                    if (lt != t)
                    {
                        //数字类型不同不算
                        if (!ConfigTools.IsNumber(lt) || !ConfigTools.IsNumber(t))
                        {
                            arrayClass = true;
                            break;
                        }
                    }
                    //类型相同 && 都是Dictionary<string, object>
                    else if(t == ConfigConstants.OBJECT_DICTIONARY_TYPE)
                    {
                        if (!ConfigTools.IsSameObjectDictionary((Dictionary<string, object>)li, (Dictionary<string, object>)item))
                        {
                            arrayClass = true;
                            break;
                        }
                    }
                }
                lt = t;
                li = item;
            }

            //普通数组
            if (!arrayClass)
            {
                Type firstType = array[0].GetType();
                if (firstType == typeof(Dictionary<string, object>))
                {
                    return GetSubObject((Dictionary<string, object>)array[0]) + "[]";
                }
                if (ConfigTools.IsNumber(firstType))//数字
                {
                    return ConfigTools.FindMinimalNumberType(array).ToString() + "[]";
                }
                else
                {
                    //string bool
                    return firstType.ToString() + "[]";
                }
            }
            else//创建数组对象
            {
                List<Declaration> declarations = new List<Declaration>();
                int i = 0;
                foreach (object item in array)
                {
                    Declaration declaration = new Declaration();
                    declaration.field = "arg" + i++;
                    declaration.type = GetDeclarationType(item);
                    declarations.Add(declaration);
                }
                return GetSubObject(declarations);
            }

            /*
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
             */
        }

        
        /// <summary>
        /// 组合声明成字符串
        /// </summary>
        /// <param name="declarations"></param>
        /// <returns></returns>
        private static string CombineDeclarations(List<Declaration> declarations)
        {
            string declarationsStr = "";
            foreach (Declaration declaration in declarations)
            {
                declarationsStr += string.Format(tempDeclaration, declaration.type, declaration.field);
            }
            return declarationsStr;
        }

        /// <summary>
        /// 获取Object、Array 和 基础类型的声明字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetDeclarationType(object value)
        {
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
            return typeStr;
        }

        /// <summary>
        /// 获取/新建SubObject
        /// </summary>
        /// <returns></returns>
        private static string GetSubObject(Dictionary<string, object> obj)
        {
            List<Declaration> declarations = DeclareObject(obj);//声明列表
            return GetSubObject(declarations);
        }

        private static string GetSubObject(List<Declaration> declarations)
        {
            //和已有的子对象进行比对
            foreach (StructSubObject subObj in subObjects)
            {
                if (subObj.Equals(declarations))
                    return subObj.name;
            }

            //新建子对象
            StructSubObject subObject = new StructSubObject();
            subObject.id = ++subID;
            subObject.name = "StructObject" + subObject.id;
            subObject.declarations = declarations;
            subObjects.Add(subObject);

            return subObject.name;
        }


        /// <summary>
        /// JsonObject 子对象
        /// </summary>
        private class StructSubObject
        {
            public uint id;
            public string name;
            public List<Declaration> declarations;//声明

            /// <summary>
            /// 对比
            /// </summary>
            /// <param name="ds"></param>
            /// <returns></returns>
            public bool Equals(List<Declaration> ds)
            {
                int l = declarations.Count;
                int k = ds.Count;
                if (l != k)
                    return false;

                for (int i = 0; i < l; i++)
                {
                    bool isHave = false;
                    for (int j = 0; j < k; j++)
                    {
                        if (declarations[i].type == ds[j].type && declarations[i].field == ds[j].field)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (!isHave)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 声明
        /// </summary>
        private class Declaration
        {
            public string type;
            public string field;
        }
    }

}

