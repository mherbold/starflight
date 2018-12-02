/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : CircularBlurEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(CircularBlur))]
	class CircularBlurEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_strength;
		private SerializedProperty s_samples;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_strength = s_target.FindProperty ("strength");
			s_samples = s_target.FindProperty ("samples");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Creates a circular blur.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_strength, new GUIContent("Strength"));
			EditorGUILayout.IntSlider (s_samples, 3, 64, "Samples");

			s_target.ApplyModifiedProperties();
		}
	}
}