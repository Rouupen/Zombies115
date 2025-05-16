using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyTextAreaAttribute))]
public class ReadOnlyTextAreaDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            GUI.enabled = false;

            var text = property.stringValue;
            var style = GUI.skin.textArea;

            float height = style.CalcHeight(new GUIContent(text), position.width);
            position.height = height;

            EditorGUI.TextArea(position, text, style);

            GUI.enabled = true;
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [ReadOnlyTextArea] with string.");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            return EditorGUIUtility.singleLineHeight;

        var text = property.stringValue;
        var style = GUI.skin.textArea;
        float height = style.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth - 40);

        return height + EditorGUIUtility.singleLineHeight + 4;
    }
}