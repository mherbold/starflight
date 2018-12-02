/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : NightVisionEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(NightVision))]
	class NightVisionEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_noiseStrength;
		private SerializedProperty s_linesStrength;
		private SerializedProperty s_linesAmount;
		private SerializedProperty s_amplification;
		private SerializedProperty s_luminosityThreshold;
		private SerializedProperty s_noiseSaturation;
		private SerializedProperty s_textureOffset;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_noiseStrength = s_target.FindProperty ("noiseStrength");
			s_linesStrength = s_target.FindProperty ("linesStrength");
			s_linesAmount = s_target.FindProperty ("linesAmount");
			s_amplification = s_target.FindProperty ("amplification");
			s_luminosityThreshold = s_target.FindProperty ("luminosityThreshold");
			s_noiseSaturation = s_target.FindProperty ("noiseSaturation");
			s_textureOffset = s_target.FindProperty ("textureOffset");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Simulates a night vision effect.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_noiseStrength, new GUIContent("Noise Strength"));
			EditorGUILayout.PropertyField(s_noiseSaturation, new GUIContent("Noise Saturation"));
			EditorGUILayout.Slider (s_linesStrength, 0, 2, "Lines Strength");
			EditorGUILayout.PropertyField (s_linesAmount, new GUIContent("Lines Amount"));
			EditorGUILayout.PropertyField (s_amplification, new GUIContent("Amplification"));
			EditorGUILayout.Slider (s_luminosityThreshold, 0, 1, "Luminosity Threshold");
			EditorGUILayout.PropertyField (s_textureOffset, new GUIContent("Texture Offset"));
			s_target.ApplyModifiedProperties();
		}
	}
}