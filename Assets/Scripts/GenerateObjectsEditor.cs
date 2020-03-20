using UnityEngine;
using UnityEditor;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateObjectsInArea))]
[CanEditMultipleObjects]
public class GenerateObjectsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        
        if (GUILayout.Button("Generate"))
        {
            (target as GenerateObjectsInArea)?.RegenerateObjects();
        }
        if (GUILayout.Button("Clear"))
        {
            (target as GenerateObjectsInArea)?.RemoveChildren();
        }
    }
}
