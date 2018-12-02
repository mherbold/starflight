/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : WavesEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Waves))]
	class WavesEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_strengthX;
		private SerializedProperty s_strengthY;
		private SerializedProperty s_frequencyX;
		private SerializedProperty s_frequencyY;
		private SerializedProperty s_speed;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_strengthX = s_target.FindProperty ("strengthX");
			s_strengthY = s_target.FindProperty ("strengthY");
			s_frequencyX = s_target.FindProperty ("frequencyX");
			s_frequencyY = s_target.FindProperty ("frequencyY");
			s_speed = s_target.FindProperty ("speed");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Creates waves.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_strengthX, new GUIContent("Horizontal Strength"));
			EditorGUILayout.PropertyField (s_strengthY, new GUIContent("Vertical Strength"));
			EditorGUILayout.PropertyField (s_frequencyX, new GUIContent("Horizontal Frequency"));
			EditorGUILayout.PropertyField (s_frequencyY, new GUIContent("Vertical Frequency"));
			EditorGUILayout.PropertyField (s_speed, new GUIContent("Speed"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}