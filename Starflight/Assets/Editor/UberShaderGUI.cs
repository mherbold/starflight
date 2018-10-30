
using UnityEngine;
using UnityEditor;

class UberShaderGUI : ShaderGUI
{
	private static class Styles
	{
		public static GUIContent albedoText = EditorGUIUtility.TrTextContent( "Albedo", "Albedo Map: UV1, RGB=Color, Alpha (Transparency)" );
		public static GUIContent specularText = EditorGUIUtility.TrTextContent( "Specular", "Specular Map: UV1, R=Intensity, G=Smoothness" );
		public static GUIContent occlusionText = EditorGUIUtility.TrTextContent( "Occlusion", "Occlusion Map: UV2, R=Intensity; Apply to Albedo Toggle" );
		public static GUIContent normalText = EditorGUIUtility.TrTextContent( "Normal", "Normal Map: UV1, RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; Orthonormalize Texture Space Toggle" );
		public static GUIContent emissiveText = EditorGUIUtility.TrTextContent( "Emissive", "Emissive Map: UV1, RGB=Color" );
		public static GUIContent waterText = EditorGUIUtility.TrTextContent( "Water", "Water Map: UV1, RGB=Uncompressed Normal, GA=DXT5 Compressed Normal" );
		public static GUIContent waterMaskText = EditorGUIUtility.TrTextContent( "Water Mask", "Water Mask: UV1, R=Opacity" );
		public static GUIContent blendSrcText = EditorGUIUtility.TrTextContent( "Source Blend", "" );
		public static GUIContent blendDstText = EditorGUIUtility.TrTextContent( "Destination Blend", "" );
		public static GUIContent blendPrePassText = EditorGUIUtility.TrTextContent( "Blend Z Pre-Pass Toggle", "" );
		public static GUIContent castShadowsText = EditorGUIUtility.TrTextContent( "Cast Shadows", "" );
		public static GUIContent uiToggleText = EditorGUIUtility.TrTextContent( "UI Toggle", "" );
		public static GUIContent uiLightDirectionText = EditorGUIUtility.TrTextContent( "UI Light Direction", "" );
		public static GUIContent uiLightColorText = EditorGUIUtility.TrTextContent( "UI Light Color", "" );

		public static string mainText = "Starflight Uber Shader Properties";
	}

	MaterialProperty m_albedoMap = null;
	MaterialProperty m_albedoColor = null;

	MaterialProperty m_alpha = null;

	MaterialProperty m_specularMap = null;
	MaterialProperty m_specularColor = null;
	MaterialProperty m_smoothness = null;

	MaterialProperty m_occlusionMap = null;
	MaterialProperty m_occlusionPower = null;
	MaterialProperty m_applyOcclusionToAlbedo = null;

	MaterialProperty m_normalMap = null;
	MaterialProperty m_normalMapScaleOffset = null;
	MaterialProperty m_orthonormalizeTextureSpace = null;

	MaterialProperty m_emissiveMap = null;
	MaterialProperty m_emissiveColor = null;

	MaterialProperty m_waterMap = null;
	MaterialProperty m_waterScale = null;

	MaterialProperty m_waterMaskMap = null;

	MaterialProperty m_blendSrc = null;
	MaterialProperty m_blendDst = null;
	MaterialProperty m_blendPrePass = null;

	MaterialProperty m_castShadows = null;

	MaterialProperty m_uiToggle = null;
	MaterialProperty m_uiLightDirection = null;
	MaterialProperty m_uiLightColor = null;

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
		// Use default labelWidth
		EditorGUIUtility.labelWidth = 0.0f;

		// Detect any changes to the material
		EditorGUI.BeginChangeCheck();

		// Primary properties
		GUILayout.Label( Styles.mainText, EditorStyles.boldLabel );

		m_MaterialEditor.TexturePropertySingleLine( Styles.albedoText, m_albedoMap, m_albedoColor, m_alpha );
		m_MaterialEditor.TexturePropertySingleLine( Styles.specularText, m_specularMap, m_specularColor, m_smoothness );
		m_MaterialEditor.TexturePropertySingleLine( Styles.occlusionText, m_occlusionMap, m_occlusionPower, m_applyOcclusionToAlbedo );
		m_MaterialEditor.TexturePropertySingleLine( Styles.normalText, m_normalMap, m_normalMapScaleOffset, m_orthonormalizeTextureSpace );
		m_MaterialEditor.TexturePropertySingleLine( Styles.emissiveText, m_emissiveMap, m_emissiveColor );
		m_MaterialEditor.TexturePropertySingleLine( Styles.waterText, m_waterMap, m_waterScale );
		m_MaterialEditor.TexturePropertySingleLine( Styles.waterMaskText, m_waterMaskMap );
		m_MaterialEditor.ShaderProperty( m_blendSrc, Styles.blendSrcText );
		m_MaterialEditor.ShaderProperty( m_blendDst, Styles.blendDstText );
		m_MaterialEditor.ShaderProperty( m_blendPrePass, Styles.blendPrePassText );
		m_MaterialEditor.ShaderProperty( m_castShadows, Styles.castShadowsText );
		m_MaterialEditor.ShaderProperty( m_uiToggle, Styles.uiToggleText );

		if ( material.GetFloat( "UIToggle" ) == 1.0f )
		{
			m_MaterialEditor.ShaderProperty( m_uiLightDirection, Styles.uiLightDirectionText, 1 );
			m_MaterialEditor.ShaderProperty( m_uiLightColor, Styles.uiLightColorText, 1 );
		}

		if ( EditorGUI.EndChangeCheck() )
		{
			MaterialChanged( material );
		}
	}

	void FindProperties( MaterialProperty[] materialPropertyList )
	{
		m_albedoMap = FindProperty( "AlbedoMap", materialPropertyList );
		m_albedoColor = FindProperty( "AlbedoColor", materialPropertyList );

		m_alpha = FindProperty( "Alpha", materialPropertyList );

		m_specularMap = FindProperty( "SpecularMap", materialPropertyList );
		m_specularColor = FindProperty( "SpecularColor", materialPropertyList );
		m_smoothness = FindProperty( "Smoothness", materialPropertyList );

		m_occlusionMap = FindProperty( "OcclusionMap", materialPropertyList );
		m_occlusionPower = FindProperty( "OcclusionPower", materialPropertyList );
		m_applyOcclusionToAlbedo = FindProperty( "ApplyOcclusionToAlbedo", materialPropertyList );

		m_normalMap = FindProperty( "NormalMap", materialPropertyList );
		m_normalMapScaleOffset = FindProperty( "NormalMapScaleOffset", materialPropertyList );
		m_orthonormalizeTextureSpace = FindProperty( "OrthonormalizeTextureSpace", materialPropertyList );

		m_emissiveMap = FindProperty( "EmissiveMap", materialPropertyList );
		m_emissiveColor = FindProperty( "EmissiveColor", materialPropertyList );

		m_waterMap = FindProperty( "WaterMap", materialPropertyList );
		m_waterScale = FindProperty( "WaterScale", materialPropertyList );

		m_waterMaskMap = FindProperty( "WaterMaskMap", materialPropertyList );

		m_blendSrc = FindProperty( "BlendSrc", materialPropertyList );
		m_blendDst = FindProperty( "BlendDst", materialPropertyList );
		m_blendPrePass = FindProperty( "BlendPrePass", materialPropertyList );

		m_castShadows = FindProperty( "CastShadows", materialPropertyList );

		m_uiToggle = FindProperty( "UIToggle", materialPropertyList );
		m_uiLightDirection = FindProperty( "UILightDirection", materialPropertyList );
		m_uiLightColor = FindProperty( "UILightColor", materialPropertyList );
	}

	void MaterialChanged( Material material )
	{
		bool albedoMapOn = material.GetTexture( "AlbedoMap" );
		bool alphaOn = ( material.GetFloat( "Alpha" ) < 1.0f );
		bool specularMapOn = material.GetTexture( "SpecularMap" );
		bool specularOn = ( material.GetColor( "SpecularColor" ) != Color.black );
		bool occlusionMapOn = material.GetTexture( "OcclusionMap" );
		bool occlusionApplyToAlbedo = ( material.GetFloat( "ApplyOcclusionToAlbedo" ) == 1.0f );
		bool normalMapOn = material.GetTexture( "NormalMap" );
		bool normalMapCompressed = false;
		bool emissiveMapOn = material.GetTexture( "EmissiveMap" );
		bool waterMapOn = material.GetTexture( "WaterMap" );
		bool waterMapCompressed = false;
		bool waterMaskMapOn = material.GetTexture( "WaterMaskMap" );
		bool textureSpaceOrthonormalize = ( material.GetFloat( "OrthonormalizeTextureSpace" ) == 1.0f );
		bool blendEnabled = ( ( (int) material.GetFloat( "BlendSrc" ) != (int) UnityEngine.Rendering.BlendMode.One ) || ( (int) material.GetFloat( "BlendDst" ) != (int) UnityEngine.Rendering.BlendMode.Zero ) );
		bool blendPrePassEnabled = ( material.GetFloat( "BlendPrePass" ) == 1.0f );
		bool castShadowsEnabled = ( material.GetFloat( "CastShadows" ) == 1.0f );
		bool uiOn = ( material.GetFloat( "UIToggle" ) == 1.0f );
		bool forwardShadowsOn = !uiOn;

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

		// force alpha and blend pre-pass off if blending is not enabled
		if ( !blendEnabled )
		{
			alphaOn = false;
			blendPrePassEnabled = false;
		}

		// force specular map off if specular color is black
		if ( !specularOn )
		{
			specularMapOn = false;
		}

		// if the normal map is on then check whether it is compressed (DXT5)
		if ( normalMapOn )
		{
			Texture2D normalMap = material.GetTexture( "NormalMap" ) as Texture2D;

			if ( normalMap.format == TextureFormat.DXT5 )
			{
				normalMapCompressed = true;
			}
		}

		// if the water map is on then check whether it is compressed (DXT5)
		if ( waterMapOn )
		{
			Texture2D waterMap = material.GetTexture( "WaterMap" ) as Texture2D;

			if ( waterMap.format == TextureFormat.DXT5 )
			{
				waterMapCompressed = true;
			}
		}

		// force water map mask off if water map is off
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
		SetKeyword( material, "OCCLUSION_APPLYTOALBEDO", occlusionApplyToAlbedo );
		SetKeyword( material, "NORMALMAP_ON", normalMapOn );
		SetKeyword( material, "NORMALMAP_COMPRESSED", normalMapCompressed );
		SetKeyword( material, "TEXTURESPACE_ORTHONORMALIZE", textureSpaceOrthonormalize );
		SetKeyword( material, "EMISSIVEMAP_ON", emissiveMapOn );
		SetKeyword( material, "WATERMAP_ON", waterMapOn );
		SetKeyword( material, "WATERMAP_COMPRESSED", waterMapCompressed );
		SetKeyword( material, "WATERMASKMAP_ON", waterMaskMapOn );
		SetKeyword( material, "UI_ON", uiOn );
		SetKeyword( material, "FORWARDSHADOWS_ON", forwardShadowsOn );

		// if either blending or ui is on then force material to be transparent (this also disables the deferred pass automatically)
		if ( blendEnabled || uiOn )
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

		// disable the shadow caster pass if the material does not want to cast shadows
		material.SetShaderPassEnabled( "ShadowCaster", castShadowsEnabled );

		if ( !castShadowsEnabled )
		{
			m_debugLogMessage[ 1 ] += " " + " SHADOWS:OFF";
		}

		// enabled or disable the blend pre pass
		material.SetShaderPassEnabled( "BlendPrePass", blendPrePassEnabled );

		if ( !blendPrePassEnabled )
		{
			m_debugLogMessage[ 1 ] += " " + " BLENDPREPASS:OFF";
		}

		// normalize the light direction
		var uiLightDirection = material.GetVector( "UILightDirection" );

		uiLightDirection = Vector3.Normalize( uiLightDirection );

		material.SetVector( "UILightDirection", uiLightDirection );

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
}
