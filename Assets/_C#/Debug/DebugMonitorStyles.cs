using UnityEngine;
using UnityEditor;

public class DebugMonitorStyles
{
    public GUIStyle HeaderStyle { get; private set; }
    public GUIStyle ValueStyle { get; private set; }
    public Color HeaderColor { get; } = Color.white;
    public Color ValueColor { get; } = Color.cyan;

    public void Initialize()
    {
        HeaderStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };
        HeaderStyle.normal.textColor = HeaderColor;
        
        ValueStyle = new GUIStyle();
        ValueStyle.normal.textColor = ValueColor;
    }
}
