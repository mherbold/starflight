/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : DuoToneEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(DuoTone))]
	class DuoToneEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_color1;
		private SerializedProperty s_color2;
		private SerializedProperty s_minLimit;
		private SerializedProperty s_maxLimit;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_color1 = s_target.FindProperty ("color1");
			s_color2 = s_target.FindProperty ("color2");
			s_minLimit = s_target.FindProperty ("minLimit");
			s_maxLimit = s_target.FindProperty ("maxLimit");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Uses only two colors.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_color1, new GUIContent("Color 1"));
			EditorGUILayout.PropertyField (s_color2, new GUIContent("Color 2"));
			EditorGUILayout.Slider (s_minLimit, 0, 1, "Minimum limit");
			EditorGUILayout.Slider (s_maxLimit, 0, 1, "Maximum limit");
			s_target.ApplyModifiedProperties();
		}
	}
}