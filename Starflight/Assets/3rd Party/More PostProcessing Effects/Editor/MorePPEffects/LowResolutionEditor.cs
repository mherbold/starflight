/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : LowResolutionEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(LowResolution))]
	class LowResolutionEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_pixelsX;
		private SerializedProperty s_pixelsY;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_pixelsX = s_target.FindProperty ("resolutionX");
			s_pixelsY = s_target.FindProperty ("resolutionY");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Simulates a low resolution.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_pixelsX, new GUIContent("Resolution X"));
			EditorGUILayout.PropertyField (s_pixelsY, new GUIContent("Resolution Y"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}