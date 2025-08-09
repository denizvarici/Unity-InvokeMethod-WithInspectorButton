using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute
{
    // This class can be used to mark methods in Unity's Inspector as buttons.
    // It can be extended with additional functionality if needed.
    // Currently, it serves as a marker for custom editor scripts to identify button methods.
}