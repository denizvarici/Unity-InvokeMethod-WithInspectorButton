#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonAttributeDrawer : Editor
{
    private Dictionary<string, object[]> methodInputs = new();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetType = target.GetType();
        var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            if (method.GetCustomAttribute<ButtonAttribute>() == null)
                continue;

            var parameters = method.GetParameters();
            string methodKey = method.Name;

            if (!methodInputs.ContainsKey(methodKey))
                methodInputs[methodKey] = new object[parameters.Length];

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($"[Button] {method.Name}", EditorStyles.boldLabel);

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                Type paramType = param.ParameterType;

                object currentValue = methodInputs[methodKey][i];

                if (paramType == typeof(int))
                    methodInputs[methodKey][i] = EditorGUILayout.IntField(param.Name, currentValue != null ? (int)currentValue : 0);
                else if (paramType == typeof(float))
                    methodInputs[methodKey][i] = EditorGUILayout.FloatField(param.Name, currentValue != null ? (float)currentValue : 0f);
                else if (paramType == typeof(string))
                    methodInputs[methodKey][i] = EditorGUILayout.TextField(param.Name, currentValue != null ? (string)currentValue : "");
                else if (paramType == typeof(bool))
                    methodInputs[methodKey][i] = EditorGUILayout.Toggle(param.Name, currentValue != null ? (bool)currentValue : false);
                else if (paramType == typeof(byte))
                    methodInputs[methodKey][i] = (byte)EditorGUILayout.IntField(param.Name, currentValue != null ? Convert.ToInt32(currentValue) : 0);
                else if (paramType.IsEnum) // ← ENUM desteği
                    methodInputs[methodKey][i] = EditorGUILayout.EnumPopup(param.Name, currentValue != null ? (Enum)currentValue : (Enum)Enum.GetValues(paramType).GetValue(0));
                else
                    EditorGUILayout.LabelField($"{param.Name}: (Unsupported type {paramType.Name})");
            }

            if (GUILayout.Button("Execute"))
            {
                try
                {
                    method.Invoke(target, methodInputs[methodKey]);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Method invocation failed: {ex.Message}");
                }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}
#endif
