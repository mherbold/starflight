/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : ColoredRays.cs
 * Abstract : Adds colored rays to the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Colored Rays")]
	public class ColoredRays : PostEffectsBase
	{
		private float timer;

		// The strength of the effect
		public float strength = 1;
		// The strength of each component
		public float RraysStrength = 2, GraysStrength = 2, BraysStrength = 2;
		// The speed of each component
		public float RraysSpeed = 5, GraysSpeed = 5, BraysSpeed = 5;

		public Shader coloredRaysShader = null;
		private Material coloredRaysMaterial = null;	
		
		public override bool CheckResources ()
		{
			coloredRaysShader = Shader.Find ("MorePPEffects/ColoredRays");
			CheckSupport (false);
			coloredRaysMaterial = CheckShaderAndCreateMaterial(coloredRaysShader, coloredRaysMaterial);
			
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
			coloredRaysMaterial.SetFloat ("timer", timer);
			coloredRaysMaterial.SetFloat ("strength", strength);
			coloredRaysMaterial.SetFloat ("RraysStrength", RraysStrength);
			coloredRaysMaterial.SetFloat ("GraysStrength", GraysStrength);
			coloredRaysMaterial.SetFloat ("BraysStrength", BraysStrength);
			coloredRaysMaterial.SetFloat ("RraysSpeed", RraysSpeed);
			coloredRaysMaterial.SetFloat ("GraysSpeed", GraysSpeed);
			coloredRaysMaterial.SetFloat ("BraysSpeed", BraysSpeed);
			Graphics.Blit (source, destination, coloredRaysMaterial);
		}
	}
}
