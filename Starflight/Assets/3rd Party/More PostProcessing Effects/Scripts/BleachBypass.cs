/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : BleachBypass.cs
 * Abstract : Reproduces the dark effect in films.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Bleach Bypass")]
	public class BleachBypass : PostEffectsBase
	{
		// The intensity of the darkness
		public float darkness = 1.0f;
		
		public Shader bleachBypassShader = null;
		private Material bleachBypassMaterial = null;	
		
		public override bool CheckResources ()
		{
			bleachBypassShader = Shader.Find ("MorePPEffects/BleachBypass");
			CheckSupport (false);
			bleachBypassMaterial = CheckShaderAndCreateMaterial(bleachBypassShader, bleachBypassMaterial);
			
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

			bleachBypassMaterial.SetFloat ("darkness", darkness);
			Graphics.Blit (source, destination, bleachBypassMaterial);
		}
	}
}
