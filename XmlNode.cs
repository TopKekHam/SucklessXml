
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Suckless.Xml
{

    public class XmlNode : IEnumerable<XmlNode>
    {

        public XmlNode() { }

        public XmlNode(string tag)
        {
            Tag = tag;
        }

        public XmlNode(string tag, params string[] attrs) : this(tag)
        {
            foreach (var attr in attrs)
            {
                Attributes.Add(attr, "");
            }
        }

        public string Tag { get; set; }
        public bool GotCloseTag { get; set; }
        public bool SelfClosing { get; set; }
        public bool PlaneText { get; set; }
        public bool Comment { get; set; }
        public string Text { get; set; }
        public List<XmlNode> Children { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public void InitChildrenAndAttributes()
        {
            Children = new List<XmlNode>();
            Attributes = new Dictionary<string, string>();
        }

        override public string ToString()
        {
            var sb = new StringBuilder(Tag);
            sb.Append(":");

            foreach (var attr in Attributes)
            {
                sb.Append(attr);
                sb.Append(" ");
            }

            return sb.ToString();
        }

        public IEnumerator<XmlNode> GetEnumerator()
        {
            yield return this;

            if (Children != null)
            {
                foreach (var node in Children)
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
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }

}