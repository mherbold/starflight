/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : LensEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Lens))]
	class LensEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_lensDistortion;
		private SerializedProperty s_cubicDistortion;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_lensDistortion = s_target.FindProperty ("lensDistortion");
			s_cubicDistortion = s_target.FindProperty ("cubicDistortion");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Creates a lens effect.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_lensDistortion, new GUIContent("Lens Distoration"));
			EditorGUILayout.PropertyField (s_cubicDistortion, new GUIContent("Cubic Distoration"));

			s_target.ApplyModifiedProperties();
		}
	}
}