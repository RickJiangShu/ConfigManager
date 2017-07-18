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
        public static object Serialize(List<SheetSource> sheets)
        {
            Type t = FindType("SerializableSet");
            if (t == null)
            {
                UnityEngine.Debug.LogError("找不到SerializableSet类！");
                return null;
            }

            object set = UnityEngine.ScriptableObject.CreateInstance(t);

            foreach (SheetSource sheet in sheets)
            {
                string fieldName = sheet.sourceName + "s";
                Array configs = Source2Configs(sheet);
                FieldInfo fieldInfo = t.GetField(fieldName);
                fieldInfo.SetValue(set,configs);
            }
            return set;
        }

        /// <summary>
        /// 从源数据反射为对应的配置数组
        /// </summary>
        /// <returns></returns>
        private static Array Source2Configs(SheetSource source)
        {
            Type configType = FindType(source.configName);

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
