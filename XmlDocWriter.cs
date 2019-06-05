using System.IO;
using System.Linq;
using System.Text;
using Suckless.Xml.Interfaces;

namespace Suckless.Xml
{
    public class XmlDocWriter : IXmlDocWriter
    {
         // Static API Call | takes XmlDoc and file path and writes file.
        public static void WriteFile(XmlDoc doc, string path)
        {
            File.WriteAllText(path, new XmlDocWriter().Write(doc));
        }

         // API Call | takes XmlDoc and returns data in string.
        public string Write(XmlDoc doc)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var node in doc.RootNodes)
            {
                AppendNode(sb, node);
            }

            return sb.ToString();
        }

        void AppendNode(StringBuilder sb, XmlNode node)
        {
            if (node.PlaneText)
            {
                AppendPlaneText(sb, node);
            }
            else if (node.Comment)
            {
                AppendComment(sb, node);
            }
            else
            {
                AppendOpenTag(sb, node);

                if (node.Children != null)
                {
                    foreach (var childNode in node.Children)
                    {
                        AppendNode(sb, childNode);
                    }
                }

                if (node.GotCloseTag)
                {
                    AppendCloseTag(sb, node);
                }
            }
        }

        void AppendOpenTag(StringBuilder sb, XmlNode node)
        {

            sb.Append($"<{node.Tag}");

            if (node.Attributes != null)
            {
                foreach (var attr in node.Attributes)
                {
                    sb.Append($" {attr.Key}");

                    if (string.IsNullOrEmpty(attr.Value) == false)
                    {
                        sb.Append($"=\"{attr.Value}\"");
                    }
                }
            }

            if (node.SelfClosing)
            {
                sb.AppendLine("/>");
            }
            else
            {
                sb.AppendLine(">");
            }

        }

        void AppendCloseTag(StringBuilder sb, XmlNode node)
        {
            sb.AppendLine($"</{node.Tag}>");
        }

        void AppendComment(StringBuilder sb, XmlNode node)
        {
            sb.AppendLine(node.Text);
        }

        void AppendPlaneText(StringBuilder sb, XmlNode node)
        {
            sb.AppendLine(node.Text);
        }

    }

}