/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Lens.cs
 * Abstract : Creates a lens effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Lens")]
	public class Lens : PostEffectsBase
	{
		// The global distortion
		public float lensDistortion = -2.0f;
		// The distortion around the lens center
		public float cubicDistortion = 0.75f;

		public Shader lensShader = null;
		private Material lensMaterial = null;	
		
		public override bool CheckResources ()
		{
			lensShader = Shader.Find ("MorePPEffects/Lens");
			CheckSupport (false);
			lensMaterial = CheckShaderAndCreateMaterial(lensShader, lensMaterial);
			
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

			lensMaterial.SetFloat ("lensDistortion", lensDistortion);
			lensMaterial.SetFloat ("cubicDistortion", cubicDistortion);
			Graphics.Blit (source, destination, lensMaterial);
		}
	}
}
