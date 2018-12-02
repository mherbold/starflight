/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : NormalMapDistortion.cs
 * Abstract : Distort the camera according to the normal map.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/NormalMap Distortion")]
	public class NormalMapDistortion : PostEffectsBase
	{
		private float timer;

		// The speed of the normal map displacement
		public float speedX = 1, speedY = 1;
		// The normal map
		public Texture normalMap;

		public Shader normalMapDistortionShader = null;
		private Material normalMapDistortionMaterial = null;	
		
		public override bool CheckResources ()
		{
			normalMapDistortionShader = Shader.Find ("MorePPEffects/NormalMapDistortion");
			CheckSupport (false);
			normalMapDistortionMaterial = CheckShaderAndCreateMaterial(normalMapDistortionShader, normalMapDistortionMaterial);
			
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
			normalMapDistortionMaterial.SetFloat ("timer", timer);
			normalMapDistortionMaterial.SetFloat ("speedX", speedX);
			normalMapDistortionMaterial.SetFloat ("speedY", speedY);
			normalMapDistortionMaterial.SetTexture ("NormalMap", normalMap);
			Graphics.Blit (source, destination, normalMapDistortionMaterial);
		}
	}
}
