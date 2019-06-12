using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Suckless.Xml
{

    public static class XmlNodeExtantions
    {

        public static bool GotChild(this XmlNode node, string tag)
        {
            return node.GotChild(tag, out var o);
        }

        public static bool GotChild(this XmlNode node, string tag, out IEnumerable<XmlNode> nodeOut)
        {
            nodeOut = null;
            List<XmlNode> nodes = null;

            if(node.Children != null) {
                var count = node.Children.Count;

                for (int i = 0; i < count; i++)
                {
                    if(node.Children[i].Tag == tag)
                    {
                        if(nodes == null)
                        {
                            nodes = new List<XmlNode>();
                        }

                        nodes.Add(node.Children[i]);

                        return true;  
                    }
                }
            }

            return false;
        }

        public static bool GotAttribute(this XmlNode node, string attribute)
        {
            return node.GotAttribute(attribute, out var o);
        }

        public static bool GotAttribute(this XmlNode node, string attribute, out string attributeValue) 
        {
            attributeValue = "";

            if(node.Attributes != null)
            {
                if(node.Attributes.ContainsKey(attribute))
                {
                    attributeValue = node.Attributes[attribute];
                    return true;
                }
            }

            return false;
        }

    }

}