/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : NormalMapDistortionEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(NormalMapDistortion))]
	class NormalMapDistortionEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_xSpeed;
		private SerializedProperty s_ySpeed;
		private SerializedProperty s_normalMap;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_xSpeed = s_target.FindProperty ("speedX");
			s_ySpeed = s_target.FindProperty ("speedY");
			s_normalMap = s_target.FindProperty ("normalMap");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Distort the camera according to a normal map.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_normalMap, new GUIContent("Normal Map"));
			EditorGUILayout.PropertyField (s_xSpeed, new GUIContent("Horizontal Speed"));
			EditorGUILayout.PropertyField (s_ySpeed, new GUIContent("Vertical Speed"));
			
			s_target.ApplyModifiedProperties();
		}
	}
}