/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : NightVision.cs
 * Abstract : Simulates a night vision effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Night Vision")]
	public class NightVision : PostEffectsBase
	{
		private float timer;

		// The strength of the noises (may alter luminosity)
		public float noiseStrength = 1;
		// The strength of the vertical lines
		public float linesStrength = 0.5f;
		// The amount of vertical lines
		public int linesAmount = 1000;
		// The amplification performed on pixels beyond luminosity threshold
		public float amplification = 1.2f;
		// The luminosity threshold
		public float luminosityThreshold = 0.2f;
		// Clamps the noise between 0 and 1
		public bool noiseSaturation = false;
		// The XY axis offset of the effect
		public float textureOffset = 1f;

		public Shader nightVisionShader = null;
		private Material nightVisionMaterial = null;	
		
		public override bool CheckResources ()
		{
			nightVisionShader = Shader.Find ("MorePPEffects/NightVision");
			CheckSupport (false);
			nightVisionMaterial = CheckShaderAndCreateMaterial(nightVisionShader, nightVisionMaterial);
			
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
			nightVisionMaterial.SetFloat ("timer", timer);
			nightVisionMaterial.SetFloat ("noiseStrength", noiseStrength);
			nightVisionMaterial.SetFloat ("linesStrength", linesStrength);
			nightVisionMaterial.SetFloat ("amplification", amplification);
			nightVisionMaterial.SetFloat ("luminosityThreshold", luminosityThreshold);
			nightVisionMaterial.SetInt ("linesAmount", linesAmount);
			nightVisionMaterial.SetInt ("noiseSaturation", (noiseSaturation) ? 1 : 0);
			nightVisionMaterial.SetFloat ("textureOffset", textureOffset);
			Graphics.Blit (source, destination, nightVisionMaterial);
		}
	}
}
