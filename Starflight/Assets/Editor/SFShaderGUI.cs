
using UnityEngine;
using UnityEditor;

class SFShaderGUI : ShaderGUI
{
	private static class Styles
	{
		public static readonly string mainText = "Starflight Shader Properties";

		public static readonly string cloudOptionsText = "\nCloud Options";

		public static readonly GUIContent scatterMapText = EditorGUIUtility.TrTextContent( "Scatter Map", "RGB=Height" );
		public static readonly GUIContent densityMapText = EditorGUIUtility.TrTextContent( "Density Map", "RGB=Height" );
		public static readonly GUIContent densityText = EditorGUIUtility.TrTextContent( "Density", "" );

		public static readonly string animationOptionsText = "\nAnimation Options";

		public static readonly GUIContent speedText = EditorGUIUtility.TrTextContent( "Speed", "" );

		public static readonly string uv1MapsText = "\nUV1 Maps";

		public static readonly GUIContent waterMaskText = EditorGUIUtility.TrTextContent( "Water Mask", "R=Opacity" );
		public static readonly GUIContent albedoText = EditorGUIUtility.TrTextContent( "Albedo", "RGB=Color, A=Transparency" );
		public static readonly GUIContent specularText = EditorGUIUtility.TrTextContent( "Specular", "RGB=Color, A=Smoothness" );
		public static readonly GUIContent normalText = EditorGUIUtility.TrTextContent( "Normal", "RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; Strength" );
		public static readonly GUIContent detailNormalText = EditorGUIUtility.TrTextContent( "Detail Normal", "RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; Strength" );
		public static readonly GUIContent emissiveText = EditorGUIUtility.TrTextContent( "Emissive", "UV1; RGB=Color" );
		public static readonly GUIContent baseScaleOffsetText = EditorGUIUtility.TrTextContent( "Base Scale & Offset", "XY=Scale, ZW=Offset" );
		public static readonly GUIContent detailScaleOffsetText = EditorGUIUtility.TrTextContent( "Detail Scale & Offset", "XY=Scale, ZW=Offset" );

		public static readonly string uv2MapsText = "\nUV2 Maps";

		public static readonly GUIContent occlusionText = EditorGUIUtility.TrTextContent( "Occlusion", "R=Intensity; Albedo Occlusion Switch" );

		public static readonly string blendingOptionsText = "\nBlending Options";

		public static readonly GUIContent blendSrcText = EditorGUIUtility.TrTextContent( "Source Blend", "" );
		public static readonly GUIContent blendDstText = EditorGUIUtility.TrTextContent( "Destination Blend", "" );

		public static readonly string alphaTestingOptionsText = "\nAlpha Testing Options";

		public static readonly GUIContent alphaTestValueText = EditorGUIUtility.TrTextContent( "Alpha Test Value", "" );

		public static readonly string depthBufferOptionsText = "\nDepth Buffer Options";

		public static readonly GUIContent zWriteText = EditorGUIUtility.TrTextContent( "Z Write", "" );

		public static readonly string miscRenderingOptionsText = "\nMisc Rendering Options";

		public static readonly GUIContent orthonormalizeText = EditorGUIUtility.TrTextContent( "Orthonormalize", "" );
		public static readonly GUIContent emissiveProjectionText = EditorGUIUtility.TrTextContent( "Emissive Projection", "When turned on, UV coordinates for the emissive map are generated from a projection matrix." );
		public static readonly GUIContent forwardShadowsText = EditorGUIUtility.TrTextContent( "Forward Shadows", "Receive shadows when forward rendering (only works for opaque materials)." );
		public static readonly GUIContent renderQueueOffsetText = EditorGUIUtility.TrTextContent( "Render Queue Offset", "" );
	}

	MaterialProperty m_scatterMapA = null;
	MaterialProperty m_scatterMapB = null;
	MaterialProperty m_densityMap = null;
	MaterialProperty m_density = null;
	MaterialProperty m_speed = null;

	MaterialProperty m_waterMaskMap = null;

	MaterialProperty m_albedoMap = null;
	MaterialProperty m_albedoColor = null;

	MaterialProperty m_specularMap = null;
	MaterialProperty m_specularColor = null;
	MaterialProperty m_smoothness = null;

	MaterialProperty m_normalMap = null;
	MaterialProperty m_normalMapStrength = null;

	MaterialProperty m_detailNormalMap = null;
	MaterialProperty m_detailNormalMapStrength = null;

	MaterialProperty m_emissiveMap = null;
	MaterialProperty m_emissiveColor = null;

	MaterialProperty m_baseScaleOffset = null;
	MaterialProperty m_detailScaleOffset = null;

	MaterialProperty m_occlusionMap = null;
	MaterialProperty m_occlusionPower = null;
	MaterialProperty m_albedoOcclusionOn = null;

	MaterialProperty m_blendSrc = null;
	MaterialProperty m_blendDst = null;

	MaterialProperty m_alphaTestValue = null;

	MaterialProperty m_zWriteOn = null;

	MaterialProperty m_orthonormalizeOn = null;
	MaterialProperty m_emissiveProjectionOn = null;
	MaterialProperty m_forwardShadowsOn = null;
	MaterialProperty m_renderQueueOffset = null;

	MaterialEditor m_materialEditor;

	bool m_firstTimeApply = true;

	string[] m_debugLogMessage;

	public SFShaderGUI()
	{
		m_debugLogMessage = new string[ 2 ];
	}

	public override void OnGUI( MaterialEditor materialEditor, MaterialProperty[] materialPropertyList )
	{
		FindProperties( materialPropertyList );

		m_materialEditor = materialEditor;

		Material material = materialEditor.target as Material;

		if ( m_firstTimeApply )
		{
			m_firstTimeApply = false;

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

		// hello
		GUILayout.Label( Styles.mainText, EditorStyles.boldLabel );

		// cloud options
		if ( m_scatterMapA != null && m_scatterMapB != null && m_densityMap != null && m_density != null )
		{
			GUILayout.Label( Styles.cloudOptionsText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.scatterMapText, m_scatterMapA );
			m_materialEditor.TexturePropertySingleLine( Styles.scatterMapText, m_scatterMapB );
			m_materialEditor.TexturePropertySingleLine( Styles.densityMapText, m_densityMap );
			m_materialEditor.ShaderProperty( m_density, Styles.densityText );
		}

		// animation options
		if ( m_speed != null )
		{
			GUILayout.Label( Styles.animationOptionsText, EditorStyles.boldLabel );

			m_materialEditor.ShaderProperty( m_speed, Styles.speedText );
		}

		// uv1 map options
		if ( ( m_albedoMap != null && m_albedoColor != null ) || ( m_specularMap != null && m_specularColor != null && m_smoothness != null ) || ( m_normalMap != null && m_normalMapStrength != null ) || ( m_detailNormalMap != null && m_detailNormalMapStrength != null ) )
		{
			GUILayout.Label( Styles.uv1MapsText, EditorStyles.boldLabel );

			if ( m_waterMaskMap != null )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.waterMaskText, m_waterMaskMap );
			}

			if ( m_albedoMap != null && m_albedoColor != null )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.albedoText, m_albedoMap, m_albedoColor );
			}

			if ( m_specularMap != null && m_specularColor != null && m_smoothness != null )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.specularText, m_specularMap, m_specularColor, m_smoothness );
			}

			if ( m_normalMap != null && m_normalMapStrength != null )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.normalText, m_normalMap, m_normalMapStrength );
			}

			if ( m_detailNormalMap != null && m_detailNormalMapStrength != null )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.detailNormalText, m_detailNormalMap, m_detailNormalMapStrength );
			}

			if ( m_emissiveMap != null && m_emissiveColor != null )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.emissiveText, m_emissiveMap, m_emissiveColor );
			}

			if ( m_baseScaleOffset != null )
			{
				m_materialEditor.ShaderProperty( m_baseScaleOffset, Styles.baseScaleOffsetText );
			}

			if ( m_detailScaleOffset != null )
			{
				m_materialEditor.ShaderProperty( m_detailScaleOffset, Styles.detailScaleOffsetText );
			}
		}

		// uv2 map options
		if ( m_occlusionMap != null && m_occlusionPower != null && m_albedoOcclusionOn != null )
		{
			GUILayout.Label( Styles.uv2MapsText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.occlusionText, m_occlusionMap, m_occlusionPower, m_albedoOcclusionOn );
		}

		// blending options
		if ( m_blendSrc != null && m_blendDst != null )
		{
			GUILayout.Label( Styles.blendingOptionsText, EditorStyles.boldLabel );

			m_materialEditor.ShaderProperty( m_blendSrc, Styles.blendSrcText );
			m_materialEditor.ShaderProperty( m_blendDst, Styles.blendDstText );
		}

		// alpha testing options
		if ( m_alphaTestValue != null )
		{
			GUILayout.Label( Styles.alphaTestingOptionsText, EditorStyles.boldLabel );

			m_materialEditor.ShaderProperty( m_alphaTestValue, Styles.alphaTestValueText );
		}

		// depth buffer options
		if ( m_zWriteOn != null )
		{
			GUILayout.Label( Styles.depthBufferOptionsText, EditorStyles.boldLabel );

			m_materialEditor.ShaderProperty( m_zWriteOn, Styles.zWriteText );
		}

		// misc rendering options
		GUILayout.Label( Styles.miscRenderingOptionsText, EditorStyles.boldLabel );

		if ( m_orthonormalizeOn != null )
		{
			m_materialEditor.ShaderProperty( m_orthonormalizeOn, Styles.orthonormalizeText );
		}

		if ( m_emissiveProjectionOn != null )
		{
			m_materialEditor.ShaderProperty( m_emissiveProjectionOn, Styles.emissiveProjectionText );
		}

		if ( m_forwardShadowsOn != null )
		{
			m_materialEditor.ShaderProperty( m_forwardShadowsOn, Styles.forwardShadowsText );
		}

		m_materialEditor.ShaderProperty( m_renderQueueOffset, Styles.renderQueueOffsetText );

		// call material changed function if something was updated
		if ( EditorGUI.EndChangeCheck() )
		{
			MaterialChanged( material );
		}
	}

	void FindProperties( MaterialProperty[] materialPropertyList )
	{
		m_scatterMapA = FindProperty( "SF_ScatterMapA", materialPropertyList, false );
		m_scatterMapB = FindProperty( "SF_ScatterMapB", materialPropertyList, false );
		m_densityMap = FindProperty( "SF_DensityMap", materialPropertyList, false );
		m_density = FindProperty( "SF_Density", materialPropertyList, false );
		m_speed = FindProperty( "SF_Speed", materialPropertyList, false );

		m_waterMaskMap = FindProperty( "SF_WaterMaskMap", materialPropertyList, false );

		m_albedoMap = FindProperty( "SF_AlbedoMap", materialPropertyList, false );
		m_albedoColor = FindProperty( "SF_AlbedoColor", materialPropertyList, false );

		m_specularMap = FindProperty( "SF_SpecularMap", materialPropertyList, false );
		m_specularColor = FindProperty( "SF_SpecularColor", materialPropertyList, false );
		m_smoothness = FindProperty( "SF_Smoothness", materialPropertyList, false );

		m_normalMap = FindProperty( "SF_NormalMap", materialPropertyList, false );
		m_normalMapStrength = FindProperty( "SF_NormalMapStrength", materialPropertyList, false );

		m_detailNormalMap = FindProperty( "SF_DetailNormalMap", materialPropertyList, false );
		m_detailNormalMapStrength = FindProperty( "SF_DetailNormalMapStrength", materialPropertyList, false );

		m_emissiveMap = FindProperty( "SF_EmissiveMap", materialPropertyList, false );
		m_emissiveColor = FindProperty( "SF_EmissiveColor", materialPropertyList, false );

		m_baseScaleOffset = FindProperty( "SF_BaseScaleOffset", materialPropertyList, false );
		m_detailScaleOffset = FindProperty( "SF_DetailScaleOffset", materialPropertyList, false );

		m_occlusionMap = FindProperty( "SF_OcclusionMap", materialPropertyList, false );
		m_occlusionPower = FindProperty( "SF_OcclusionPower", materialPropertyList, false );
		m_albedoOcclusionOn = FindProperty( "SF_AlbedoOcclusionOn", materialPropertyList, false );

		m_blendSrc = FindProperty( "SF_BlendSrc", materialPropertyList, false );
		m_blendDst = FindProperty( "SF_BlendDst", materialPropertyList, false );

		m_alphaTestValue = FindProperty( "SF_AlphaTestValue", materialPropertyList, false );

		m_zWriteOn = FindProperty( "SF_ZWriteOn", materialPropertyList, false );

		m_orthonormalizeOn = FindProperty( "SF_OrthonormalizeOn", materialPropertyList, false );
		m_emissiveProjectionOn = FindProperty( "SF_EmissiveProjectionOn", materialPropertyList, false );
		m_forwardShadowsOn = FindProperty( "SF_ForwardShadowsOn", materialPropertyList, false );
		m_renderQueueOffset = FindProperty( "SF_RenderQueueOffset", materialPropertyList );
	}

	void MaterialChanged( Material material )
	{
		// texture maps on/off
		bool waterMaskMapOn = HasTextureMap( material, "SF_WaterMaskMap" );
		bool albedoMapOn = HasTextureMap( material, "SF_AlbedoMap" );
		bool specularMapOn = HasTextureMap( material, "SF_SpecularMap" );
		bool occlusionMapOn = HasTextureMap( material, "SF_OcclusionMap" );
		bool normalMapOn = HasTextureMap( material, "SF_NormalMap" );
		bool detailNormalMapOn = HasTextureMap( material, "SF_DetailNormalMap" );
		bool emissiveMapOn = HasTextureMap( material, "SF_EmissiveMap" );

		// texture compression
		bool normalMapIsCompressed = TextureIsCompressed( material, "SF_NormalMap" );
		bool detailNormalMapIsCompressed = TextureIsCompressed( material, "SF_DetailNormalMap" );

		// toggle switches on/off
		bool albedoOcclusionOn = IsSwitchedOn( material, "SF_AlbedoOcclusionOn" );
		bool orthonormalizeOn = IsSwitchedOn( material, "SF_OrthonormalizeOn" );
		bool emissiveProjectionOn = IsSwitchedOn( material, "SF_EmissiveProjectionOn" );
		bool forwardShadowsOn = IsSwitchedOn( material, "SF_ForwardShadowsOn" );

		// enable alpha testing if alpha test value > 0
		bool alphaTestOn = false;

		if ( material.HasProperty( "SF_AlphaTestValue" ) )
		{
			alphaTestOn = ( material.GetFloat( "SF_AlphaTestValue" ) > 0.0f );
		}

		// enable blending if blend src != one or blend dst != zero
		bool blendOn = false;

		if ( material.HasProperty( "SF_BlendSrc" ) && material.HasProperty( "SF_BlendDst" ) )
		{
			blendOn = ( ( (int) material.GetFloat( "SF_BlendSrc" ) != (int) UnityEngine.Rendering.BlendMode.One ) || ( (int) material.GetFloat( "SF_BlendDst" ) != (int) UnityEngine.Rendering.BlendMode.Zero ) );
		}

		// enable specular if specular color is not black
		bool specularOn = false;

		if ( material.HasProperty( "SF_SpecularColor" ) )
		{
			specularOn = ( material.GetColor( "SF_SpecularColor" ) != Color.black );
		}

		// if specular is not on then there is no point in having a specular map
		if ( !specularOn )
		{
			specularMapOn = false;
		}

		// if there is no detail normal map then there is no point in having a water mask map
		if ( !detailNormalMapOn )
		{
			waterMaskMapOn = false;
		}

		// for debug log
		m_debugLogMessage[ 1 ] = "Enabled keywords:";

		// set all the material keywords
		SetKeyword( material, "SF_WATERMASKMAP_ON", waterMaskMapOn );
		SetKeyword( material, "SF_ALBEDOMAP_ON", albedoMapOn );
		SetKeyword( material, "SF_SPECULARMAP_ON", specularMapOn );
		SetKeyword( material, "SF_NORMALMAP_ON", normalMapOn );
		SetKeyword( material, "SF_NORMALMAP_ISCOMPRESSED", normalMapIsCompressed );
		SetKeyword( material, "SF_DETAILNORMALMAP_ON", detailNormalMapOn );
		SetKeyword( material, "SF_DETAILNORMALMAP_ISCOMPRESSED", detailNormalMapIsCompressed );
		SetKeyword( material, "SF_EMISSIVEMAP_ON", emissiveMapOn );
		SetKeyword( material, "SF_OCCLUSIONMAP_ON", occlusionMapOn );
		SetKeyword( material, "SF_ALBEDOOCCLUSION_ON", albedoOcclusionOn );
		SetKeyword( material, "SF_SPECULAR_ON", specularOn );
		SetKeyword( material, "SF_ALPHA_ON", blendOn || alphaTestOn );
		SetKeyword( material, "SF_ALPHATEST_ON", alphaTestOn );
		SetKeyword( material, "SF_ORTHONORMALIZE_ON", orthonormalizeOn );
		SetKeyword( material, "SF_EMISSIVEPROJECTION_ON", emissiveProjectionOn );
		SetKeyword( material, "SF_FORWARDSHADOWS_ON", forwardShadowsOn );

		// if blending is on then force material to be transparent (this also disables the deferred pass automatically)
		if ( blendOn )
		{
			material.SetOverrideTag( "RenderType", "Transparent" );
			material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent + Mathf.RoundToInt( m_renderQueueOffset.floatValue );

			m_debugLogMessage[ 1 ] += " TYPE:TRANSPARENT QUEUE:" + material.renderQueue;
		}
		else if ( alphaTestOn )
		{
			material.SetOverrideTag( "RenderType", "TransparentCutout" );
			material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.AlphaTest + Mathf.RoundToInt( m_renderQueueOffset.floatValue );

			m_debugLogMessage[ 1 ] += " TYPE:TRANSPARENTCUTOUT QUEUE:" + material.renderQueue;
		}
		else
		{
			material.SetOverrideTag( "RenderType", "" );
			material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Geometry + Mathf.RoundToInt( m_renderQueueOffset.floatValue );

			m_debugLogMessage[ 1 ] += " TYPE:OPAQUE QUEUE:" + material.renderQueue;
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
