/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : RadialBlurEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(RadialBlur))]
	class RadialBlurEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_blurStrength;
		private SerializedProperty s_samples;
		private SerializedProperty s_blurCenterX;
		private SerializedProperty s_blurCenterY;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_blurStrength = s_target.FindProperty ("blurStrength");
			s_samples = s_target.FindProperty ("samples");
			s_blurCenterX = s_target.FindProperty ("centerX");
			s_blurCenterY = s_target.FindProperty ("centerY");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Creates a radial blur.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_blurStrength, new GUIContent("Blur Strength"));
			EditorGUILayout.IntSlider (s_samples, 3, 64, "Samples");
			EditorGUILayout.Slider (s_blurCenterX, 0, 1, "Blur Center X");
			EditorGUILayout.Slider (s_blurCenterY, 0, 1, "Blur Center Y");

			s_target.ApplyModifiedProperties();
		}
	}
}