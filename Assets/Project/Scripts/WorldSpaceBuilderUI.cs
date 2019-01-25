using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldSpace))]
public class WorldSpaceBuilderUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        WorldSpace space = (WorldSpace)target;
        if(GUILayout.Button("Build Space"))
        {
            space.BuildSpace();
        }
        else if(GUILayout.Button("Wipe Space"))
        {
            space.WipeSpace();
        }
    }
}