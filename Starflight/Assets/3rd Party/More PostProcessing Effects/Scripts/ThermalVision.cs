/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : ThermalVision.cs
 * Abstract : Simulates a thermal vision.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Thermal Vision")]
	public class ThermalVision : PostEffectsBase
	{
		public Shader thermalVisionShader = null;
		private Material thermalVisionMaterial = null;	
		
		public override bool CheckResources ()
		{
			thermalVisionShader = Shader.Find ("MorePPEffects/ThermalVision");
			CheckSupport (false);
			thermalVisionMaterial = CheckShaderAndCreateMaterial(thermalVisionShader, thermalVisionMaterial);
			
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

			Graphics.Blit (source, destination, thermalVisionMaterial);
		}
	}
}
