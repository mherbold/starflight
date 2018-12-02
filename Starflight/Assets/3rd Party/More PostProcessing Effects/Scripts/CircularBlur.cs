/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : CircularBlur.cs
 * Abstract : Creates a circular blur.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Circular Blur")]
	public class CircularBlur : PostEffectsBase
	{
		// The strength of the blur
		public float strength = 1;
		// The number of samples
		public int samples = 12;

		public Shader circularBlurShader = null;
		private Material circularBlurMaterial = null;	
		
		public override bool CheckResources ()
		{
			circularBlurShader = Shader.Find ("MorePPEffects/CircularBlur");
			CheckSupport (false);
			circularBlurMaterial = CheckShaderAndCreateMaterial(circularBlurShader, circularBlurMaterial);
			
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

			circularBlurMaterial.SetFloat ("strength", strength);
			circularBlurMaterial.SetFloat ("samples", samples);
			Graphics.Blit (source, destination, circularBlurMaterial);
		}
	}
}
