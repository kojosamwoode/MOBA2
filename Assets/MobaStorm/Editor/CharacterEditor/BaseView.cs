using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public abstract class BaseView<T> : EditorWindow where T : EditorWindow
{
    protected static EditorWindow m_window;
    protected static float m_winWidth;
    protected static float m_winHeight;
    protected static int m_defaultLabelFontSize;

    protected abstract void CreateViews();
    protected abstract void InitializeViews();
    protected abstract void RenderTemplates();
    protected abstract void UpdateTemplateParameters();

    protected static void Setup()
    {
        m_window = GetWindow();
    }

    protected virtual void OnGUI()
    {
        CreateViews();
        InitializeViews();

        if (m_window == null)
        {
            Setup();
            return;
        }
        else
        {
            m_winWidth = m_window.position.width;
            m_winHeight = m_window.position.height;
            UpdateTemplateParameters();
            RenderTemplates();
        }
    }

    private static EditorWindow GetWindow()
    {
        return GetWindow(typeof(T));
    }
}
