#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ExtraDry.Highlight;

public class XmlConfiguration 
{
    public Dictionary<string, Definition> Definitions { get; set; }

    public XDocument XmlDocument { get; protected set; }

    public XmlConfiguration(XDocument xmlDocument)
    {
        XmlDocument = xmlDocument;
        Definitions = XmlDocument
                .Descendants("definition")
                .Select(GetDefinition)
                .ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
    }

    private Definition GetDefinition(XElement definitionElement)
    {
        var name = definitionElement.GetAttributeValue("name");
        var patterns = GetPatterns(definitionElement);
        var caseSensitive = bool.Parse(definitionElement.GetAttributeValue("caseSensitive"));
        var style = GetDefinitionStyle(definitionElement);

        return new Definition(name, caseSensitive, style, patterns);
    }

    private IDictionary<string, Pattern> GetPatterns(XContainer definitionElement)
    {
        var patterns = definitionElement
            .Descendants("pattern")
            .Select(GetPattern) 
            .ToDictionary(x => x.Name);

        return patterns;
    }

    private Pattern GetPattern(XElement patternElement)
    {
        const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
        var patternType = patternElement.GetAttributeValue("type");
        if (patternType.Equals("block", stringComparison)) {
            return GetBlockPattern(patternElement);
        }
        if (patternType.Equals("markup", stringComparison)) {
            return GetMarkupPattern(patternElement);
        }
        if (patternType.Equals("word", stringComparison)) {
            return GetWordPattern(patternElement);
        }

        throw new InvalidOperationException(String.Format("Unknown pattern type: {0}", patternType));
    }

    private static BlockPattern GetBlockPattern(XElement patternElement)
    {
        var name = patternElement.GetAttributeValue("name");
        var style = GetPatternStyle(patternElement);
        var beginsWith = patternElement.GetAttributeValue("beginsWith");
        var endsWith = patternElement.GetAttributeValue("endsWith");
        var escapesWith = patternElement.GetAttributeValue("escapesWith");

        return new BlockPattern(name, style, beginsWith, endsWith, escapesWith);
    }

    private static MarkupPattern GetMarkupPattern(XElement patternElement)
    {
        var name = patternElement.GetAttributeValue("name");
        var style = GetPatternStyle(patternElement);
        var highlightAttributes = Boolean.Parse(patternElement.GetAttributeValue("highlightAttributes"));
        var bracketColors = GetMarkupPatternBracketColors(patternElement);
        var attributeNameColors = GetMarkupPatternAttributeNameColors(patternElement);
        var attributeValueColors = GetMarkupPatternAttributeValueColors(patternElement);

        return new MarkupPattern(name, style, highlightAttributes, bracketColors, attributeNameColors, attributeValueColors);
    }

    private static WordPattern GetWordPattern(XElement patternElement)
    {
        var name = patternElement.GetAttributeValue("name");
        var style = GetPatternStyle(patternElement);
        var words = GetPatternWords(patternElement);

        return new WordPattern(name, style, words);
    }

    private static IEnumerable<string> GetPatternWords(XContainer patternElement)
    {
        var words = new List<string>();
        var wordElements = patternElement.Descendants("word");
        if (wordElements != null) {
            words.AddRange(from wordElement in wordElements select Regex.Escape(wordElement.Value));
        }

        return words;
    }

    private static Style GetPatternStyle(XContainer patternElement)
    {
        var fontElement = patternElement.Descendants("font").Single();
        var colors = GetPatternColors(fontElement);
        var font = GetPatternFont(fontElement);

        return new Style(colors, font);
    }

    private static ColorPair GetPatternColors(XElement fontElement)
    {
        var foreColor = Color.FromName(fontElement.GetAttributeValue("foreColor"));
        var backColor = Color.FromName(fontElement.GetAttributeValue("backColor"));

        return new ColorPair(foreColor, backColor);
    }

    private static Font GetPatternFont(XElement fontElement, Font defaultFont = null)
    {
        var fontFamily = fontElement.GetAttributeValue("name");
        if (fontFamily != null) {
            var emSize = fontElement.GetAttributeValue("size").ToSingle(11f);
            var style = Enum<FontStyle>.Parse(fontElement.GetAttributeValue("style"), FontStyle.Regular, true);

            return new Font { Name = fontFamily, Size = emSize, Style = style };
        }

        return defaultFont;
    }

    private static ColorPair GetMarkupPatternBracketColors(XContainer patternElement)
    {
        const string descendantName = "bracketStyle";
        return GetMarkupPatternColors(patternElement, descendantName);
    }

    private static ColorPair GetMarkupPatternAttributeNameColors(XContainer patternElement)
    {
        const string descendantName = "attributeNameStyle";
        return GetMarkupPatternColors(patternElement, descendantName);
    }

    private static ColorPair GetMarkupPatternAttributeValueColors(XContainer patternElement)
    {
        const string descendantName = "attributeValueStyle";
        return GetMarkupPatternColors(patternElement, descendantName);
    }

    private static ColorPair GetMarkupPatternColors(XContainer patternElement, XName descendantName)
    {
        var fontElement = patternElement.Descendants("font").Single();
        var element = fontElement.Descendants(descendantName).SingleOrDefault();
        if (element != null) {
            var colors = GetPatternColors(element);
            return colors;
        }
        return null;
    }

    private static Style GetDefinitionStyle(XNode definitionElement)
    {
        const string xpath = "default/font";
        var fontElement = definitionElement.XPathSelectElement(xpath);
        var colors = GetDefinitionColors(fontElement);
        var font = GetDefinitionFont(fontElement);

        return new Style(colors, font);
    }

    private static ColorPair GetDefinitionColors(XElement fontElement)
    {
        var foreColor = Color.FromName(fontElement.GetAttributeValue("foreColor"));
        var backColor = Color.FromName(fontElement.GetAttributeValue("backColor"));

        return new ColorPair(foreColor, backColor);
    }

    private static Font GetDefinitionFont(XElement fontElement)
    {
        var fontName = fontElement.GetAttributeValue("name");
        var fontSize = Convert.ToSingle(fontElement.GetAttributeValue("size"));
        var fontStyle = (FontStyle) Enum.Parse(typeof(FontStyle), fontElement.GetAttributeValue("style"), true);
         
        return new Font {  Name = fontName, Size = fontSize, Style = fontStyle };
    }
}
