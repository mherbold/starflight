/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : ColorizationEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Colorization))]
	class ColorizationEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_Rchannel;
		private SerializedProperty s_Gchannel;
		private SerializedProperty s_Bchannel;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_Rchannel = s_target.FindProperty ("Rchannel");
			s_Gchannel = s_target.FindProperty ("Gchannel");
			s_Bchannel = s_target.FindProperty ("Bchannel");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Recolors the camera.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_Rchannel, new GUIContent("Red Channel"));
			EditorGUILayout.PropertyField (s_Gchannel, new GUIContent("Green Channel"));
			EditorGUILayout.PropertyField (s_Bchannel, new GUIContent("Blue Channel"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}