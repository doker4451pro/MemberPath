using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MemberPathAttribute))]
public class MemberPathPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);
        var attribute = this.attribute as MemberPathAttribute;
        var value = fieldInfo.GetValue(property.serializedObject.targetObject) as MemberPath;

        var rect = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
        if (value != null)
            ValidateValue(value, attribute.type);
        var displayValue = value == null || !value.path.Any() ? "[Empty]" : string.Join(".", value.path);
        if (EditorGUI.DropdownButton(rect, new GUIContent(displayValue), FocusType.Passive))
        {
            var menu = new GenericMenu();
            PopulateMenuFromType(menu, attribute.type, null, value == null ? null : string.Join("/", value.path), property);
            menu.ShowAsContext();
        }
        EditorGUI.EndProperty();
    }


    private void PopulateMenuFromType(GenericMenu menu, Type type, string accumulatedString, string currentValue, SerializedProperty property)
    {
        foreach (var field in type.GetFields())
        {
            var pathToField = accumulatedString == null ? field.Name : accumulatedString + $"/{field.Name}";
            if (IsBindType(field.FieldType))
            {
                menu.AddItem(new GUIContent(pathToField), currentValue == pathToField, () =>
                {
                    property.serializedObject.Update();
                    var newPath = new MemberPath { path = pathToField.Split('/').ToList() };
                    property.managedReferenceValue = newPath;
                    
                    if (!property.serializedObject.ApplyModifiedProperties())
                    {
                        Debug.LogWarning("No changes");
                    }
                    else
                    {
                        Debug.Log("Changes");
                    }
                });
            }
            else
            {

                PopulateMenuFromType(menu, field.FieldType, pathToField, currentValue, property);
            }
        }
    }

    private bool IsBindType(Type type)
    {
        return type.IsPrimitive;
    }

    private void ValidateValue(MemberPath value, Type type)
    {
        var validPath = new List<string>();
        var currentType = type;
        foreach (var line in value.path)
        {
            var property = currentType.GetField(line);
            if (property != null)
            {
                validPath.Add(line);
                currentType = property.FieldType;
            }
            else
            {

                Debug.LogWarning($"Error while validating memberPath:{string.Join(".", value.path)}");
                break;
            }
        }

        value.path = validPath;
    }
}
