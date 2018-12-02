/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : ThermalVisionEditor.cs
 * 
 * */

using UnityEngine;
using System.Collections;
using UnityEditor;
using MorePPEffects;

namespace MorePPEffects
{
	[CustomEditor(typeof(ThermalVision))]
	class ThermalVisionEditor : Editor {

		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField ("Simulates a thermal vision.", EditorStyles.miniLabel);
		}
	}
}