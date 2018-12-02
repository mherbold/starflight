/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : NegativeEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Negative))]
	class NegativeEditor : Editor {
		
		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField ("Returns the negative image of the camera.", EditorStyles.miniLabel);
		}
	}
}