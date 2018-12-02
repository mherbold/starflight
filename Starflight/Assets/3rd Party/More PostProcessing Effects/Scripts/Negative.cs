/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Negative.cs
 * Abstract : Returns the negative image of the camera.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Negative")]
	public class Negative : PostEffectsBase
	{	
		public Shader negativeShader = null;
		private Material negativeMaterial = null;	
		
		public override bool CheckResources ()
		{
			negativeShader = Shader.Find ("MorePPEffects/Negative");
			CheckSupport (false);
			negativeMaterial = CheckShaderAndCreateMaterial(negativeShader, negativeMaterial);
			
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

			Graphics.Blit (source, destination, negativeMaterial);
		}
	}
}
