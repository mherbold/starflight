/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : RippleEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Ripple))]
	class RippleEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_waves;
		private SerializedProperty s_distortion;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_waves = s_target.FindProperty ("waves");
			s_distortion = s_target.FindProperty ("distortion");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Adds ripples to the camera.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_waves, new GUIContent("Waves"));
			EditorGUILayout.PropertyField (s_distortion, new GUIContent("Distortion"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}