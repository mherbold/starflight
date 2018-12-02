/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Drunk.cs
 * Abstract : Simulates the view when drunk.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Drunk")]
	public class Drunk : PostEffectsBase
	{
		private float timer;
		// The strength of the effect
		public float strength = 2.0f;
		
		public Shader drunkShader = null;
		private Material drunkMaterial = null;	
		
		public override bool CheckResources ()
		{
			drunkShader = Shader.Find ("MorePPEffects/Drunk");
			CheckSupport (false);
			drunkMaterial = CheckShaderAndCreateMaterial(drunkShader, drunkMaterial);
			
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
			drunkMaterial.SetFloat ("timer", timer);
			drunkMaterial.SetFloat ("strength", strength);
			Graphics.Blit (source, destination, drunkMaterial);
		}
	}
}
