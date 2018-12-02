/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Scanner.cs
 * Abstract : Creates scanner lines.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Scanner")]
	public class Scanner : PostEffectsBase
	{
		private float timer;

		// The intensity of the lines
		public float linesIntensity = 0.5f;
		// The vertical speed of the lines
		public float linesSpeed = 1.0f;
		// The amount of lines
		public float linesAmount = 300;
		
		public Shader scannerShader = null;
		private Material scannerMaterial = null;	
		
		public override bool CheckResources ()
		{
			scannerShader = Shader.Find ("MorePPEffects/Scanner");
			CheckSupport (false);
			scannerMaterial = CheckShaderAndCreateMaterial(scannerShader, scannerMaterial);
			
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
			
			timer += Time.deltaTime;
			scannerMaterial.SetFloat ("timer", timer);
			scannerMaterial.SetFloat ("linesIntensity", linesIntensity);
			scannerMaterial.SetFloat ("linesSpeed", linesSpeed);
			scannerMaterial.SetFloat ("linesAmount", linesAmount);
			Graphics.Blit (source, destination, scannerMaterial);
		}
	}
}
