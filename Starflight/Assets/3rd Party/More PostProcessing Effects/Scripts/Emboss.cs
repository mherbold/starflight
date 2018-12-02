/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Emboss.cs
 * Abstract : This effect reproduce an embossed style. You can choose the strength of the bumps which will more or less create the 
 * style. It is also possible to add a grayscale mode to the effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Emboss")]
	public class Emboss : PostEffectsBase
	{
		// The strength represent the shift amount of the emboss
		public float strength = 0.6f;
		// Grayscale is used to know if the shader must apply a grayscale mode
		public bool grayscale = false;
		
		public Shader embossShader = null;
		private Material embossMaterial = null;	
		
		public override bool CheckResources ()
		{
			embossShader = Shader.Find ("MorePPEffects/Emboss");
			CheckSupport (false);
			embossMaterial = CheckShaderAndCreateMaterial(embossShader, embossMaterial);
			
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

			embossMaterial.SetFloat ("strength", strength);
			embossMaterial.SetInt ("grayscale", (grayscale) ? 1 : 0);
			Graphics.Blit (source, destination, embossMaterial);
		}
	}
}
