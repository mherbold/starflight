/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : ScannerEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Scanner))]
	class ScannerEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_linesIntensity;
		private SerializedProperty s_linesSpeed;
		private SerializedProperty s_linesAmount;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_linesIntensity = s_target.FindProperty ("linesIntensity");
			s_linesSpeed = s_target.FindProperty ("linesSpeed");
			s_linesAmount = s_target.FindProperty ("linesAmount");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Draws scanner lines.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_linesIntensity, new GUIContent("Lines Intensity"));
			EditorGUILayout.PropertyField(s_linesSpeed, new GUIContent("Lines Speed"));
			EditorGUILayout.PropertyField(s_linesAmount, new GUIContent("Lines Amount"));
			s_target.ApplyModifiedProperties();
		}
	}
}