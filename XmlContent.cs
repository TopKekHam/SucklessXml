
using System.Collections.Generic;

namespace Suckless.Xml
{
    enum XmlTagType { Comment, OpenTag, CloseTag, PlaneText, SelfClosedTag }
    class XmlContent
    {
        public string Body { get; set; }
        public XmlTagType Type { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public void InitAttributes()
        {
            Attributes = new Dictionary<string, string>();
        }
    }
}