/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : BlackAndGreenEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(BlackAndGreen))]
	class BlackAndGreenEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_smoothness;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_smoothness = s_target.FindProperty ("smoothness");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Only grayscale and green colors are shown.", EditorStyles.miniLabel);
			EditorGUILayout.Slider (s_smoothness, 0.1f, 1.0f, "Smoothness");
			
			s_target.ApplyModifiedProperties();
		}
	}
}