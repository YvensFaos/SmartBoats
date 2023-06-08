using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GenerateObjectsInArea))]
    [CanEditMultipleObjects]
    public class GenerateObjectsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

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
}