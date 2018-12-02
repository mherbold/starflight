/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Shadows2D.cs
 * Abstract : Create a 2D shadow of the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Shadows 2D")]
	public class Shadows2D : PostEffectsBase
	{
		// The strength of the shadow
		public float shadowStrength = 2.0f;
		// The offset of the shadow
		public float offsetStrength = 5.0f;
		
		public Shader shadows2DShader = null;
		private Material shadows2DMaterial = null;	
		
		public override bool CheckResources ()
		{
			shadows2DShader = Shader.Find ("MorePPEffects/Shadows2D");
			CheckSupport (false);
			shadows2DMaterial = CheckShaderAndCreateMaterial(shadows2DShader, shadows2DMaterial);
			
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

			shadows2DMaterial.SetFloat ("shadowStrength", shadowStrength);
			shadows2DMaterial.SetFloat ("offsetStrength", offsetStrength);
			Graphics.Blit (source, destination, shadows2DMaterial);
		}
	}
}
