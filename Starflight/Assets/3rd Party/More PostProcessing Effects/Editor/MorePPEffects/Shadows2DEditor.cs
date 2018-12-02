/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Shadows2DEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Shadows2D))]
	class Shadows2DEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_shadowStrength;
		private SerializedProperty s_offsetStrength;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_shadowStrength = s_target.FindProperty ("shadowStrength");
			s_offsetStrength = s_target.FindProperty ("offsetStrength");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Creates a 2D shadow of the camera.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_shadowStrength, new GUIContent("Shadow Strength"));
			EditorGUILayout.PropertyField(s_offsetStrength, new GUIContent("Shadow Offset"));

			s_target.ApplyModifiedProperties();
		}
	}
}