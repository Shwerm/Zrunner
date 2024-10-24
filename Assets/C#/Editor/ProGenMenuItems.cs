using UnityEditor;
using UnityEngine;

public class ProGenMenuItems
{
    [MenuItem("Assets/Create/ProGen/Configuration")]
    public static void CreateProGenConfig()
    {
        var asset = ScriptableObject.CreateInstance<ProGenConfiguration>();
        
        AssetDatabase.CreateAsset(asset, "Assets/ProGenConfiguration.asset");
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
