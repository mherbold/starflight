/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : EmbossEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Emboss))]
	class EmbossEditor : Editor {

		private SerializedObject s_target;
		private SerializedProperty s_grayscale;
		private SerializedProperty s_strength;

		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_grayscale = s_target.FindProperty ("grayscale");
			s_strength = s_target.FindProperty ("strength");
		}

		public override void OnInspectorGUI()
		{
			s_target.Update ();

			EditorGUILayout.LabelField ("Reproduce an embossed style.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_strength, new GUIContent("Strength"));
			EditorGUILayout.PropertyField (s_grayscale, new GUIContent("Grayscale"));

			s_target.ApplyModifiedProperties();
		}
	}
}