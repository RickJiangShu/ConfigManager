namespace ConfigManagerEditor
{
    using System.Collections;
    using System.Xml;
    using System.Collections.Generic;

    public class XmlParser
    {
        public static StructSource Parse(string content,string fileName)
        {
            StructSource source = new StructSource();

            source.content = content;
            source.sourceName = fileName.Substring(0, fileName.LastIndexOf('.')); ;//文件名
            source.className = source.sourceName + "Xml";//类名

            XmlDocument document = new XmlDocument();
            document.LoadXml(content);
            source.obj = (Dictionary<string, object>)ParseNode(document);
            return source;
        }

        private static object ParseNode(XmlNode node)
        {
            //判断是否是对象或数组
            if (IsObject(node))
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                //添加属性字段
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        result.Add(attribute.Name, ConfigTools.ConvertBaseObject(attribute.Value));
                    }
                }

                List<string> addedNodeNames = new List<string>();
                //添加子节点（如果多个同名子节点则是数组）
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType != XmlNodeType.Element)
                        continue;

                    string childName = childNode.Name;
                    if (addedNodeNames.Contains(childName))
                        continue;

                    XmlNodeList sameNameNodes = node.SelectNodes(childName);//相同名字的

                    //数组
                    if (sameNameNodes.Count > 1)
                    {
                        object[] objects = new object[sameNameNodes.Count];
                        for (int i = 0, l = sameNameNodes.Count; i < l; i++)
                        {
                            XmlNode sameNameChild = sameNameNodes[i];
                            objects[i] = ParseNode(sameNameChild);
                        }
                        result.Add(childName, objects);
                    }
                    //对象
                    else
                    {
                        result.Add(childName, ParseNode(childNode));
                    }

                    addedNodeNames.Add(childName);
                }

                return result;
            }
            return ConfigTools.ConvertBaseObject(node.InnerText.Trim());
        }

        /// <summary>
        /// 判断是否是对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool IsObject(XmlNode node)
        {
            if (node.Attributes != null && node.Attributes.Count > 0)
                return true;

            int elementCount = 0;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                    elementCount++;
            }
            return elementCount >= 1;
        }
    }


}
