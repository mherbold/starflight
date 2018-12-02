/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Ripple.cs
 * Abstract : Adds ripples to the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Ripple")]
	public class Ripple : PostEffectsBase
	{
		// The intensity of the waves
		public float waves = 4.0f;
		// The strength of the distortion of the waves
		public float distortion = 5f;
		
		public Shader rippleShader = null;
		private Material rippleMaterial = null;	
		
		public override bool CheckResources ()
		{
			rippleShader = Shader.Find ("MorePPEffects/Ripple");
			CheckSupport (false);
			rippleMaterial = CheckShaderAndCreateMaterial(rippleShader, rippleMaterial);
			
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

			rippleMaterial.SetFloat ("waves", waves);
			rippleMaterial.SetFloat ("distortion", distortion);
			Graphics.Blit (source, destination, rippleMaterial);
		}
	}
}
