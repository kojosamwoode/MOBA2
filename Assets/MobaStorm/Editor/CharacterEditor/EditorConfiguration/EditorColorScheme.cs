using UnityEngine;
using System.Collections.Generic;
using System;

public class EditorColorScheme
{
    private EditorConfiguration.EEditorColorSchemes m_type;
    public EditorConfiguration.EEditorColorSchemes Type { get { return m_type; } set { m_type = value; } }

    private Color m_defaultTextColor;
    public Color DefaultTextColor { get { return m_defaultTextColor; } set { m_defaultTextColor = value; } }

    private Color m_linkTextColor;
    public Color LinkTextColor { get { return m_linkTextColor; } set { m_linkTextColor = value; } }

    private Color m_buttonTextColor;
    public Color ButtonTextColor { get { return m_buttonTextColor; } set { m_buttonTextColor = value; } }

    private Color m_importantTextColor;
    public Color ImportantTextColor { get { return m_importantTextColor; } set { m_importantTextColor = value; } }

    private Color m_errorTextcolor;
    public Color ErrorTextColor { get { return m_errorTextcolor; } set { m_errorTextcolor = value; } }

    private Color m_headerBackgroundColor;
    public Color HeaderBackgroundColor { get { return m_headerBackgroundColor; } set { m_headerBackgroundColor = value; } }

    private Color m_bodyBackgroundColor;
    public Color BodyBackgroundColor { get { return m_bodyBackgroundColor; } set { m_bodyBackgroundColor = value; } }

    private Color m_bodyBackgroundColor2;
    public Color BodyBackgroundColor2 { get { return m_bodyBackgroundColor2; } set { m_bodyBackgroundColor2 = value; } }

    private Color m_listBackgroundColor;
    public Color ListBackgroundColor { get { return m_listBackgroundColor; } set { m_listBackgroundColor = value; } }

    private Color m_listHeaderBackgroundColor;
    public Color ListHeaderBackgroundColor { get { return m_listHeaderBackgroundColor; } set { m_listHeaderBackgroundColor = value; } }

    private Color m_boxBackgroundColor;
    public Color BoxBackgroundColor { get { return m_boxBackgroundColor; } set { m_boxBackgroundColor = value; } }

    private Color m_errorBoxBackgroundColor;
    public Color ErrorBoxBackgroundColor { get { return m_errorBoxBackgroundColor;  } set { m_errorBoxBackgroundColor = value; } }

    public EditorColorScheme()
    {

    }

    public EditorColorScheme(EditorColorScheme colorScheme)
    {
        m_type = colorScheme.Type;
        m_defaultTextColor = colorScheme.DefaultTextColor;
        m_linkTextColor = colorScheme.LinkTextColor;
        m_buttonTextColor = colorScheme.ButtonTextColor;
        m_importantTextColor = colorScheme.ImportantTextColor;
        m_errorTextcolor = colorScheme.ErrorTextColor;
        m_headerBackgroundColor = colorScheme.HeaderBackgroundColor;
        m_bodyBackgroundColor = colorScheme.BodyBackgroundColor;
        m_bodyBackgroundColor2 = colorScheme.BodyBackgroundColor2;
        m_listBackgroundColor = colorScheme.ListBackgroundColor;
        m_listHeaderBackgroundColor = colorScheme.ListHeaderBackgroundColor;
        m_boxBackgroundColor = colorScheme.BoxBackgroundColor;
        m_errorBoxBackgroundColor = colorScheme.ErrorBoxBackgroundColor;
    }

    public EditorColorScheme(Dictionary<string, object> colorSchemeData)
    {
        Deserialize(colorSchemeData);
    }

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        dic.Add("type", m_type.ToString());
        SerializeColor("defaultTextColor", m_defaultTextColor, ref dic);
        SerializeColor("linkTextColor", m_linkTextColor, ref dic);
        SerializeColor("buttonTextColor", m_buttonTextColor, ref dic);
        SerializeColor("importantTextColor", m_importantTextColor, ref dic);
        SerializeColor("errorTextColor", m_errorTextcolor, ref dic);
        SerializeColor("headerBackgroundColor", m_headerBackgroundColor, ref dic);
        SerializeColor("bodyBackgroundColor", m_bodyBackgroundColor, ref dic);
        SerializeColor("bodyBackgroundColor2", m_bodyBackgroundColor2, ref dic);
        SerializeColor("listBackgroundColor", m_listBackgroundColor, ref dic);
        SerializeColor("listHeaderBackgroundColor", m_listHeaderBackgroundColor, ref dic);
        SerializeColor("boxBackgroundColor", m_boxBackgroundColor, ref dic);
        SerializeColor("errorBoxBackgroundColor", m_errorBoxBackgroundColor, ref dic);

        return dic;
    }

    private void SerializeColor(string key, Color color, ref Dictionary<string, object> dic)
    {
        dic.Add(key + "-r", color.r);
        dic.Add(key + "-g", color.g);
        dic.Add(key + "-b", color.b);
        dic.Add(key + "-a", color.a);
    }

    private void Deserialize(Dictionary<string, object> colorData)
    {
        m_type = (EditorConfiguration.EEditorColorSchemes)Enum.Parse(typeof(EditorConfiguration.EEditorColorSchemes), (string)colorData["type"], true);
        m_defaultTextColor = DeserializeColor("defaultTextColor", colorData);
        m_linkTextColor = DeserializeColor("linkTextColor", colorData);
        m_buttonTextColor = DeserializeColor("buttonTextColor", colorData);
        m_importantTextColor = DeserializeColor("importantTextColor", colorData);
        m_errorTextcolor = DeserializeColor("errorTextColor", colorData);
        m_headerBackgroundColor = DeserializeColor("headerBackgroundColor", colorData);
        m_bodyBackgroundColor = DeserializeColor("bodyBackgroundColor", colorData);
        m_bodyBackgroundColor2 = DeserializeColor("bodyBackgroundColor2", colorData);
        m_listBackgroundColor = DeserializeColor("listBackgroundColor", colorData);
        m_listHeaderBackgroundColor = DeserializeColor("listHeaderBackgroundColor", colorData);
        m_boxBackgroundColor = DeserializeColor("boxBackgroundColor", colorData);
        m_errorBoxBackgroundColor = DeserializeColor("errorBoxBackgroundColor", colorData);
    }

    private Color DeserializeColor(string key, Dictionary<string, object> colorData)
    {
        Color color = new Color();
        float colorRed = (float)Convert.ToDouble(colorData[key + "-r"]);
        float colorGreen = (float)Convert.ToDouble(colorData[key + "-g"]);
        float colorBlue = (float)Convert.ToDouble(colorData[key + "-b"]);
        float colorAlpha = (float)Convert.ToDouble(colorData[key + "-a"]);
        color.r = colorRed;
        color.g = colorGreen;
        color.b = colorBlue;
        color.a = colorAlpha;
        return color;
    }

    public override string ToString()
    {
        return "Type: " + m_type + " \n" +
               "Default Text Color (r) " + m_defaultTextColor.r + " (g) " + m_defaultTextColor.g + " (b) " + m_defaultTextColor.b + " (a) " + m_defaultTextColor.a + " \n" +
               "Link Text Color (r) " + m_linkTextColor.r + " (g) " + m_linkTextColor.g + " (b) " + m_linkTextColor.b + " (a) " + m_linkTextColor.a + " \n" +
               "Button Text Color (r) " + m_buttonTextColor.r + " (g) " + m_buttonTextColor.g + " (b) " + m_buttonTextColor.b + " (a) " + m_buttonTextColor.a + " \n" +
               "Important Text Color (r) " + m_importantTextColor.r + " (g) " + m_importantTextColor.g + " (b) " + m_importantTextColor.b + " (a) " + m_importantTextColor.a + " \n" +
               "Error Text Color (r) " + m_errorTextcolor.r + " (g) " + m_errorTextcolor.g + " (b) " + m_errorTextcolor.b + " (a) " + m_errorTextcolor.a + " \n" +
               "Header Background Color (r) " + m_headerBackgroundColor.r + " (g) " + m_headerBackgroundColor.g + " (b) " + m_headerBackgroundColor.b + " (a) " + m_headerBackgroundColor.a + " \n" +
               "Body Background Color (r) " + m_bodyBackgroundColor.r + " (g) " + m_bodyBackgroundColor.g + " (b) " + m_bodyBackgroundColor.b + " (a) " + m_bodyBackgroundColor.a + " \n" +
               "Body Background Color 2 (r) " + m_bodyBackgroundColor2.r + " (g) " + m_bodyBackgroundColor2.g + " (b) " + m_bodyBackgroundColor2.b + " (a) " + m_bodyBackgroundColor2.a + " \n" +
               "List Background Color (r) " + m_listBackgroundColor.r + " (g) " + m_listBackgroundColor.g + " (b) " + m_listBackgroundColor.b + " (a) " + m_listBackgroundColor.a + " \n" +
               "List Header Background Color (r) " + m_listHeaderBackgroundColor.r + " (g) " + m_listHeaderBackgroundColor.g + " (b) " + m_listHeaderBackgroundColor.b + " (a) " + m_listHeaderBackgroundColor.a + " \n" +
               "Box Background Color (r) " + m_boxBackgroundColor.r + " (g) " + m_boxBackgroundColor.g + " (b) " + m_boxBackgroundColor.b + " (a) " + m_boxBackgroundColor.a + " \n" +
               "Error Box Background Color (r) " + m_errorBoxBackgroundColor.r + " (g) " + m_errorBoxBackgroundColor.g + " (b) " + m_errorBoxBackgroundColor.b + " (a) " + m_errorBoxBackgroundColor.a;
    }
}
