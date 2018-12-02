/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : HeadacheEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Headache))]
	class HeadacheEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_strength;
		private SerializedProperty s_speed;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_strength = s_target.FindProperty ("strength");
			s_speed = s_target.FindProperty ("speed");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Simulates a headache.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_strength, new GUIContent("Strength"));
			EditorGUILayout.PropertyField (s_speed, new GUIContent("Speed"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}