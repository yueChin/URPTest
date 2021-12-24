using System;
using UnityEditor;

public partial class PostProcessClip
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PostProcessClip))]
    public class PostProcessClipEditor : UnityEditor.Editor
    {
        private PostProcessClip m_PostProcessClip;
        private Editor m_ProfileEditor;
        private SerializedProperty m_ProfileProperty;
        private SerializedProperty m_CurveProperty;

        private void OnEnable()
        {
            m_PostProcessClip = new PostProcessClip();
            m_ProfileEditor = Editor.CreateEditor(m_PostProcessClip.Template.Profile);
            m_ProfileProperty = serializedObject.FindProperty("template.profile");
            m_CurveProperty = serializedObject.FindProperty("template.weightCurve");
        }

        private void OnDestroy()
        {
            DestroyImmediate(m_ProfileEditor);
        }

        public override void OnInspectorGUI()
        {
            m_PostProcessClip.Template.Layer = EditorGUILayout.LayerField("Layer",m_PostProcessClip.Template.Layer);
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ProfileProperty);
            EditorGUILayout.PropertyField(m_CurveProperty);
            serializedObject.ApplyModifiedProperties();
            
            m_ProfileEditor?.OnInspectorGUI();
        }
    }
#endif
}
