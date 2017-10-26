using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public abstract class BaseTemplate
{
    protected Color m_defaultColor = GUI.color;
    protected float m_windowWidth;
    protected float m_windowHeight;
    protected EditorWindow m_parent;
    protected Vector2 m_scrollPosition = Vector2.zero;
    public bool Initialized;

    public BaseTemplate()
    {

    }

    public virtual void Initialize()
    {
        Initialized = true;
    }

    public virtual void Destroy()
    {
        Initialized = false;
    }

    protected virtual void DefaultRegionContent()
    {

    }

    public void UpdateViewParameters(float windowWidth, float windowHeight, EditorWindow parent)
    {
        m_windowWidth = windowWidth;
        m_windowHeight = windowHeight;
        m_parent = parent;
    }

    public virtual void Render()
    {

    }

    protected void DrawBox(Color color, Rect size, Color resetColor)
    {
        GUI.color = color;
        GUI.Box(size, "");
        GUI.color = resetColor;
    }

    protected void DrawBox(GUIStyle boxStyle, Rect size)
    {
        GUI.Box(size, "", boxStyle);
    }

    protected void DrawRegionWithDefaultContent(float x, float y, float width, float height, Color regionColor, bool ignoreBoxBackground = false)
    {
        DrawRegion(x, y, width, height, DefaultRegionContent, regionColor, ignoreBoxBackground);
    }

    protected void DrawRegion(float x, float y, float width, float height, Action regionContent, Color regionColor, bool ignoreBoxBackground = false)
    {
        if(!ignoreBoxBackground)
        {
            DrawBox(regionColor, new Rect(x, y, width, height), m_defaultColor);
        }

        GUILayout.BeginArea(new Rect(x, y, width, height));

        regionContent();

        GUILayout.EndArea();
    }

    protected void DrawScrolleableRegionWithDefaultContent(float x, float y, float width, float height, Color regionColor, float scrollWidth, float scrollHeight, bool ignoreBoxBackground = false, GUIStyle scrollStyle = null)
    {
        DrawScrolleableRegion(x, y, width, height, DefaultRegionContent, regionColor, scrollWidth, scrollHeight, ignoreBoxBackground, scrollStyle);
    }

    protected void DrawScrolleableRegion(float x, float y, float width, float height, Action regionContent, Color regionColor, float scrollWidth, float scrollHeight, bool ignoreBoxBackground = false, GUIStyle scrollStyle = null)
    {
        if(!ignoreBoxBackground)
        {
            DrawBox(regionColor, new Rect(x, y, width, height), m_defaultColor);
        }

        GUILayout.BeginArea(new Rect(x, y, width, height));

        if(scrollStyle != null)
        {
            scrollStyle.fixedWidth = scrollWidth;
            scrollStyle.fixedHeight = scrollHeight;
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, scrollStyle);
        }
        else
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(scrollWidth), GUILayout.Height(scrollHeight));
        }

        regionContent();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    protected GUIStyle CreateTextStyle(GUIStyle templateStyle, TextAnchor alignment, FontStyle fontStyle, int fontSize, Color textColor, float boxWidth = 0, float boxHeight = 0, Func<GUIStyle, GUIStyle> extras = null)
    {
        GUIStyle style = new GUIStyle(templateStyle);
        style.alignment = alignment;
        style.fontStyle = fontStyle;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;

        if (boxWidth != 0)
        {
            style.fixedWidth = boxWidth;
        }
        if (boxHeight != 0)
        {
            style.fixedHeight = boxHeight;
        }
        if (extras != null)
        {
            style = extras(style);
        }
        return style;
    }

    protected GUIStyle CreateElementStyle(GUIStyle templateStyle, Texture2D backgroundTexture, float boxWidth = 0, float boxHeight = 0)
    {
        GUIStyle style = new GUIStyle(templateStyle);
        style.normal.background = backgroundTexture;
        return style;
    }
}
