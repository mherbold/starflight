
using UnityEditor;
using UnityEngine;

using System.IO;

public class EditorUtilitySaveFilePanel : MonoBehaviour
{
	[MenuItem( "Starflight Remake/Save Texture to File" )]
	static void Apply()
	{
		Texture2D texture = Selection.activeObject as Texture2D;

		if ( texture == null )
		{
			EditorUtility.DisplayDialog( "Select Texture", "You Must Select a Texture first!", "Ok" );
			return;
		}

		var path = EditorUtility.SaveFilePanel( "Save texture as PNG", "", texture.name + ".png", "png" );

		if ( path.Length != 0 )
		{
			Texture2D readableTexture = null;

			if ( ( texture.format == TextureFormat.ARGB32 ) || ( texture.format == TextureFormat.RGB24 ) )
			{
				readableTexture = texture;
			}
			else
			{
				RenderTexture renderTexture = RenderTexture.GetTemporary( texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB );

				Graphics.Blit( texture, renderTexture );

				RenderTexture previous = RenderTexture.active;

				RenderTexture.active = renderTexture;

				readableTexture = new Texture2D( texture.width, texture.height );

				readableTexture.ReadPixels( new Rect( 0, 0, renderTexture.width, renderTexture.height ), 0, 0 );

				readableTexture.Apply();

				RenderTexture.active = previous;

				RenderTexture.ReleaseTemporary( renderTexture );
			}

			var pngData = readableTexture.EncodeToPNG();

			if ( pngData != null )
			{
				File.WriteAllBytes( path, pngData );
			}
		}
	}
}
