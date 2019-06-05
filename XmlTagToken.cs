
namespace Suckless.Xml {

    enum XmlTagTokenType {Key, Value, Equels}
    class XmlTagToken {

        public string Value { get; set; }
        public XmlTagTokenType Type { get; set; }

    }

}