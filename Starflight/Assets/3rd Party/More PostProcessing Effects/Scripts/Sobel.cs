/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Sobel.cs
 * Abstract : Reproduces a sobel effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Sobel")]
	public class Sobel : PostEffectsBase
	{
		// The color of the edges
		public Color edgeColor = Color.black;
		// The background color
		public Color backgroundColor = Color.white;
		// Show the background or a solid color
		public bool showBackground = true;
		// The threshold to draw edges
		public float threshold = 3f;

		public Shader sobelShader = null;
		private Material sobelMaterial = null;	
		
		public override bool CheckResources ()
		{
			sobelShader = Shader.Find ("MorePPEffects/Sobel");
			CheckSupport (false);
			sobelMaterial = CheckShaderAndCreateMaterial(sobelShader, sobelMaterial);
			
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

			sobelMaterial.SetColor ("edgeColor", edgeColor);
			sobelMaterial.SetColor ("backgroundColor", backgroundColor);
			sobelMaterial.SetFloat ("threshold", threshold);
			sobelMaterial.SetInt ("showBackground", (showBackground) ? 1 : 0);
			Graphics.Blit (source, destination, sobelMaterial);
		}
	}
}
