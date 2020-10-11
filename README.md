# SucklessXml - Working on XML files with LINQ.

## How to use Suckless.Xml
### Reading file
You can do it in 4 different ways.

```c#
var xmlString = File.ReadAllText("filepath/file.xml");

//first - reading file.
var xmldoc = XmlDoc.ReadFile("filepath");

//second - reading string.
var xmldoc2 = XmlDoc.ReadFile(xmlString);

// create reader.
var xmlReader = new XmlDocReader();

//do you want to read the comments?
//false by default.
//xmlReader.ReadWithComments = true / false;

//third - read file.
var xmldoc3 = xmlReader.ReadFile("filepath/file.xml");

//fourth - read string.
var xmldoc4 = xmlReader.ReadFile(xmlString);
```

### Working on XmlDoc

XmlDoc contains root nodes and each node contains children nodes.
Each node is XmlNode, XmlNodes contain Attributes too.
If XmlNode dont have children the prop will be null, same goes for attributes.

```c#
    var doc = XmlDoc.ReadFile("file.xml");

    var docRootNodes = doc.RootNodes;
    var firstRootNodeChildren = docRootNodes[0].Children;
    var nameAttribute = firstRootNodes[0].Attributes["name"];
```

Iterating on all doc nodes and counting the amount of nodes with tag "div".

```c#
    var doc = XmlDoc.ReadFile("file.xml");

    var divCount = 0;
    
    foreach(var node in doc) 
    {
        if(node.Tag = "div")
        {
            divCount ++;
        }
    }
```

Because XmlDoc and XmlNode is implementing IEnumerable<Node> you can use c# Linq on it.

We got couple built in methods for ease of use.

XmlNode.GotChild(tagName)

```c#
    var doc = XmlDoc.ReadFile("file.xml");

    var imgFatherNodes = doc.Where(node => node.GotChild("img"));
```

XmlNode.GotAttribute(attributeName)

```c#
    var doc = XmlDoc.ReadFile("file.xml");

    var nodesWithHref = doc.Where(node => node.GotAttribute("href"));
```

## Work In Progress Features

### Serialization
