/*
 * Author:  Rick
 * Create:  2017/7/10 10:09:42
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
namespace ConfigManagerEditor
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    /// <summary>
    /// 序列化器，通过反射的方式序列化出SerializableList
    /// </summary>
    public class Serializer
    {
        public static object Serialize(List<SheetSource> sheets,List<StructSource> structs)
        {
            Type t = FindType("SerializableSet");
            if (t == null)
            {
                UnityEngine.Debug.LogError("找不到SerializableSet类！");
                return null;
            }

            object set = UnityEngine.ScriptableObject.CreateInstance(t);

            //Config
            foreach (SheetSource source in sheets)
            {
                string fieldName = source.originalName + "s";
                Array configs = Source2Configs(source);
                FieldInfo fieldInfo = t.GetField(fieldName);
                fieldInfo.SetValue(set,configs);
            }

            //Json
            foreach (StructSource source in structs)
            {
                string fieldName = source.originalName;

                //使用 FromJson 获取json对象
                Type jsonType = FindType(source.className);
                object jsonObj = SerializeObject(jsonType, source.obj);

                FieldInfo fieldInfo = t.GetField(fieldName);
                fieldInfo.SetValue(set, jsonObj);
            }
            return set;
        }

        /// <summary>
        /// 从源数据反射为对应的配置数组
        /// </summary>
        /// <returns></returns>
        private static Array Source2Configs(SheetSource source)
        {
            Type configType = FindType(source.className);

            int count = source.row - 3;
            Array configs = Array.CreateInstance(configType, count);
            for(int y = 3,i = 0;i<count;y++,i++)
            {
                object config = Activator.CreateInstance(configType);
                for(int x = 0;x<source.column;x++)
                {
                    string valueType = source.matrix[1,x];
                    string valueField = source.matrix[2,x];
                    string valueString = source.matrix[y,x];
                    FieldInfo field = configType.GetField(valueField);

                    try
                    {
                        object value = ConfigTools.SourceValue2Object(valueType, valueString);
                        field.SetValue(config, value);
                    }
                    catch
                    {
                        UnityEngine.Debug.LogError(string.Format("SourceValue2Object Error!valueType={0},valueString={1}",valueType,valueString));
                    }
                }
                configs.SetValue(config, i);
            }
            return configs;
        }


        /// <summary>
        /// 序列化结构对象
        /// </summary>
        /// <param name="inputObject"></param>
        /// <returns></returns>
        private static object SerializeObject(Type outputType,Dictionary<string, object> inputObject)
        {
            object obj = Activator.CreateInstance(outputType);

            foreach (string field in inputObject.Keys)
            {
                FieldInfo fieldInfo = outputType.GetField(field);
                Type fieldType = fieldInfo.FieldType;//生成类的类型

                object value = inputObject[field];
                Type type = value.GetType();//解析的类型
             
                //对象
                if (type == typeof(Dictionary<string, object>))
                {
                    object subOjbect = SerializeObject(fieldType, (Dictionary<string, object>)value);
                    fieldInfo.SetValue(obj, subOjbect);
                }
                //数组
                else if (type.IsArray)
                {
                    object array = SerializeArray(fieldType, (object[])value);
                    fieldInfo.SetValue(obj, array);
                }
                //String\Number\Bool
                else
                {
                    fieldInfo.SetValue(obj, value);
                }
            }
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputType">FieldType</param>
        /// <param name="inputArray">object[]</param>
        /// <returns></returns>
        private static object SerializeArray(Type outputType,object[] inputArray)
        {
            //普通数组
            if (outputType.IsArray)
            {
                Type elementType = outputType.GetElementType();
                Array outputArray = Array.CreateInstance(elementType, inputArray.Length);

                if (inputArray[0].GetType() == typeof(Dictionary<string, object>))
                {
                    for (int i = 0; i < inputArray.Length; i++)
                    {
                        outputArray.SetValue(SerializeObject(elementType, (Dictionary<string, object>)inputArray[i]), i);
                    }
                }
                else
                {
                    for (int i = 0; i < inputArray.Length; i++)
                    {
                        outputArray.SetValue(inputArray[i], i);
                    }
                }
                return outputArray;
            }
            //自定义数组
            else
            {
                object arrayObject = Activator.CreateInstance(outputType);
                for (int i = 0, l = inputArray.Length; i < l; i++)
                {
                    string field = "arg" + i;
                    FieldInfo fieldInfo = outputType.GetField(field);

                    //对象
                    if (inputArray[i].GetType() == typeof(Dictionary<string, object>))
                    {
                        object value = SerializeObject(fieldInfo.FieldType, (Dictionary<string, object>)inputArray[i]);
                        fieldInfo.SetValue(arrayObject, value);
                    }
                    //数组
                    else if (inputArray[i].GetType().IsArray)
                    {
                        fieldInfo.SetValue(arrayObject, SerializeArray(fieldInfo.FieldType, (object[])inputArray[i]));
                    }
                    else
                    {
                        fieldInfo.SetValue(arrayObject, inputArray[i]);
                    }
                }
                return arrayObject;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        private static Type FindType(string qualifiedTypeName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }
    }
}
