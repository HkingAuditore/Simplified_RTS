Shader "Unlit/BrushPass"
{
    Properties
    {
		[Header(OutLine)]
		// Stroke Color
		_StrokeColor ("Stroke Color", Color) = (0,0,0,1)
		// Noise Map
		_OutlineNoise ("Outline Noise Map", 2D) = "white" {}
		_Outline ("Outline", Range(0, 1)) = 0.1
		// Outside Noise Width
		_OutsideNoiseWidth ("Outside Noise Width", Range(1, 2)) = 1.3
		_OutlineCut("Outside Cut", Range(0,1)) = .5
		_MaxOutlineZOffset ("Max Outline Z Offset", Range(0,1)) = 0.5
    }
    SubShader
    {
		Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
 
		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 
			CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float _BumpScale;
			float4 _EmissionColor;
			float _Smoothness;
			float _Specular;
			float _Cutoff;
		//ChinesePainting
			sampler2D _Ramp;
			float4 _Ramp_ST;
			sampler2D _StrokeTex;
			float4 _StrokeTex_ST;
			float radius;
            float resolution;
            //the direction of our blur
            //hstep (1.0, 0.0) -> x-axis blur
            //vstep(0.0, 1.0) -> y-axis blur
            //for example horizontaly blur equal:
            //float hstep = 1;
            //float vstep = 0;
            float hstep;
            float vstep;
			float _InteriorNoiseLevel;
			sampler2D _InteriorNoise;
			float _Outline;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			float _OutsideNoiseWidth;
			half _MaxOutlineZOffset;
			half _OutlineCut;
			CBUFFER_END
		ENDHLSL

		Pass {
			NAME "BrushPass"
			Tags { "LightMode"="SRPDefaultUnlit" }
 
			HLSLPROGRAM
 
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x gles
 
			//#pragma target 4.5 // https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html
 
			#pragma vertex vert
			#pragma fragment frag
 
			// Material Keywords
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			//#pragma shader_feature _METALLICSPECGLOSSMAP
			//#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			//#pragma shader_feature _OCCLUSIONMAP
			//#pragma shader_feature _ _CLEARCOAT _CLEARCOATMAP // URP v10+
 
			//#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
			//#pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
			//#pragma shader_feature _SPECULAR_SETUP
			#pragma shader_feature _RECEIVE_SHADOWS_OFF
 
			// URP Keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
 
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog
 
			// Includes
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
 
			struct Attributes {
				float4 positionOS   : POSITION;
				float3 normalOS		: NORMAL;
				float4 tangentOS	: TANGENT;
				float4 color		: COLOR;
				float2 uv           : TEXCOORD0;
				float2 lightmapUV   : TEXCOORD1;
			};
 
			struct Varyings {
				float4 positionCS				: SV_POSITION;
				float4 color					: COLOR;
				float2 uv					: TEXCOORD0;
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
 
				#ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
					float3 positionWS			: TEXCOORD2;
				#endif
 
				float3 normalWS					: TEXCOORD3;
				#ifdef _NORMALMAP
					float4 tangentWS 			: TEXCOORD4;
				#endif
 
				float3 viewDirWS 				: TEXCOORD5;
				half4 fogFactorAndVertexLight	: TEXCOORD6;
				half offsetPos : TEXCOORD7;// x: fogFactor, yzw: vertex light
 
				#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					float4 shadowCoord			: TEXCOORD8;
				#endif
			};
 
			// Automatically defined with SurfaceInput.hlsl
			//TEXTURE2D(_BaseMap);
			//SAMPLER(sampler_BaseMap);
 
			#if SHADER_LIBRARY_VERSION_MAJOR < 9
			// This function was added in URP v9.x.x versions, if we want to support URP versions before, we need to handle it instead.
			// Computes the world space view direction (pointing towards the viewer).
			float3 GetWorldSpaceViewDir(float3 positionWS) {
				if (unity_OrthoParams.w == 0) {
					// Perspective
					return _WorldSpaceCameraPos - positionWS;
				} else {
					// Orthographic
					float4x4 viewMat = GetWorldToViewMatrix();
					return viewMat[2].xyz;
				}
			}
			#endif
 
			Varyings vert(Attributes IN) {
				Varyings OUT;
 
				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				OUT.positionCS = positionInputs.positionCS;
				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
				OUT.color = IN.color;

				///ChinesePainting
				float4 burn = tex2Dlod(_OutlineNoise, IN.positionOS);
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, IN.normalOS);
				scaledir += 0.1;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);
				// camera space
				float4 position_cs = mul(UNITY_MATRIX_MV, IN.positionOS);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;
                
                // unity_CameraProjection[1].y = fov/2
				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.1 * _OutsideNoiseWidth ;
				// position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.1 * _OutsideNoiseWidth ;
				position_cs.z = offset_pos_cs.z;
				// OUT.offsetPos = burn.y * _Outline * 1.1 * _OutsideNoiseWidth;
				OUT.positionCS = mul(UNITY_MATRIX_P, position_cs);
				///ChinesePainting
				
				#ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
					OUT.positionWS = positionInputs.positionWS;
				#endif
 
				OUT.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
 
				VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
				OUT.normalWS =  normalInputs.normalWS;
				#ifdef _NORMALMAP
					real sign = IN.tangentOS.w * GetOddNegativeScale();
					OUT.tangentWS = half4(normalInputs.tangentWS.xyz, sign);
				#endif
 
				half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
				half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
 
				OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
 
				OUTPUT_LIGHTMAP_UV(IN.lightmapUV, unity_LightmapST, OUT.lightmapUV);
				OUTPUT_SH(OUT.normalWS.xyz, OUT.vertexSH);
 
				#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					OUT.shadowCoord = GetShadowCoord(positionInputs);
				#endif
 
				return OUT;
			}
 
			InputData InitializeInputData(Varyings IN, half3 normalTS){
				InputData inputData = (InputData)0;
 
				#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
					inputData.positionWS = IN.positionWS;
				#endif
 
				half3 viewDirWS = SafeNormalize(IN.viewDirWS);
				#ifdef _NORMALMAP
					float sgn = IN.tangentWS.w; // should be either +1 or -1
					float3 bitangent = sgn * cross(IN.normalWS.xyz, IN.tangentWS.xyz);
					inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(IN.tangentWS.xyz, bitangent.xyz, IN.normalWS.xyz));
				#else
					inputData.normalWS = IN.normalWS;
				#endif
 
				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = viewDirWS;
 
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
					inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif

				//ChinesePainting
				
 
				inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.vertexSH, inputData.normalWS);
				return inputData;
			}
 
			SurfaceData InitializeSurfaceData(Varyings IN){
				SurfaceData surfaceData = (SurfaceData)0;
				// Note, we can just use SurfaceData surfaceData; here and not set it.
				// However we then need to ensure all values in the struct are set before returning.
				// By casting 0 to SurfaceData, we automatically set all the contents to 0.
 
				half4 albedoAlpha = SampleAlbedoAlpha(IN.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
				surfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
				surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb * IN.color.rgb;
 
				// For the sake of simplicity I'm not supporting the metallic/specular map or occlusion map
				// for an example of that see : https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl
 
				surfaceData.smoothness = _Smoothness;
				surfaceData.normalTS = SampleNormal(IN.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
				surfaceData.emission = SampleEmission(IN.uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
				surfaceData.specular = _Specular;
				surfaceData.occlusion = 1;
 
				return surfaceData;
			}
 
			half4 frag(Varyings IN) : SV_Target {
				half4 c = _StrokeColor;
				half3 burn = tex2D(_OutlineNoise, IN.uv).rgb;
				if (burn.x > _OutlineCut)
					discard;
				return c;
			}
			ENDHLSL
		}
    }
}
