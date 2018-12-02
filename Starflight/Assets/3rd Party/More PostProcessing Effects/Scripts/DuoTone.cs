/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : DuoTone.cs
 * Abstract : Uses only two colors on the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Duo Tone")]
	public class DuoTone : PostEffectsBase
	{
		// Represent the two colors
		public Color color1 = Color.white;
		public Color color2 = Color.black;

		public float minLimit = 0.2f;
		public float maxLimit = 0.9f;
		
		public Shader duoToneShader = null;
		private Material duoToneMaterial = null;	
		
		public override bool CheckResources ()
		{
			duoToneShader = Shader.Find ("MorePPEffects/DuoTone");
			CheckSupport (false);
			duoToneMaterial = CheckShaderAndCreateMaterial(duoToneShader, duoToneMaterial);
			
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

			duoToneMaterial.SetColor ("color1", color1);
			duoToneMaterial.SetColor ("color2", color2);
			duoToneMaterial.SetFloat ("minLimit", minLimit);
			duoToneMaterial.SetFloat ("maxLimit", maxLimit);
			Graphics.Blit (source, destination, duoToneMaterial);
		}
	}
}
