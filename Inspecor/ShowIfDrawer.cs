using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
public class ShowIfAttributeDrawer : PropertyDrawer
{
    #region Reflection helper
    private static MethodInfo GetMethod(object target, string methodName)
    {
        return GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.InvariantCulture)).FirstOrDefault();
    }

    private static FieldInfo GetField(object target, string fieldName)
    {
        return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
    }

    private static PropertyInfo GetProperty(object target, string propertyName)
    {
        return GetAllPropertyInfo(target, f => f.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();
    }

    private static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
    {
        List<Type> types = new List<Type>()
        {
        target.GetType()
        };

        while (types.Last().BaseType != null)
        {
            types.Add(types.Last().BaseType);
        }

        for (int i = types.Count - 1; i >= 0; i--)
        {
            IEnumerable<FieldInfo> fieldInfos = types[i]
                .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(predicate);

            foreach (var fieldInfo in fieldInfos)
            {
                yield return fieldInfo;
            }
        }

    }

    private static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
    {
        IEnumerable<MethodInfo> methodInfos = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(predicate);

        return methodInfos;
    }

    private static IEnumerable<PropertyInfo> GetAllPropertyInfo(object target, Func<PropertyInfo, bool> predicate)
    {
        IEnumerable<PropertyInfo> propertyInfos = target.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(predicate);

        return propertyInfos;
    }
    #endregion

    private bool MeetsConditions(SerializedProperty property)
    {
        var showIfAttribute = attribute as ShowIfAttribute;
        var target = property.serializedObject.targetObject;
        List<bool> conditionValues = new List<bool>();

        foreach (var condition in showIfAttribute.Conditions)
        {
            FieldInfo conditionField = GetField(target, condition);         //Method ���� �ش� ���� ã��
            if (conditionField != null && conditionField.FieldType == typeof(bool))
            {
                conditionValues.Add((bool)conditionField.GetValue(target));
            }

            MethodInfo conditionMethod = GetMethod(target, condition);      //Method ���� �ش� ���� ã��
            if (conditionMethod != null && conditionMethod.ReturnType == typeof(bool) && conditionMethod.GetParameters().Length == 0)
            {
                conditionValues.Add((bool)conditionMethod.Invoke(target, null));
            }

            PropertyInfo conditionProperty = GetProperty(target, condition);      //Property ���� �ش� ���� ã��
            if (conditionProperty != null && conditionProperty.PropertyType == typeof(bool) && conditionProperty.GetIndexParameters().Length == 0)
            {
                var method = conditionProperty.GetMethod;
                conditionValues.Add((bool)method.Invoke(target, null));
            }

        }

        if (conditionValues.Count > 0)
        {
            bool met;
            if (showIfAttribute.Operator == ShowIfAttribute.ConditionOperator.And)
            {
                met = true;
                foreach (var value in conditionValues)
                    met = met && value;
            }
            else
            {
                met = false;
                foreach (var value in conditionValues)
                    met = met || value;
            }

            return met;
        }
        else
        {
            Debug.LogError("Invalid boolean condition fields or methods used!");
            return true;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Calcluate the property height, <span class="hljs-keyword">if</span> we don<span class="hljs-string">'t meet the condition and the draw mode is DontDraw, then height will be 0.
        bool meetsCondition = MeetsConditions(property);
        var showIfAttribute = attribute as ShowIfAttribute;


        if (!meetsCondition && showIfAttribute.Action == ShowIfAttribute.ActionOnConditionFail.DontDraw)
            return 0;

        if (property.isExpanded)
            return EditorGUI.GetPropertyHeight(property);
        else
            return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool meetsCondition = MeetsConditions(property);
        // Early out, if conditions met, draw and go.

        SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");

        Debug.Log($"[GetPropertyHeight] {property.propertyPath} : {meetsCondition}");
        if (meetsCondition)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        var showIfAttribute = attribute as ShowIfAttribute;
        if (showIfAttribute.Action == ShowIfAttribute.ActionOnConditionFail.DontDraw)
        {
            return;
        }
        else if (showIfAttribute.Action == ShowIfAttribute.ActionOnConditionFail.JustDisable)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif