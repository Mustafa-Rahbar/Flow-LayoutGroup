using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(DynamicFlowLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class DynamicFlowLayoutGroupEditor : Editor
    {
        private SerializedProperty m_Padding;
        private SerializedProperty m_ChildAlignment;
        private SerializedProperty m_StartCorner;
        private SerializedProperty m_StartAxis;
        private SerializedProperty m_CellSize;
        private SerializedProperty m_Spacing;
        private SerializedProperty m_ChildControlWidth;
        private SerializedProperty m_ChildControlHeight;

        private void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_CellSize = serializedObject.FindProperty("m_CellSize"); ;
            m_Spacing = serializedObject.FindProperty("m_Spacing"); ;
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis"); ;
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_ChildControlWidth = serializedObject.FindProperty("m_ChildControlWidth"); ;
            m_ChildControlHeight = serializedObject.FindProperty("m_ChildControlHeight"); ;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(m_Padding, true);

            bool isVertical = m_StartAxis.enumValueIndex == 1;
            Rect controlRect = EditorGUILayout.GetControlRect();
            controlRect = EditorGUI.PrefixLabel(controlRect, EditorGUIUtility.TrTextContent("Cell Size"));
            Vector2Field(controlRect, m_CellSize, isVertical);

            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);

            controlRect = EditorGUILayout.GetControlRect();
            controlRect = EditorGUI.PrefixLabel(controlRect, EditorGUIUtility.TrTextContent("Control Size"));
            controlRect.width = Mathf.Max(50f, (controlRect.width - 4f) / 3f);
            EditorGUIUtility.labelWidth = 50f;
            EditorGUI.BeginDisabledGroup(isVertical);
            ToggleLeft(controlRect, m_ChildControlWidth, EditorGUIUtility.TrTextContent("Width"));
            EditorGUI.EndDisabledGroup();
            controlRect.x += controlRect.width + 2f;
            EditorGUI.BeginDisabledGroup(!isVertical);
            ToggleLeft(controlRect, m_ChildControlHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUI.EndDisabledGroup();
            EditorGUIUtility.labelWidth = 0;
            base.serializedObject.ApplyModifiedProperties();
        }

        private void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            bool boolValue = !property.boolValue;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            boolValue = EditorGUI.ToggleLeft(position, label, boolValue);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = property.hasMultipleDifferentValues || !property.boolValue;
            }

            EditorGUI.EndProperty();
        }

        private void Vector2Field(Rect position, SerializedProperty property, bool isVertical)
        {
            Vector2 vector2Value = property.vector2Value;
            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            float labelWidth = 12f;
            float spacing = 2f;
            float totalSpacing = (labelWidth + 2 * spacing) * 3;

            float floatWidth = (position.width - totalSpacing) / 3f;

            Rect xFieldRect = new Rect(position.x, position.y, floatWidth + labelWidth, position.height);
            Rect yFieldRect = new Rect(xFieldRect.xMax + spacing * 2, position.y, floatWidth + labelWidth, position.height);

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.BeginDisabledGroup(!isVertical);
            vector2Value.x = EditorGUI.FloatField(xFieldRect, EditorGUIUtility.TrTextContent("X"), vector2Value.x);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(isVertical);
            vector2Value.y = EditorGUI.FloatField(yFieldRect, EditorGUIUtility.TrTextContent("Y"), vector2Value.y);
            EditorGUI.EndDisabledGroup();
            EditorGUIUtility.labelWidth = defaultLabelWidth;

            property.vector2Value = vector2Value;
        }
    }
}
