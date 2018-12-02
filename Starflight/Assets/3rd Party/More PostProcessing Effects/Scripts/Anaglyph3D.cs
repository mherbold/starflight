/*
 * Author : Maxime JUMELLE
 * Namespace : MorePPEffects
 * Project : More Post-Processing Effects Package
 * 
 * If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com
 * 
 * File : Anaglyph3D.cs
 * Abstract : Creates a stereo anaglyph 3D effect.
 * 
 * */

using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace MorePPEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/More Effects/Anaglyph 3D")]
	public class Anaglyph3D : PostEffectsBase
	{
		public Shader anaglyph3DShader = null;
		private Material anaglyph3DMaterial = null;	
		
		public override bool CheckResources ()
		{
			anaglyph3DShader = Shader.Find ("MorePPEffects/Anaglyph3D");
			CheckSupport (false);
			anaglyph3DMaterial = CheckShaderAndCreateMaterial(anaglyph3DShader, anaglyph3DMaterial);
			
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

			Graphics.Blit (source, destination, anaglyph3DMaterial);
		}
	}
}
