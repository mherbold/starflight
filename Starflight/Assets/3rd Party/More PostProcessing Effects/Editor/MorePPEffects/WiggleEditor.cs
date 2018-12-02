/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : WiggleEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Wiggle))]
	class WiggleEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_amplitudeX;
		private SerializedProperty s_amplitudeY;
		private SerializedProperty s_distortionX;
		private SerializedProperty s_distortionY;
		private SerializedProperty s_speed;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_amplitudeX = s_target.FindProperty ("amplitudeX");
			s_amplitudeY = s_target.FindProperty ("amplitudeY");
			s_distortionX = s_target.FindProperty ("distortionX");
			s_distortionY = s_target.FindProperty ("distortionY");
			s_speed = s_target.FindProperty ("speed");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Makes the camera wiggle.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_amplitudeX, new GUIContent("Amplitude X"));
			EditorGUILayout.PropertyField (s_amplitudeY, new GUIContent("Amplitude Y"));
			EditorGUILayout.PropertyField (s_distortionX, new GUIContent("Distortion X"));
			EditorGUILayout.PropertyField (s_distortionY, new GUIContent("Distortion Y"));
			EditorGUILayout.PropertyField (s_speed, new GUIContent("Speed"));

			s_target.ApplyModifiedProperties();
		}
	}
}