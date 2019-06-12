using System;
using System.Collections.Generic;

namespace Suckless.Xml
{

    class XmlContentReader : IXmlContentReader
    {

        // Takes source and creates list of all XmlContent in the source.
        public List<XmlContent> Read(string source)
        {
            List<XmlContent> content = CreateBaseContent(source.Trim());

            content.ForEach(xContent => GenerateTagFromContent(xContent));

            return content;
        }

        List<XmlContent> CreateBaseContent(string source)
        {
            var idx = 0;
            var start = 0;

            List<XmlContent> content = new List<XmlContent>();
            XmlContent xmlContent;

            while (idx < source.Length)
            {

                if (source[idx] == '<')
                {
                    if (idx != start)
                    {
                        if (IsNotEmpty(source, start, idx - 1))
                        {
                            xmlContent = ParsePlaneText(source, start, idx - 1);
                            content.Add(xmlContent);
                        }

                        start = idx;
                    }

                    if (StartOfComment(source, idx))
                    {
                        while (source[idx] != '>' || EndOfComment(source, idx) == false)
                        {
                            idx++;
                        }

                        xmlContent = ParseCommend(source, start, idx);
                    }
                    else
                    {
                        while (source[idx] != '>')
                        {
                            idx++;
                        }

                        xmlContent = ParseTag(source, start, idx);
                    }

                    content.Add(xmlContent);
                    idx++;
                    start = idx;
                }
                else
                {
                    idx++;
                }

            }

            if (idx != start)
            {
                xmlContent = ParsePlaneText(source, start, idx);
                content.Add(xmlContent);
            }

            return content;
        }

        bool IsNotEmpty(string source, int start, int end)
        {
            return source.FirstIndex(ch => EmptyChar(ch) == false, start, end) != -1;
        }

        bool EmptyChar(char ch)
        {
            return ch == ' ' || ch == '\r' || ch == '\t' || ch == '\n';
        }

        bool StartOfComment(string source, int idx)
        {
            //< ! - -
            return source[idx + 1] == '!' && source[idx + 2] == '-' && source[idx + 3] == '-';
        }

        bool EndOfComment(string source, int idx)
        {
            //- - >
            return source[idx - 2] == '-' && source[idx - 1] == '-';
        }

        // Take agrs and creates XmlContent type of PlainText 
        XmlContent ParsePlaneText(string source, int start, int end)
        {
            var content = CreateBaseXmlContent(source, start, end);
            content.Type = XmlTagType.PlaneText;
            return content;
        }

        XmlContent ParseCommend(string source, int start, int end)
        {
            var content = CreateBaseXmlContent(source, start, end);
            content.Type = XmlTagType.Comment;
            return content;
        }

        // Takes args and creates the XmlContent with the right type and fill's the body.
        XmlContent ParseTag(string source, int start, int end)
        {
            var content = CreateBaseXmlContent(source, start, end);

            if (content.Body[1] == '/')
            {
                content.Type = XmlTagType.CloseTag;
            }
            if (content.Body[content.Body.Length - 2] == '/')
            {
                content.Type = XmlTagType.SelfClosedTag;
            }

            return content;
        }

        // Takes args and creates XmlContent and fill's body. 
        XmlContent CreateBaseXmlContent(string source, int start, int end)
        {
            var l = source.Length;
            return new XmlContent { Body = source.Substring(start, end - start + 1).Trim() , Type = XmlTagType.OpenTag };
        }

        // Checks if the tag is comment.
        bool IsComment(string content)
        {
            if (content.Length < 7)
            {
                return false;
            }

            return StartOfComment(content, 0) && EndOfComment(content, content.Length - 1);
        }

        // Takes XmlContent and creates XmlNode from it, fill's the tag and the attributes. 
        void GenerateTagFromContent(XmlContent content)
        {
            string body;
            try
            {
                body = GetNodeBody(content);
            }
            catch (Exception)
            {
                throw new Exception($"Can't get node body ({content.Body})");
            }

            if (content.Type != XmlTagType.Comment &&
               content.Type != XmlTagType.PlaneText)
            {
                var tokenList = GenerateTagTokens(body);
                FillTagFromTokens(content, tokenList);
            }
        }

        // Takes XmlContent and return the body of the tag by type.
        string GetNodeBody(XmlContent content)
        {
            string body;

            if (content.Type == XmlTagType.SelfClosedTag)
            {
                body = content.Body.Substring(1, content.Body.Length - 3);
            }
            else if (content.Type == XmlTagType.CloseTag)
            {
                body = content.Body.Substring(2, content.Body.Length - 3);
            }
            else if (content.Type == XmlTagType.PlaneText)
            {
                body = content.Body;
            }
            else
            {
                body = content.Body.Substring(1, content.Body.Length - 2);
            }

            return body;
        }

        // Takes tag body and creates list of XmlTagTokens that represents the tag name, and attributes.  
        List<XmlTagToken> GenerateTagTokens(string body)
        {
            var list = new List<XmlTagToken>();

            var idx = 0;
            var nextIdx = 0;
            var token = GetNextToken(body, idx, out nextIdx);
            idx = nextIdx;

            while (token != null)
            {
                list.Add(token);
                token = GetNextToken(body, idx, out nextIdx);
                idx = nextIdx;
            }

            return list;
        }

        // Takes tag body and the starting point creates XmlTagToken that represents next token.
        // Returns NULL if no token found.
        // out END the index of the next char after the current token in body. 
        XmlTagToken GetNextToken(string body, int start, out int end)
        {
            end = -1;
            var idx = body.FirstIndex(ch => EmptyChar(ch) == false, start);
            var bl = body.Length;

            if (idx == -1)
            {
                return null;
            }

            var token = new XmlTagToken();
            var tokenStart = idx;

            if (body[idx] == '"')
            {
                token.Type = XmlTagTokenType.Value;
                idx = body.FirstIndex(ch => ch == '"', ++idx);

                if (idx == -1)
                {
                    string err = body;
                }
            }
            else if (body[idx] == '=')
            {
                token.Type = XmlTagTokenType.Equels;
                idx++;
            }
            else
            {
                token.Type = XmlTagTokenType.Key;
                idx = body.FirstIndex(ch => EmptyChar(ch) || ch == '=', ++idx);

                if (idx == -1)
                {
                    idx = body.Length;
                }
            }

            if (token.Type == XmlTagTokenType.Value)
            {

                token.Value = body.Substring(tokenStart + 1, idx - tokenStart - 1);

                idx++;
            }
            else
            {
                token.Value = body.Substring(tokenStart, idx - tokenStart);
            }

            end = idx;
            return token;
        }

        // Takes XmlContent and the tokens that need to be parsed.
        // Fills the XmlContent by the XmlTagToken passed.
        void FillTagFromTokens(XmlContent content, List<XmlTagToken> tokens)
        {
            if (content.Type == XmlTagType.Comment || content.Type == XmlTagType.PlaneText)
            {
                return;
            }

            if (tokens[0].Type != XmlTagTokenType.Key)
            {
                throw new XmlContentException(content, "Tag name not found");
            }

            content.Name = tokens[0].Value;
            var lastTokenName = "";

            if (tokens.Count > 1)
            {
                content.InitAttributes();
            }

            for (int i = 1; i < tokens.Count; i++)
            {
                if (tokens[i].Type == XmlTagTokenType.Key)
                {
                    lastTokenName = tokens[i].Value;
                    content.Attributes.Add(lastTokenName, null);
                }
                else if (tokens[i].Type == XmlTagTokenType.Equels)
                {
                    i++;

                    if (tokens[i].Type == XmlTagTokenType.Value)
                    {
                        content.Attributes[lastTokenName] = tokens[i].Value;
                        lastTokenName = "";
                    }
                    else
                    {
                        throw new XmlContentException(content, "Value doesnt found after assignment operator");
                    }
                }
                else if (tokens[i].Type == XmlTagTokenType.Value)
                {
                    throw new Exception("Value doesnt have attribute name");
                }
            }

        }
    }

}