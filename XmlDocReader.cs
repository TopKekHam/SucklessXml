using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Suckless.Xml.Interfaces;

namespace Suckless.Xml
{

    public class XmlDocReader : IXmlDocReader
    {
        public bool ReadWithComments { get; set; } = false;
        IXmlContentReader _contentReader;
        public XmlDocReader()
        {
            _contentReader = new XmlContentReader();
        }

        // API Static Call | takes file path source and creates XmlDoc from it.
        public XmlDoc ReadFile(string path)
        {
            string source = File.ReadAllText(path);

            return Read(source);
        }

        // API Call | takes file string source and creates XmlDoc from it.
        public XmlDoc Read(string source)
        {
            List<XmlContent> list;

            list = _contentReader.Read(source);

            return ParseContent(list);
        }

        XmlDoc ParseContent(List<XmlContent> list)
        {
            var doc = new XmlDoc();

            while (list.Any())
            {
                var node = ParseNode(list);

                if (node != null)
                {
                    doc.RootNodes.Insert(0, node);
                }
            }

            return doc;
        }

        // Takes the stack of all content and returns the next XmlNode in XmlDoc, removes XmlContnet in the process.
        // If no node found returns NULL.
        XmlNode ParseNode(List<XmlContent> list)
        {
            var tag = list.PopBottom();

            XmlNode node = null;

            if (ReadWithComments == false)
            {
                while (tag.Type == XmlTagType.Comment)
                {
                    if (list.Any())
                    {
                        tag = list.PopBottom();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            if (tag.Type == XmlTagType.CloseTag)
            {
                node = ParseCloseTag(list, tag);
            }

            if (tag.Type == XmlTagType.PlaneText)
            {
                node = ParsePlanteTextNode(tag.Body);
            }

            if (tag.Type == XmlTagType.Comment)
            {
                node = ParseCommentNode(tag.Body);
            }

            if (tag.Type == XmlTagType.SelfClosedTag
             || tag.Type == XmlTagType.OpenTag)
            {
                node = CreateNodeFromContent(tag);
            }

            return node;
        }

        // Takes the close tag XmlContent and list of all XmlContent, and returns the XmlNode with children.
        // Removes all XmlContent that uses, children XmlContent will be removed too. 
        XmlNode ParseCloseTag(List<XmlContent> list, XmlContent tag)
        {
            // var idx = list.FirstIndex(xContent => xContent.Type == XmlTagType.OpenTag
            //              && xContent.Name == tag.Name);

            var idx = GetOpenTagIndex(list, tag);

            if (idx == -1)
            {
                throw new Exception($"Not open tag pair found for close tag({tag.Body})");
            }

            XmlNode node;

            var tempContent = list.PopAt(idx);
            node = CreateNodeFromContent(tempContent);
            node.GotCloseTag = true;

            var childContentList = new List<XmlContent>(list.Skip(idx));

            if (childContentList.Any())
            {
                node.InitChildrenAndAttributes();

                while (childContentList.Any())
                {
                    var childNode = ParseNode(childContentList);
                    if (childNode != null)
                    {
                        node.Children.Insert(0, childNode);
                    }
                }
            }

            list.RemoveRange(idx, list.Count() - idx);

            return node;
        }

        // Takes list of XmlContent and XmlContent close tag and returns the XmlContent pair index.
        // Returns -1 if not found.
        int GetOpenTagIndex(List<XmlContent> list, XmlContent tag)
        {
            int idx = list.Count;
            int counter = 1;

            while (counter > 0)
            {
                idx--;

                if (idx < 0)
                {
                    break;
                }

                if (list[idx].Name == tag.Name)
                {
                    if (list[idx].Type == XmlTagType.OpenTag)
                    {
                        counter--;
                    }
                    else if (list[idx].Type == XmlTagType.CloseTag)
                    {
                        counter++;
                    }
                }
            }

            return idx;
        }

        //Takes string(XmlContent.Body) and creates PlaneText XmlNode from it. 
        XmlNode ParsePlanteTextNode(string body)
        {
            return new XmlNode("text")
            {
                PlaneText = true,
                Text = body
            };
        }

        //Takes string(XmlContent.Body) and creates Comment XmlNode from it. 
        XmlNode ParseCommentNode(string body)
        {
            return new XmlNode("comment")
            {
                Comment = true,
                Text = body
            };
        }

        // Takes XmlContent and creates XmlNode with all properties except Children.
        XmlNode CreateNodeFromContent(XmlContent content)
        {
            XmlNode node = new XmlNode();

            if (content.Type == XmlTagType.PlaneText)
            {
                node.PlaneText = true;
                node.Tag = "text";
                node.Text = content.Body;
            }
            else
            {
                node.Tag = content.Name;
                node.Attributes = content.Attributes;

                if (content.Type == XmlTagType.SelfClosedTag)
                {
                    node.SelfClosing = true;
                }
            }

            return node;
        }

    }

}