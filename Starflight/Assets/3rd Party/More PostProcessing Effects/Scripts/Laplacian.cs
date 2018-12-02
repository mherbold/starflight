/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Laplacian.cs
 * Abstract : Returns the laplacian of the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Laplacian")]
	public class Laplacian : PostEffectsBase
	{
		public Shader laplacianShader = null;
		private Material laplacianMaterial = null;	
		
		public override bool CheckResources ()
		{
			laplacianShader = Shader.Find ("MorePPEffects/Laplacian");
			CheckSupport (false);
			laplacianMaterial = CheckShaderAndCreateMaterial(laplacianShader, laplacianMaterial);
			
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

			Graphics.Blit (source, destination, laplacianMaterial);
		}
	}
}
