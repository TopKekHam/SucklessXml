using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Suckless.Xml
{

    public static class XmlDocExtantions
    {

        public static XmlNode ToXmlNode(this object obj)
        {
            if (obj == null)
                return null;

            return MakeNode(obj);
        }

        static XmlNode MakeNode(object obj, string nodeName = null)
        {
            XmlNode node;
            var objType  = obj.GetType();
            
            if (obj is IEnumerable list)
            {
                if (nodeName == null)
                {
                    if(objType.IsGenericType)
                    {
                        var itemType = objType.GetGenericArguments()[0]; // use this...   
                        nodeName = $"CollectionOf{itemType.Name}";
                    } 
                    else 
                    {
                        nodeName = "Collection";
                    }
                }

                node = new XmlNode(nodeName);

                node.Children = new List<XmlNode>();

                foreach (var childObj in list)
                {
                    node.Children.Add(MakeNode(childObj, objType.Name));
                }
            }
            else
            {
                node = new XmlNode(objType.Name);
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