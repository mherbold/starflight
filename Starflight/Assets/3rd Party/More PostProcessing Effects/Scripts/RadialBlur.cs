/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : RadialBlur.cs
 * Abstract : Creates a radial blur.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Radial Blur")]
	public class RadialBlur : PostEffectsBase
	{
		// The strength of the blur
		public float blurStrength = 1.0f;
		// The number of samples
		public int samples = 16;
		// The center of the view (blur will be applied arount the center)
		public float centerX = 0.5f, centerY = 0.5f;

		public Shader radialBlurVisionShader = null;
		private Material radialBlurMaterial = null;	
		
		public override bool CheckResources ()
		{
			radialBlurVisionShader = Shader.Find ("MorePPEffects/RadialBlur");
			CheckSupport (false);
			radialBlurMaterial = CheckShaderAndCreateMaterial(radialBlurVisionShader, radialBlurMaterial);
			
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

			radialBlurMaterial.SetFloat ("blurStrength", blurStrength);
			radialBlurMaterial.SetInt ("samples", samples);
			radialBlurMaterial.SetFloat ("blurCenterX", centerX);
			radialBlurMaterial.SetFloat ("blurCenterY", centerY);
			Graphics.Blit (source, destination, radialBlurMaterial);
		}
	}
}
