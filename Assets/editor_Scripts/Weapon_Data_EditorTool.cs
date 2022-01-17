using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponStats))]
public class Weapon_Data_EditorTool : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDamageRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weapon_Type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("valid_Weapon_Slot"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("firing_Modes"));

        serializedObject.ApplyModifiedProperties();
    }
}
