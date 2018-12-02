/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : PosterizationEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Posterization))]
	class PosterizationEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_tonesAmount;
		private SerializedProperty s_gamma;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_tonesAmount = s_target.FindProperty ("tonesAmount");
			s_gamma = s_target.FindProperty ("gamma");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Applies a posterization effect on camera.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_tonesAmount, new GUIContent("Tones Amount"));
			EditorGUILayout.PropertyField (s_gamma, new GUIContent("Gamma Factor"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}