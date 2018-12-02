/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : BlackAndRed.cs
 * Abstract : Only show in grayscale with red color.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Black and Red")]
	public class BlackAndRed : PostEffectsBase
	{
		// The smoothness of the effect
		public float smoothness = 0.25f;

		public Shader blackAndRedShader = null;
		private Material blackAndRedMaterial = null;	
		
		public override bool CheckResources ()
		{
			blackAndRedShader = Shader.Find ("MorePPEffects/BlackAndRed");
			CheckSupport (false);
			blackAndRedMaterial = CheckShaderAndCreateMaterial(blackAndRedShader, blackAndRedMaterial);
			
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

			blackAndRedMaterial.SetFloat ("smoothness", smoothness);
			Graphics.Blit (source, destination, blackAndRedMaterial);
		}
	}
}
