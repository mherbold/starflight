/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Posterization.cs
 * Abstract : Applies a posterization effect on camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Posterization")]
	public class Posterization : PostEffectsBase
	{
		// The different variations of tones
		public float tonesAmount = 25.0f;
		// The gamma factor
		public float gamma = 0.5f;
		
		public Shader posterizationShader = null;
		private Material posterizationMaterial = null;	
		
		public override bool CheckResources ()
		{
			posterizationShader = Shader.Find ("MorePPEffects/Posterization");
			CheckSupport (false);
			posterizationMaterial = CheckShaderAndCreateMaterial(posterizationShader, posterizationMaterial);
			
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

			posterizationMaterial.SetFloat ("tonesAmount", tonesAmount);
			posterizationMaterial.SetFloat ("gammaFactor", gamma);
			Graphics.Blit (source, destination, posterizationMaterial);
		}
	}
}
