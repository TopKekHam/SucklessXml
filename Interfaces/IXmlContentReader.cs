
using System.Collections.Generic;

namespace Suckless.Xml
{
    interface IXmlContentReader
    {
        List<XmlContent> Read(string source);
    }
}