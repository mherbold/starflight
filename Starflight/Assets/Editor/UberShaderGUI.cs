
using UnityEngine;
using UnityEditor;

class UberShaderGUI : ShaderGUI
{
	private static class Styles
	{
		public static GUIContent albedoText = EditorGUIUtility.TrTextContent( "Albedo", "Albedo Map: UV1, RGB=Color, Alpha (Transparency)" );
		public static GUIContent specularText = EditorGUIUtility.TrTextContent( "Specular", "Specular Map: UV1, R=Intensity, G=Smoothness" );
		public static GUIContent occlusionText = EditorGUIUtility.TrTextContent( "Occlusion", "Occlusion Map: UV2, R=Intensity; Albedo Occlusion Enabled" );
		public static GUIContent normalText = EditorGUIUtility.TrTextContent( "Normal", "Normal Map: UV1, RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; Orthonormalize Enabled" );
		public static GUIContent emissiveText = EditorGUIUtility.TrTextContent( "Emissive", "Emissive Map: UV1, RGB=Color" );
		public static GUIContent waterText = EditorGUIUtility.TrTextContent( "Water", "Water Map: UV1, RGB=Uncompressed Normal, GA=DXT5 Compressed Normal" );
		public static GUIContent waterMaskText = EditorGUIUtility.TrTextContent( "Water Mask", "Water Mask: UV1, R=Opacity" );
		public static GUIContent blendSrcText = EditorGUIUtility.TrTextContent( "Source Blend", "" );
		public static GUIContent blendDstText = EditorGUIUtility.TrTextContent( "Destination Blend", "" );
		public static GUIContent blendPrePassText = EditorGUIUtility.TrTextContent( "Blend Z Pre-Pass", "" );
		public static GUIContent castShadowsText = EditorGUIUtility.TrTextContent( "Cast Shadows", "" );
		public static GUIContent lightOverrideText = EditorGUIUtility.TrTextContent( "Light Override", "" );
		public static GUIContent lightOverrideDirectionText = EditorGUIUtility.TrTextContent( "Light Override Direction", "" );
		public static GUIContent lightOverrideColorText = EditorGUIUtility.TrTextContent( "Light Override Color", "" );

		public static string mainText = "Starflight Uber Shader Properties";
	}

	MaterialProperty m_albedoMap = null;
	MaterialProperty m_albedoColor = null;

	MaterialProperty m_specularMap = null;
	MaterialProperty m_specularColor = null;
	MaterialProperty m_smoothness = null;

	MaterialProperty m_occlusionMap = null;
	MaterialProperty m_occlusionPower = null;
	MaterialProperty m_albedoOcclusionOn = null;

	MaterialProperty m_normalMap = null;
	MaterialProperty m_normalMapScaleOffset = null;
	MaterialProperty m_orthonormalizeOn = null;

	MaterialProperty m_emissiveMap = null;
	MaterialProperty m_emissiveColor = null;

	MaterialProperty m_waterMap = null;
	MaterialProperty m_waterScale = null;

	MaterialProperty m_waterMaskMap = null;

	MaterialProperty m_blendSrc = null;
	MaterialProperty m_blendDst = null;
	MaterialProperty m_blendPrePassOn = null;

	MaterialProperty m_shadowCasterOn = null;

	MaterialProperty m_lightOverrideOn = null;
	MaterialProperty m_lightOverrideDirection = null;
	MaterialProperty m_lightOverrideColor = null;

	MaterialEditor m_MaterialEditor;

	bool m_FirstTimeApply = true;

	string[] m_debugLogMessage;

	public UberShaderGUI()
	{
		m_debugLogMessage = new string[ 2 ];
	}

	public override void OnGUI( MaterialEditor materialEditor, MaterialProperty[] materialPropertyList )
	{
		FindProperties( materialPropertyList );

		m_MaterialEditor = materialEditor;

		Material material = materialEditor.target as Material;

		if ( m_FirstTimeApply )
		{
			m_FirstTimeApply = false;

			MaterialChanged( material );
		}

		ShaderPropertiesGUI( material );
	}

	public void ShaderPropertiesGUI( Material material )
	{
		// use default label width
		EditorGUIUtility.labelWidth = 0.0f;

		// fetect any changes to the material
		EditorGUI.BeginChangeCheck();

		// primary properties
		GUILayout.Label( Styles.mainText, EditorStyles.boldLabel );

		if ( m_albedoMap != null && m_albedoColor != null )
		{
			m_MaterialEditor.TexturePropertySingleLine( Styles.albedoText, m_albedoMap, m_albedoColor );
		}
		else if ( m_albedoColor != null )
		{
			m_MaterialEditor.ShaderProperty( m_albedoColor, Styles.albedoText );
		}

		if ( m_specularMap != null && m_specularColor != null && m_smoothness != null )
		{
			m_MaterialEditor.TexturePropertySingleLine( Styles.specularText, m_specularMap, m_specularColor, m_smoothness );
		}

		if ( m_occlusionMap != null && m_occlusionPower != null && m_albedoOcclusionOn != null )
		{
			m_MaterialEditor.TexturePropertySingleLine( Styles.occlusionText, m_occlusionMap, m_occlusionPower, m_albedoOcclusionOn );
		}

		if ( m_normalMap != null && m_normalMapScaleOffset != null && m_orthonormalizeOn != null )
		{
			m_MaterialEditor.TexturePropertySingleLine( Styles.normalText, m_normalMap, m_normalMapScaleOffset, m_orthonormalizeOn );
		}

		if ( m_emissiveMap != null && m_emissiveColor != null )
		{
			m_MaterialEditor.TexturePropertySingleLine( Styles.emissiveText, m_emissiveMap, m_emissiveColor );
		}

		if ( m_waterMap != null && m_waterScale != null )
		{
			m_MaterialEditor.TexturePropertySingleLine( Styles.waterText, m_waterMap, m_waterScale );
		}

		if ( m_waterMaskMap != null )
		{
			if ( HasTextureMap( material, "WaterMap" ) )
			{
				m_MaterialEditor.TexturePropertySingleLine( Styles.waterMaskText, m_waterMaskMap );
			}
		}

		if ( m_blendSrc != null && m_blendDst != null )
		{
			m_MaterialEditor.ShaderProperty( m_blendSrc, Styles.blendSrcText );
			m_MaterialEditor.ShaderProperty( m_blendDst, Styles.blendDstText );
		}

		if ( m_blendPrePassOn != null )
		{
			m_MaterialEditor.ShaderProperty( m_blendPrePassOn, Styles.blendPrePassText );
		}

		if ( m_shadowCasterOn != null )
		{
			m_MaterialEditor.ShaderProperty( m_shadowCasterOn, Styles.castShadowsText );
		}

		if ( m_lightOverrideOn != null )
		{
			m_MaterialEditor.ShaderProperty( m_lightOverrideOn, Styles.lightOverrideText );

			if ( IsSwitchedOn( material, "LightOverrideOn" ) )
			{
				m_MaterialEditor.ShaderProperty( m_lightOverrideDirection, Styles.lightOverrideDirectionText, 1 );
				m_MaterialEditor.ShaderProperty( m_lightOverrideColor, Styles.lightOverrideColorText, 1 );
			}
		}

		if ( EditorGUI.EndChangeCheck() )
		{
			MaterialChanged( material );
		}
	}

	void FindProperties( MaterialProperty[] materialPropertyList )
	{
		m_albedoMap = FindProperty( "AlbedoMap", materialPropertyList, false );
		m_albedoColor = FindProperty( "AlbedoColor", materialPropertyList, false );

		m_specularMap = FindProperty( "SpecularMap", materialPropertyList, false );
		m_specularColor = FindProperty( "SpecularColor", materialPropertyList, false );
		m_smoothness = FindProperty( "Smoothness", materialPropertyList, false );

		m_occlusionMap = FindProperty( "OcclusionMap", materialPropertyList, false );
		m_occlusionPower = FindProperty( "OcclusionPower", materialPropertyList, false );
		m_albedoOcclusionOn = FindProperty( "AlbedoOcclusionOn", materialPropertyList, false );

		m_normalMap = FindProperty( "NormalMap", materialPropertyList, false );
		m_normalMapScaleOffset = FindProperty( "NormalMapScaleOffset", materialPropertyList, false );
		m_orthonormalizeOn = FindProperty( "OrthonormalizeOn", materialPropertyList, false );

		m_emissiveMap = FindProperty( "EmissiveMap", materialPropertyList, false );
		m_emissiveColor = FindProperty( "EmissiveColor", materialPropertyList, false );

		m_waterMap = FindProperty( "WaterMap", materialPropertyList, false );
		m_waterScale = FindProperty( "WaterScale", materialPropertyList, false );

		m_waterMaskMap = FindProperty( "WaterMaskMap", materialPropertyList, false );

		m_blendSrc = FindProperty( "BlendSrc", materialPropertyList, false );
		m_blendDst = FindProperty( "BlendDst", materialPropertyList, false );
		m_blendPrePassOn = FindProperty( "BlendPrePassOn", materialPropertyList, false );

		m_shadowCasterOn = FindProperty( "ShadowCasterOn", materialPropertyList, false );

		m_lightOverrideOn = FindProperty( "LightOverrideOn", materialPropertyList, false );
		m_lightOverrideDirection = FindProperty( "LightOverrideDirection", materialPropertyList, false );
		m_lightOverrideColor = FindProperty( "LightOverrideColor", materialPropertyList, false );
	}

	void MaterialChanged( Material material )
	{
		// texture maps on/off
		bool albedoMapOn = HasTextureMap( material, "AlbedoMap" );
		bool specularMapOn = HasTextureMap( material, "SpecularMap" );
		bool occlusionMapOn = HasTextureMap( material, "OcclusionMap" );
		bool normalMapOn = HasTextureMap( material, "NormalMap" );
		bool emissiveMapOn = HasTextureMap( material, "EmissiveMap" );
		bool waterMapOn = HasTextureMap( material, "WaterMap" );
		bool waterMaskMapOn = HasTextureMap( material, "WaterMaskMap" );

		// texture compression
		bool normalMapIsCompressed = TextureIsCompressed( material, "NormalMap" );
		bool waterMapIsCompressed = TextureIsCompressed( material, "WaterMap" );

		// toggle switches on/off
		bool albedoOcclusionOn = IsSwitchedOn( material, "AlbedoOcclusionOn" );
		bool orthonormalizeOn = IsSwitchedOn( material, "OrthonormalizeOn" );
		bool blendPrePassOn = IsSwitchedOn( material, "BlendPrePassOn" );
		bool shadowCasterOn = IsSwitchedOn( material, "ShadowCasterOn" );
		bool lightOverrideOn = IsSwitchedOn( material, "LightOverrideOn" );

		// disable forward shadow rendering if light override is enabled
		bool forwardShadowsOn = !lightOverrideOn;

		// start with alpha disabled
		bool alphaOn = false;

		// enable alpha if alpha is less than 1
		if ( material.HasProperty( "AlbedoColor" ) )
		{
			var color = material.GetColor( "AlbedoColor" );

			alphaOn = ( color.a < 1.0f );
		}

		// enable alpha if the albedo map is enabled and it has an alpha channel and it is set up as transparency
		if ( albedoMapOn )
		{
			Texture2D albedoMap = material.GetTexture( "AlbedoMap" ) as Texture2D;

			bool formatHasAlpha;

			switch ( albedoMap.format )
			{
				case TextureFormat.Alpha8:
				case TextureFormat.ARGB4444:
				case TextureFormat.RGBA32:
				case TextureFormat.ARGB32:
				case TextureFormat.DXT5:
				case TextureFormat.RGBA4444:
				case TextureFormat.BGRA32:
				case TextureFormat.RGBAHalf:
				case TextureFormat.RGBAFloat:
				case TextureFormat.BC7:
				case TextureFormat.DXT5Crunched:
					formatHasAlpha = true;
					break;

				default:
					formatHasAlpha = false;
					break;
			}

			if ( formatHasAlpha && albedoMap.alphaIsTransparency )
			{
				alphaOn = true;
			}
		}

		// enable blending if blendsrc != one or blenddst != zero
		bool blendOn = false;

		if ( material.HasProperty( "BlendSrc" ) && material.HasProperty( "BlendDst" ) )
		{
			blendOn = ( ( (int) material.GetFloat( "BlendSrc" ) != (int) UnityEngine.Rendering.BlendMode.One ) || ( (int) material.GetFloat( "BlendDst" ) != (int) UnityEngine.Rendering.BlendMode.Zero ) );
		}

		// if blending is not enabled there is no point in having alpha turned on and no point in having the blend prepass enabled
		if ( !blendOn )
		{
			alphaOn = false;
			blendPrePassOn = false;
		}

		// enable specular if specular color is not black
		bool specularOn = false;

		if ( material.HasProperty( "SpecularColor" ) )
		{
			specularOn = ( material.GetColor( "SpecularColor" ) != Color.black );
		}

		// if specular is not on then there is no point in having a specular map
		if ( !specularOn )
		{
			specularMapOn = false;
		}

		// if water is not on then there is no point in having a water mask map
		if ( !waterMapOn )
		{
			waterMaskMapOn = false;
		}

		// for debug log
		m_debugLogMessage[ 1 ] = "Enabled keywords:";

		// set all the material keywords
		SetKeyword( material, "ALBEDOMAP_ON", albedoMapOn );
		SetKeyword( material, "ALPHA_ON", alphaOn );
		SetKeyword( material, "SPECULARMAP_ON", specularMapOn );
		SetKeyword( material, "SPECULAR_ON", specularOn );
		SetKeyword( material, "OCCLUSIONMAP_ON", occlusionMapOn );
		SetKeyword( material, "ALBEDOOCCLUSION_ON", albedoOcclusionOn );
		SetKeyword( material, "NORMALMAP_ON", normalMapOn );
		SetKeyword( material, "NORMALMAP_ISCOMPRESSED", normalMapIsCompressed );
		SetKeyword( material, "ORTHONORMALIZE_ON", orthonormalizeOn );
		SetKeyword( material, "EMISSIVEMAP_ON", emissiveMapOn );
		SetKeyword( material, "WATERMAP_ON", waterMapOn );
		SetKeyword( material, "WATERMAP_ISCOMPRESSED", waterMapIsCompressed );
		SetKeyword( material, "WATERMASKMAP_ON", waterMaskMapOn );
		SetKeyword( material, "LIGHTOVERRIDE_ON", lightOverrideOn );
		SetKeyword( material, "FORWARDSHADOWS_ON", forwardShadowsOn );

		// if blending is on then force material to be transparent (this also disables the deferred pass automatically)
		if ( blendOn )
		{
			m_debugLogMessage[ 1 ] += " " + "TYPE:TRANSPARENT";

			material.SetOverrideTag( "RenderType", "Transparent" );
			material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
		}
		else
		{
			material.SetOverrideTag( "RenderType", "" );
			material.renderQueue = -1;
		}

		// disable the shadow caster pass if the material does not want to cast shadows (does not work!)
		material.SetShaderPassEnabled( "SHADOWCASTER", shadowCasterOn );

		if ( !shadowCasterOn )
		{
			m_debugLogMessage[ 1 ] += " " + " SHADOWCASTER:OFF";
		}

		// enabled or disable the blend pre pass (does not work!)
		material.SetShaderPassEnabled( "BLENDPREPASS", blendPrePassOn );

		if ( !blendPrePassOn )
		{
			m_debugLogMessage[ 1 ] += " " + " BLENDPREPASS:OFF";
		}

		// normalize the light override direction
		if ( material.HasProperty( "LightOverrideDirection" ) )
		{
			var lightOverrideDirection = material.GetVector( "LightOverrideDirection" );

			lightOverrideDirection = Vector3.Normalize( lightOverrideDirection );

			material.SetVector( "LightOverrideDirection", lightOverrideDirection );
		}

		// debug log
		if ( ( m_debugLogMessage[ 0 ] == null ) || ( m_debugLogMessage[ 0 ] != m_debugLogMessage[ 1 ] ) )
		{
			m_debugLogMessage[ 0 ] = m_debugLogMessage[ 1 ];

			Debug.Log( m_debugLogMessage[ 0 ] );
		}
	}

	void SetKeyword( Material material, string keyword, bool state )
	{
		if ( state )
		{
			material.EnableKeyword( keyword );

			m_debugLogMessage[ 1 ] += " " + keyword;
		}
		else
		{
			material.DisableKeyword( keyword );
		}
	}

	bool HasTextureMap( Material material, string propertyName )
	{
		if ( material.HasProperty( propertyName ) )
		{
			return material.GetTexture( propertyName );
		}

		return false;
	}

	bool IsSwitchedOn( Material material, string propertyName )
	{
		if ( material.HasProperty( propertyName ) )
		{
			return ( material.GetFloat( propertyName ) == 1.0f );
		}

		return false;
	}

	bool TextureIsCompressed( Material material, string propertyName )
	{
		if ( HasTextureMap( material, propertyName ) )
		{
			Texture2D texture = material.GetTexture( propertyName ) as Texture2D;

			if ( texture.format == TextureFormat.DXT5 )
			{
				return true;
			}
		}

		return false;
	}
}
