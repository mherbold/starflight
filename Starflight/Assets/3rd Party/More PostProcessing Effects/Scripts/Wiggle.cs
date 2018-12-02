/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Wiggle.cs
 * Abstract : Makes the image wiggle.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Wiggle")]
	public class Wiggle : PostEffectsBase
	{
		private float timer = 0;
		// The amplitude of the effect
		public float amplitudeX = 20, amplitudeY = 10;
		// The distortion of the effect
		public float distortionX = 1.0f, distortionY = 1.0f;
		// The speed of the effect
		public float speed = 5.0f;
		
		public Shader wiggleShader = null;
		private Material wiggleMaterial = null;	
		
		public override bool CheckResources ()
		{
			wiggleShader = Shader.Find ("MorePPEffects/Wiggle");
			CheckSupport (false);
			wiggleMaterial = CheckShaderAndCreateMaterial(wiggleShader, wiggleMaterial);
			
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
			wiggleMaterial.SetFloat ("timer", timer);
			wiggleMaterial.SetFloat ("amplitudeX", amplitudeX);
			wiggleMaterial.SetFloat ("amplitudeY", amplitudeY);
			wiggleMaterial.SetFloat ("distortionX", distortionX);
			wiggleMaterial.SetFloat ("distortionY", distortionY);
			wiggleMaterial.SetFloat ("speed", speed);
			Graphics.Blit (source, destination, wiggleMaterial);
		}
	}
}
