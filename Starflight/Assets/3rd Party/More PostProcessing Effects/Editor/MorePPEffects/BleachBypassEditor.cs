/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : BleachBypassEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(BleachBypass))]
	class BleachBypassEditor : Editor {
		
		private SerializedObject s_target;
		private SerializedProperty s_darkness;
		
		public void OnEnable()
		{
			s_target = new SerializedObject (target);
			s_darkness = s_target.FindProperty ("darkness");
		}
		
		public override void OnInspectorGUI()
		{
			s_target.Update ();
			
			EditorGUILayout.LabelField ("Reproduces the dark ambience in films.", EditorStyles.miniLabel);
			EditorGUILayout.PropertyField (s_darkness, new GUIContent("Dark Intensity"));

			s_target.ApplyModifiedProperties();
		}
	}
}