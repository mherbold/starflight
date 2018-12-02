/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : LowResolution.cs
 * Abstract : Simulates a low resolution.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Low Resolution")]
	public class LowResolution : PostEffectsBase
	{
		// The resolution represents the number of pixels
		public int resolutionX = 200;
		public int resolutionY = 200;
		
		public Shader lowShader = null;
		private Material lowMaterial = null;	
		
		public override bool CheckResources ()
		{
			lowShader = Shader.Find ("MorePPEffects/LowResolution");
			CheckSupport (false);
			lowMaterial = CheckShaderAndCreateMaterial(lowShader, lowMaterial);
			
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
			
			lowMaterial.SetInt ("pixelsX", resolutionX);
			lowMaterial.SetInt ("pixelsY", resolutionY);
			Graphics.Blit (source, destination, lowMaterial);
		}
	}
}
