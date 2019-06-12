
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Suckless.Xml
{

    public class XmlDoc : IEnumerable<XmlNode>
    {

        public static XmlDoc ReadFile(string path)
        {
            return new XmlDocReader().ReadFile(path);
        }

        public static XmlDoc Read(string source)
        {
            return new XmlDocReader().Read(source);
        }

        public void WriteFile(XmlDoc doc, string path)
        {
            XmlDocWriter.WriteFile(doc, path);
        }

        public string Write()
        {
            return new XmlDocWriter().Write(this);
        }

        public XmlDoc()
        {
            RootNodes = new List<XmlNode>();
        }

        public List<XmlNode> RootNodes { get; set; }

        public IEnumerator<XmlNode> GetEnumerator()
        {
            foreach (var node in RootNodes)
            {
                var childNodeEnumerator = node.GetEnumerator();
                childNodeEnumerator.MoveNext();

                while (childNodeEnumerator.Current != null)
                {

                    yield return childNodeEnumerator.Current;
                    var end = childNodeEnumerator.MoveNext();

                    if (end == false)
                    {
                        break;
                    }

                }
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}