using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationManager)), CanEditMultipleObjects]
public class GenerationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        
        if (GUILayout.Button("Generate Boxes"))
        {
            (target as GenerationManager)?.GenerateBoxes();
        }
        if (GUILayout.Button("Generate Boats/Pirates"))
        {
            (target as GenerationManager)?.GenerateObjects();
        }
        if (GUILayout.Button("Start Simulation"))
        {
            (target as GenerationManager)?.StartSimulation();
        }
        if (GUILayout.Button("Stop Simulation"))
        {
            (target as GenerationManager)?.StopSimulation();
        }
    }
}
