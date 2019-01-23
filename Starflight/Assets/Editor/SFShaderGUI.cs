
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
		public static readonly GUIContent detailAlbedoText = EditorGUIUtility.TrTextContent( "Detail Albedo", "RGB=Color, A=Transparency" );
		public static readonly GUIContent specularText = EditorGUIUtility.TrTextContent( "Specular", "RGB=Color, A=Smoothness" );
		public static readonly GUIContent normalText = EditorGUIUtility.TrTextContent( "Normal", "RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; Strength" );
		public static readonly GUIContent detailNormalText = EditorGUIUtility.TrTextContent( "Detail Normal", "RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; Strength" );
		public static readonly GUIContent emissiveText = EditorGUIUtility.TrTextContent( "Emissive", "UV1; RGB=Color" );

		public static readonly string scaleOffsetText = "\nTexture Scale and Offset";

		public static readonly string uv2MapsText = "\nUV2 Maps";

		public static readonly GUIContent occlusionText = EditorGUIUtility.TrTextContent( "Occlusion", "R=Intensity; Albedo Occlusion Switch" );

		public static readonly string cullingOptionsText = "\nCulling Options";

		public static readonly GUIContent cullModeText = EditorGUIUtility.TrTextContent( "Cull Mode", "" );

		public static readonly string blendingOptionsText = "\nBlending Options";

		public static readonly GUIContent blendSrcText = EditorGUIUtility.TrTextContent( "Source Blend", "" );
		public static readonly GUIContent blendDstText = EditorGUIUtility.TrTextContent( "Destination Blend", "" );

		public static readonly string alphaTestingOptionsText = "\nAlpha Testing Options";

		public static readonly GUIContent alphaTestValueText = EditorGUIUtility.TrTextContent( "Alpha Test Value", "" );

		public static readonly string depthBufferOptionsText = "\nDepth Buffer Options";

		public static readonly GUIContent zWriteText = EditorGUIUtility.TrTextContent( "Z Write", "" );
		public static readonly GUIContent zTestText = EditorGUIUtility.TrTextContent( "Z Test", "" );
		public static readonly GUIContent overrideDepthOutputText = EditorGUIUtility.TrTextContent( "Override Depth Output", "Turn this on to write maximum Z to the depth buffer instead of the actual Z." );

		public static readonly string depthFadeOptionsText = "\nDepth Fade Options";
		public static readonly GUIContent depthFadeText = EditorGUIUtility.TrTextContent( "Depth Fade", "" );
		public static readonly GUIContent depthFadeParamsText = EditorGUIUtility.TrTextContent( "Depth Fade Params", "X=Near Transparent, Y=Near Opaque, Z=Far Opaque, W=Far Transparent" );

		public static readonly string miscRenderingOptionsText = "\nMisc Rendering Options";

		public static readonly GUIContent orthonormalizeText = EditorGUIUtility.TrTextContent( "Orthonormalize", "Orthonormalize the normal at every pixel." );
		public static readonly GUIContent emissiveProjectionText = EditorGUIUtility.TrTextContent( "Emissive Projection", "When turned on, UV coordinates for the emissive map are generated from a projection matrix." );
		public static readonly GUIContent forwardShadowsText = EditorGUIUtility.TrTextContent( "Forward Shadows", "Receive shadows when forward rendering (only works for opaque materials)." );
		public static readonly GUIContent behindEverythingText = EditorGUIUtility.TrTextContent( "Behind Everything", "Forces this material to render into the background like a skybox." );
		public static readonly GUIContent fractalDetailsText = EditorGUIUtility.TrTextContent( "Fractal Details", "Applies fractal details to the surface for extreme close ups." );
	}

	MaterialProperty m_scatterMapA = null;
	MaterialProperty m_scatterMapB = null;
	MaterialProperty m_densityMap = null;
	MaterialProperty m_density = null;
	MaterialProperty m_speed = null;

	MaterialProperty m_waterMaskMap = null;

	MaterialProperty m_albedoMap = null;
	MaterialProperty m_detailAlbedoMap = null;
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

	MaterialProperty m_occlusionMap = null;
	MaterialProperty m_occlusionPower = null;
	MaterialProperty m_albedoOcclusionOn = null;

	MaterialProperty m_cullMode = null;

	MaterialProperty m_blendSrc = null;
	MaterialProperty m_blendDst = null;

	MaterialProperty m_alphaTestValue = null;

	MaterialProperty m_zWriteOn = null;
	MaterialProperty m_zTest = null;
	MaterialProperty m_overrideDepthOutput = null;

	MaterialProperty m_depthFadeOn = null;
	MaterialProperty m_depthFadeParams = null;

	MaterialProperty m_orthonormalizeOn = null;
	MaterialProperty m_emissiveProjectionOn = null;
	MaterialProperty m_forwardShadowsOn = null;
	MaterialProperty m_behindEverythingOn = null;
	MaterialProperty m_fractalDetailsOn = null;

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
			MaterialChanged( material );

			m_firstTimeApply = false;
		}

		// use default label width
		EditorGUIUtility.labelWidth = 0.0f;

		// detect any changes to the material
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

		// albedo options
		if ( m_albedoMap != null && m_albedoColor != null )
		{
			GUILayout.Label( Styles.albedoText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.albedoText, m_albedoMap, m_albedoColor );

			if ( m_albedoMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_albedoMap );
			}
		}

		// detail albedo options
		if ( m_detailAlbedoMap != null )
		{
			GUILayout.Label( Styles.detailAlbedoText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.detailAlbedoText, m_detailAlbedoMap );

			if ( m_detailAlbedoMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_detailAlbedoMap );
			}
		}

		// specular options
		if ( m_specularMap != null && m_specularColor != null && m_smoothness != null )
		{
			GUILayout.Label( Styles.specularText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.specularText, m_specularMap, m_specularColor, m_smoothness );

			if ( m_specularMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_specularMap );
			}
		}

		// normal options
		if ( m_normalMap != null && m_normalMapStrength != null )
		{
			GUILayout.Label( Styles.normalText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.normalText, m_normalMap, m_normalMapStrength );

			if ( m_normalMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_normalMap );
			}
		}

		// detail normal options
		if ( m_detailNormalMap != null && m_detailNormalMapStrength != null )
		{
			GUILayout.Label( Styles.detailNormalText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.detailNormalText, m_detailNormalMap, m_detailNormalMapStrength );

			if ( m_detailNormalMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_detailNormalMap );
			}
		}

		// emissive options
		if ( m_emissiveMap != null && m_emissiveColor != null )
		{
			GUILayout.Label( Styles.emissiveText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.emissiveText, m_emissiveMap, m_emissiveColor );

			if ( m_emissiveMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_emissiveMap );
			}
		}

		// water mask map options
		if ( m_waterMaskMap != null )
		{
			GUILayout.Label( Styles.waterMaskText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.waterMaskText, m_waterMaskMap );

			if ( m_waterMaskMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_waterMaskMap );
			}
		}

		// uv2 map options
		if ( m_occlusionMap != null && m_occlusionPower != null && m_albedoOcclusionOn != null )
		{
			GUILayout.Label( Styles.occlusionText, EditorStyles.boldLabel );

			m_materialEditor.TexturePropertySingleLine( Styles.occlusionText, m_occlusionMap, m_occlusionPower, m_albedoOcclusionOn );

			if ( m_occlusionMap.textureValue != null )
			{
				m_materialEditor.TextureScaleOffsetProperty( m_occlusionMap );
			}
		}

		// culling options
		if ( m_cullMode != null )
		{
			GUILayout.Label( Styles.cullingOptionsText, EditorStyles.boldLabel );

			m_materialEditor.ShaderProperty( m_cullMode, Styles.cullModeText );
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
		if ( m_zWriteOn != null || m_zTest != null || m_overrideDepthOutput != null )
		{
			GUILayout.Label( Styles.depthBufferOptionsText, EditorStyles.boldLabel );

			if ( m_zWriteOn != null )
			{
				m_materialEditor.ShaderProperty( m_zWriteOn, Styles.zWriteText );
			}

			if ( m_zTest != null )
			{
				m_materialEditor.ShaderProperty( m_zTest, Styles.zTestText );
			}

			if ( m_overrideDepthOutput != null )
			{
				m_materialEditor.ShaderProperty( m_overrideDepthOutput, Styles.overrideDepthOutputText );
			}
		}

		// depth fade options
		if ( m_depthFadeOn != null && m_depthFadeParams != null )
		{
			GUILayout.Label( Styles.depthFadeOptionsText, EditorStyles.boldLabel );

			m_materialEditor.ShaderProperty( m_depthFadeOn, Styles.depthFadeText );

			if ( IsSwitchedOn( material, "SF_DepthFadeOn" ) )
			{
				m_materialEditor.ShaderProperty( m_depthFadeParams, Styles.depthFadeParamsText );
			}
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

		if ( m_behindEverythingOn != null )
		{
			m_materialEditor.ShaderProperty( m_behindEverythingOn, Styles.behindEverythingText );
		}

		if ( m_fractalDetailsOn != null )
		{
			m_materialEditor.ShaderProperty( m_fractalDetailsOn, Styles.fractalDetailsText );
		}

		m_materialEditor.RenderQueueField();

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

		m_albedoMap = FindProperty( "_MainTex", materialPropertyList, false );
		m_detailAlbedoMap = FindProperty( "_DetailAlbedoMap", materialPropertyList, false );
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

		m_occlusionMap = FindProperty( "SF_OcclusionMap", materialPropertyList, false );
		m_occlusionPower = FindProperty( "SF_OcclusionPower", materialPropertyList, false );
		m_albedoOcclusionOn = FindProperty( "SF_AlbedoOcclusionOn", materialPropertyList, false );

		m_cullMode = FindProperty( "SF_CullMode", materialPropertyList, false );

		m_blendSrc = FindProperty( "SF_BlendSrc", materialPropertyList, false );
		m_blendDst = FindProperty( "SF_BlendDst", materialPropertyList, false );

		m_alphaTestValue = FindProperty( "SF_AlphaTestValue", materialPropertyList, false );

		m_zWriteOn = FindProperty( "SF_ZWriteOn", materialPropertyList, false );
		m_zTest = FindProperty( "SF_ZTest", materialPropertyList, false );
		m_overrideDepthOutput = FindProperty( "SF_OverrideDepthOutput", materialPropertyList, false );

		m_depthFadeOn = FindProperty( "SF_DepthFadeOn", materialPropertyList, false );
		m_depthFadeParams = FindProperty( "SF_DepthFadeParams", materialPropertyList, false );

		m_orthonormalizeOn = FindProperty( "SF_OrthonormalizeOn", materialPropertyList, false );
		m_emissiveProjectionOn = FindProperty( "SF_EmissiveProjectionOn", materialPropertyList, false );
		m_forwardShadowsOn = FindProperty( "SF_ForwardShadowsOn", materialPropertyList, false );
		m_behindEverythingOn = FindProperty( "SF_BehindEverythingOn", materialPropertyList, false );
		m_fractalDetailsOn = FindProperty( "SF_FractalDetailsOn", materialPropertyList, false );
	}

	void MaterialChanged( Material material )
	{
		// copy old albedo map property to new albedo map property
		if ( material.HasProperty( "SF_AlbedoMap" ) && material.HasProperty( "_MainTex" ) )
		{
			if ( HasTextureMap( material, "SF_AlbedoMap" ) )
			{
				if ( !HasTextureMap( material, "_MainTex" ) )
				{
					var textureMap = material.GetTexture( "SF_AlbedoMap" );

					material.SetTexture( "_MainTex", textureMap );

					m_albedoMap.textureValue = textureMap;

					Debug.Log( "Transferred albedo map for material " + material.name + "." );
				}
			}
		}

		// copy old detail albedo map property to new detail albedo map property
		if ( material.HasProperty( "SF_DetailAlbedoMap" ) && material.HasProperty( "_DetailAlbedoMap" ) )
		{
			if ( HasTextureMap( material, "SF_DetailAlbedoMap" ) )
			{
				if ( !HasTextureMap( material, "_DetailAlbedoMap" ) )
				{
					var textureMap = material.GetTexture( "SF_DetailAlbedoMap" );

					material.SetTexture( "_DetailAlbedoMap", textureMap );

					m_detailAlbedoMap.textureValue = textureMap;

					Debug.Log( "Transferred detail albedo map for material " + material.name + "." );
				}
			}
		}

		// copy old base scale transform property
		if ( material.HasProperty( "SF_BaseScaleOffset" ) )
		{
			var scaleAndOffset = material.GetVector( "SF_BaseScaleOffset" );

			if ( m_albedoMap != null )
			{
				if ( m_albedoMap.textureScaleAndOffset != scaleAndOffset )
				{
					material.SetVector( "_MainTex_ST", scaleAndOffset );

					m_albedoMap.textureScaleAndOffset = scaleAndOffset;

					Debug.Log( "Transferred albedo map scale and offset for material " + material.name + "." );
				}
			}

			if ( m_specularMap != null )
			{
				if ( m_specularMap.textureScaleAndOffset != scaleAndOffset )
				{
					material.SetVector( "SF_SpecularMap_ST", scaleAndOffset );

					m_specularMap.textureScaleAndOffset = scaleAndOffset;

					Debug.Log( "Transferred specular map scale and offset for material " + material.name + "." );
				}
			}

			if ( m_normalMap != null )
			{
				if ( m_normalMap.textureScaleAndOffset != scaleAndOffset )
				{
					material.SetVector( "SF_NormalMap_ST", scaleAndOffset );

					m_normalMap.textureScaleAndOffset = scaleAndOffset;

					Debug.Log( "Transferred normal map scale and offset for material " + material.name + "." );
				}
			}

			if ( m_emissiveMap != null )
			{
				if ( m_emissiveMap.textureScaleAndOffset != scaleAndOffset )
				{
					material.SetVector( "SF_EmissiveMap_ST", scaleAndOffset );

					m_emissiveMap.textureScaleAndOffset = scaleAndOffset;

					Debug.Log( "Transferred emissive map scale and offset for material " + material.name + "." );
				}
			}
		}

		// copy old detail scale transform property
		if ( material.HasProperty( "SF_DetailScaleOffset" ) )
		{
			var scaleAndOffset = material.GetVector( "SF_DetailScaleOffset" );

			if ( m_detailAlbedoMap != null )
			{
				if ( m_detailAlbedoMap.textureScaleAndOffset != scaleAndOffset )
				{
					material.SetVector( "_DetailAlbedoMap_ST", scaleAndOffset );

					m_detailAlbedoMap.textureScaleAndOffset = scaleAndOffset;

					Debug.Log( "Transferred detail albedo map scale and offset for material " + material.name + "." );
				}
			}

			if ( m_detailNormalMap != null )
			{
				if ( m_detailNormalMap.textureScaleAndOffset != scaleAndOffset )
				{
					material.SetVector( "_DetailNormalMap_ST", scaleAndOffset );

					m_detailNormalMap.textureScaleAndOffset = scaleAndOffset;

					Debug.Log( "Transferred detail normal map scale and offset for material " + material.name + "." );
				}
			}
		}

		// texture maps on/off
		bool waterMaskMapOn = HasTextureMap( material, "SF_WaterMaskMap" );
		bool albedoMapOn = HasTextureMap( material, "_MainTex" );
		bool detailAlbedoMapOn = HasTextureMap( material, "_DetailAlbedoMap" );
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
		bool overrideDepthOutputOn = IsSwitchedOn( material, "SF_OverrideDepthOutput" );
		bool depthFadeOn = IsSwitchedOn( material, "SF_DepthFadeOn" );
		bool orthonormalizeOn = IsSwitchedOn( material, "SF_OrthonormalizeOn" );
		bool emissiveProjectionOn = IsSwitchedOn( material, "SF_EmissiveProjectionOn" );
		bool forwardShadowsOn = IsSwitchedOn( material, "SF_ForwardShadowsOn" );
		bool behindEverythingOn = IsSwitchedOn( material, "SF_BehindEverythingOn" );
		bool fractalDetailsOn = IsSwitchedOn( material, "SF_FractalDetailsOn" );

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
		SetKeyword( material, "SF_DETAILALBEDOMAP_ON", detailAlbedoMapOn );
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
		SetKeyword( material, "SF_OVERRIDEDEPTHOUTPUT_ON", overrideDepthOutputOn );
		SetKeyword( material, "SF_DEPTHFADE_ON", depthFadeOn );
		SetKeyword( material, "SF_ORTHONORMALIZE_ON", orthonormalizeOn );
		SetKeyword( material, "SF_EMISSIVEPROJECTION_ON", emissiveProjectionOn );
		SetKeyword( material, "SF_FORWARDSHADOWS_ON", forwardShadowsOn );
		SetKeyword( material, "SF_BEHINDEVERYTHING_ON", behindEverythingOn );
		SetKeyword( material, "SF_FRACTALDETAILS_ON", fractalDetailsOn );

		// if blending is on then force material to be transparent (this also disables the deferred pass automatically)
		if ( blendOn )
		{
			material.SetOverrideTag( "RenderType", "Transparent" );

			m_debugLogMessage[ 1 ] += " TYPE:TRANSPARENT QUEUE:" + material.renderQueue;
		}
		else if ( alphaTestOn )
		{
			material.SetOverrideTag( "RenderType", "TransparentCutout" );

			m_debugLogMessage[ 1 ] += " TYPE:TRANSPARENTCUTOUT QUEUE:" + material.renderQueue;
		}
		else
		{
			material.SetOverrideTag( "RenderType", "" );

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
