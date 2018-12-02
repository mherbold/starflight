/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Waves.cs
 * Abstract : Creates waves on the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Waves")]
	public class Waves : PostEffectsBase
	{
		private float timer;
		
		// The strength of the waves
		public float strengthX = 0.5f, strengthY = 0.5f;
		// The frequencies of the waves
		public float frequencyX = 10, frequencyY = 10;
		// The speed of the waves
		public float speed = 2;
		
		public Shader wavesShader = null;
		private Material wavesMaterial = null;	
		
		public override bool CheckResources ()
		{
			wavesShader = Shader.Find ("MorePPEffects/Waves");
			CheckSupport (false);
			wavesMaterial = CheckShaderAndCreateMaterial(wavesShader, wavesMaterial);
			
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
			wavesMaterial.SetFloat ("timer", timer);
			wavesMaterial.SetFloat ("strengthX", strengthX);
			wavesMaterial.SetFloat ("strengthY", strengthY);
			wavesMaterial.SetFloat ("frequencyX", frequencyX);
			wavesMaterial.SetFloat ("frequencyY", frequencyY);
			wavesMaterial.SetFloat ("speed", speed);
			Graphics.Blit (source, destination, wavesMaterial);
		}
	}
}
