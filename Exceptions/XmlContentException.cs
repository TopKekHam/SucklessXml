

using System;

namespace Suckless.Xml {

    class XmlContentException : Exception
    {
        public XmlContent Content {get; private set;}

        public XmlContentException(XmlContent content, string message = "") : base(message)
        {
            Content = content;
        }

    }

}