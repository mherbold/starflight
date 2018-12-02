/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Dreamy.cs
 * Abstract : Recreates a dream effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Dreamy")]
	public class Dreamy : PostEffectsBase
	{
		// The strength of the effect
		public float strength = 0.6f;

		public Shader dreamyShader = null;
		private Material dreamyMaterial = null;	
		
		public override bool CheckResources ()
		{
			dreamyShader = Shader.Find ("MorePPEffects/Dreamy");
			CheckSupport (false);
			dreamyMaterial = CheckShaderAndCreateMaterial(dreamyShader, dreamyMaterial);
			
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

			dreamyMaterial.SetFloat ("strength", strength);
			Graphics.Blit (source, destination, dreamyMaterial);
		}
	}
}
