/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : ColoredRaysEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(ColoredRays))]
	class ColoredRaysEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_strength;
		private SerializedProperty s_RraysStrength;
		private SerializedProperty s_GraysStrength;
		private SerializedProperty s_BraysStrength;
		private SerializedProperty s_RraysSpeed;
		private SerializedProperty s_GraysSpeed;
		private SerializedProperty s_BraysSpeed;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_strength = s_target.FindProperty ("strength");
			s_RraysStrength = s_target.FindProperty ("RraysStrength");
			s_GraysStrength = s_target.FindProperty ("GraysStrength");
			s_BraysStrength = s_target.FindProperty ("BraysStrength");
			s_RraysSpeed = s_target.FindProperty ("RraysSpeed");
			s_GraysSpeed = s_target.FindProperty ("GraysSpeed");
			s_BraysSpeed = s_target.FindProperty ("BraysSpeed");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Adds rays to the camera.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_strength, new GUIContent("Strength"));
			EditorGUILayout.PropertyField (s_RraysStrength, new GUIContent("Red Strength"));
			EditorGUILayout.PropertyField (s_GraysStrength, new GUIContent("Green Strength"));
			EditorGUILayout.PropertyField (s_BraysStrength, new GUIContent("Blue Strength"));
			EditorGUILayout.PropertyField (s_RraysSpeed, new GUIContent("Red Speed"));
			EditorGUILayout.PropertyField (s_GraysSpeed, new GUIContent("Green Speed"));
			EditorGUILayout.PropertyField (s_BraysSpeed, new GUIContent("Blue Speed"));

			s_target.ApplyModifiedProperties();
		}
	}
}