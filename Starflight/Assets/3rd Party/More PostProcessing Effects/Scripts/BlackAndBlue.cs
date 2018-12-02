/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : BlackAndBlue.cs
 * Abstract : Only show in grayscale with blue color.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Black and Blue")]
	public class BlackAndBlue : PostEffectsBase
	{
		// The smoothness of the effect
		public float smoothness = 0.25f;

		public Shader blackAndBlueShader = null;
		private Material blackAndBlueMaterial = null;	
		
		public override bool CheckResources ()
		{
			blackAndBlueShader = Shader.Find ("MorePPEffects/BlackAndBlue");
			CheckSupport (false);
			blackAndBlueMaterial = CheckShaderAndCreateMaterial(blackAndBlueShader, blackAndBlueMaterial);
			
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

			blackAndBlueMaterial.SetFloat ("smoothness", smoothness);
			Graphics.Blit (source, destination, blackAndBlueMaterial);
		}
	}
}
