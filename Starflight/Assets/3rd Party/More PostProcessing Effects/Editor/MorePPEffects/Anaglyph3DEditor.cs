/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Anaglyph3DEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Anaglyph3D))]
	class Anaglyph3DEditor : Editor {
		
		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField ("Creates a stereo anaglyph 3D effect.", EditorStyles.miniLabel);
		}
	}
}