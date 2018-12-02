/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Colorization.cs
 * Abstract : Recolors the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Colorization")]
	public class Colorization : PostEffectsBase
	{
		// The different channels of color
		public float Rchannel = 0.7f, Gchannel = 0.7f, Bchannel = 0.7f;
		
		public Shader colorizationShader = null;
		private Material colorizationMaterial = null;	
		
		public override bool CheckResources ()
		{
			colorizationShader = Shader.Find ("MorePPEffects/Colorization");
			CheckSupport (false);
			colorizationMaterial = CheckShaderAndCreateMaterial(colorizationShader, colorizationMaterial);
			
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
			
			colorizationMaterial.SetFloat ("Rchannel", Rchannel);
			colorizationMaterial.SetFloat ("Gchannel", Gchannel);
			colorizationMaterial.SetFloat ("Bchannel", Bchannel);
			Graphics.Blit (source, destination, colorizationMaterial);
		}
	}
}
