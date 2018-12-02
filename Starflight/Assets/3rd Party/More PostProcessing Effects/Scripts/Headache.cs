/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Headache.cs
 * Abstract : Simulates a headache effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Headache")]
	public class Headache : PostEffectsBase
	{
		private float timer;

		// The strength of the headache
		public float strength = 1.0f;
		// The speed of the effect
		public float speed = 10f;
		
		public Shader headacheShader = null;
		private Material headacheMaterial = null;	
		
		public override bool CheckResources ()
		{
			headacheShader = Shader.Find ("MorePPEffects/Headache");
			CheckSupport (false);
			headacheMaterial = CheckShaderAndCreateMaterial(headacheShader, headacheMaterial);
			
			if (!isSupported)
				ReportAutoDisable ();
			return isSupported;
		}
		
		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
			if (CheckResources()==false)
			{
				Graphics.Blit (source, destination);
				return;
			}
			
			timer += Time.deltaTime;
			headacheMaterial.SetFloat ("timer", timer);
			headacheMaterial.SetFloat ("strength", strength);
			headacheMaterial.SetFloat ("speed", speed);
			Graphics.Blit (source, destination, headacheMaterial);
		}
	}
}
