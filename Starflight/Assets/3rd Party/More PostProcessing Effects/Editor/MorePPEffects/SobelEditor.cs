/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : SobelEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Sobel))]
	class SobelEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_threshold;
		private SerializedProperty s_edgeColor;
		private SerializedProperty s_showBackground;
		private SerializedProperty s_backgroundColor;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_edgeColor = s_target.FindProperty ("edgeColor");
			s_showBackground = s_target.FindProperty("showBackground");
			s_backgroundColor = s_target.FindProperty("backgroundColor");
			s_threshold = s_target.FindProperty("threshold");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Reproduces a sobel effect.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_threshold, new GUIContent("Threshold"));
			EditorGUILayout.PropertyField (s_edgeColor, new GUIContent("Edge Color"));
			EditorGUILayout.PropertyField (s_showBackground, new GUIContent("Show Background"));
			if (!s_showBackground.boolValue) {
				EditorGUILayout.PropertyField (s_backgroundColor, new GUIContent("Background Color"));
			}

			s_target.ApplyModifiedProperties();
		}
	}
}