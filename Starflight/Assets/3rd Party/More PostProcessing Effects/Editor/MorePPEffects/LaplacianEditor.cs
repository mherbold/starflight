/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : LaplacianEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(Laplacian))]
	class LaplacianEditor : Editor {

		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField ("Returns the laplacian of the camera.", EditorStyles.miniLabel);
		}
	}
}