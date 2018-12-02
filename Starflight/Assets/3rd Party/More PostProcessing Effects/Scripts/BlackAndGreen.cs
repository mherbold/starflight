/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : BlackAndGreen.cs
 * Abstract : Only show in grayscale with green color.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Black and Green")]
	public class BlackAndGreen : PostEffectsBase
	{
		// The smoothness of the effect
		public float smoothness = 0.25f;
		
		public Shader blackAndGreenShader = null;
		private Material blackAndGreenMaterial = null;	
		
		public override bool CheckResources ()
		{
			blackAndGreenShader = Shader.Find ("MorePPEffects/BlackAndGreen");
			CheckSupport (false);
			blackAndGreenMaterial = CheckShaderAndCreateMaterial(blackAndGreenShader, blackAndGreenMaterial);
			
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
			
			blackAndGreenMaterial.SetFloat ("smoothness", smoothness);
			Graphics.Blit (source, destination, blackAndGreenMaterial);
		}
	}
}
