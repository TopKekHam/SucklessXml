

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Suckless.Xml
{

    public static class XmlDocExtantions
    {

        public static XmlDoc ToXmlDoc(this object obj)
        {
            if (obj == null)
                return null;

            var doc = new XmlDoc();

            doc.RootNodes.Add(MakeNode(obj));

            return doc;
        }

        static XmlNode MakeNode(object obj, string nodeName = null)
        {
            XmlNode node;

            if (obj is IEnumerable list)
            {
                if (nodeName != null)
                {
                    node = new XmlNode(nodeName);
                }
                else
                {
                    return null;
                }

                node.Children = new List<XmlNode>();

                foreach (var childObj in list)
                {
                    node.Children.Add(MakeNode(childObj, obj.GetType().Name));
                }
            }
            else
            {
                node = new XmlNode(obj.GetType().Name);
            }

            FillNode(node, obj);

            return node;
        }

        static void FillNode(XmlNode node, object obj)
        {
            var attrs = obj.GetType().GetRuntimeFields();

            foreach (var attr in attrs)
            {
                var type = attr.GetValue(obj).GetType();

                if (type.IsClass || type is IEnumerable)
                {
                    if (node.Children == null)
                    {
                        node.Children = new List<XmlNode>();
                    }

                    node.Children.Add(MakeNode(attr.GetValue(obj), attr.Name));
                }
                else
                {
                    if (node.Attributes == null)
                    {
                        node.Attributes = new Dictionary<string, string>();
                    }

                    node.Attributes.Add(attr.Name, attr.GetValue(obj).ToString());
                }
            }

        }

    }

}