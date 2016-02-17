using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ControlSelector))]
public class ControlSelectorInspector : Editor {

    int index = 0;
    string[] options = new string[]
    {
        "None",
        "Linear",
        "Dough"
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ControlSelector cs = target as ControlSelector;

        index = System.Array.IndexOf(options, cs.controlType);

        Rect r = EditorGUILayout.BeginHorizontal();
        index = EditorGUILayout.Popup("Control Type:", index, options, EditorStyles.popup);
        EditorGUILayout.EndHorizontal();

        
        cs.controlType = options[index];

        EditorUtility.SetDirty(target);

    }

    
}
