Shader "Unlit/ChinesePainting"
{
	Properties {
		_BaseMap ("Base Texture", 2D) = "white" {}
		_BaseColor ("Example Colour", Color) = (0, 0.66, 0.73, 1)
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_Specular ("Specular", Range(0, 1)) = 0.5
 
		[Toggle(_ALPHATEST_ON)] _EnableAlphaTest("Enable Alpha Cutoff", Float) = 0.0
		_Cutoff ("Alpha Cutoff", Float) = 0.5
 
		[Toggle(_NORMALMAP)] _EnableBumpMap("Enable Normal/Bump Map", Float) = 0.0
		_BumpMap ("Normal/Bump Texture", 2D) = "bump" {}
		_BumpScale ("Bump Scale", Float) = 1
 
		[Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
		_EmissionMap ("Emission Texture", 2D) = "white" {}
		_EmissionColor ("Emission Colour", Color) = (0, 0, 0, 0)
		
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
		_OutlineMinWidth ("Outline Min Width", Range(0,1)) = 0.5

		[Header(Interior)]

		_Ramp ("Ramp Texture", 2D) = "white" {}
		// Stroke Map
		_StrokeTex ("Stroke Noise Tex", 2D) = "white" {}
		_InteriorNoise ("Interior Noise Map", 2D) = "white" {}
		_LightingAdjust("Light Adjust", Range(0, 1)) = 0.15
		_BrightnessAdjust("Brightness Adjust", Range(-1, 1)) = 0.15
		_GlobalBrightness("Global Brightness", Range(-1, 1)) = 0.15
		_RimPower("Rim Power", Range(0, 1)) = 0.15
		// Interior Noise Level
		_InteriorNoiseLevel ("Interior Noise Level", Range(0, 1)) = 0.15
		// Guassian Blur
		radius ("Guassian Blur Radius", Range(0,60)) = 30
        resolution ("Resolution", float) = 800
        hstep("HorizontalStep", Range(0,1)) = 0.5
        vstep("VerticalStep",	Range(0,1)) = 0.5  
	}
	SubShader { 
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
            half hstep;
            half vstep;
			half _InteriorNoiseLevel;
			sampler2D _InteriorNoise;
			half _Outline;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			float _OutsideNoiseWidth;
			half _MaxOutlineZOffset;
			half _OutlineCut;
			half _LightingAdjust;
			half _OutlineMinWidth;
			half _RimPower;
			half _BrightnessAdjust;
			half _GlobalBrightness;
			CBUFFER_END
		ENDHLSL
 
		Pass {
			Name "BasePass"
			Tags { "LightMode"="UniversalForward" }
 
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
				half4 fogFactorAndVertexLight	: TEXCOORD6; // x: fogFactor, yzw: vertex light
 
				#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					float4 shadowCoord			: TEXCOORD7;
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
				// surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb * IN.color.rgb;
				surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
 
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
				SurfaceData surfaceData = InitializeSurfaceData(IN);
				InputData inputData		= InitializeInputData(IN, surfaceData.normalTS);
 
				// In URP v10+ versions we could use this :
				// half4 color = UniversalFragmentPBR(inputData, surfaceData);
 
				// But for other versions, we need to use this instead.
				// We could also avoid using the SurfaceData struct completely, but it helps to organise things.
				half4 color = UniversalFragmentPBR(inputData, half4(1,1,1,1), 0, 
					0, surfaceData.smoothness, surfaceData.occlusion, 
					surfaceData.emission, surfaceData.alpha);
 
				color.rgb = MixFog(color.rgb, inputData.fogCoord);
 
				// color.a = OutputAlpha(color.a);
				// Not sure if this is important really. It's implemented as :
				// saturate(outputAlpha + _DrawObjectPassData.a);
				// Where _DrawObjectPassData.a is 1 for opaque objects and 0 for alpha blended.
				// But it was added in URP v8, and versions before just didn't have it.
				// We could still saturate the alpha to ensure it doesn't go outside the 0-1 range though :


				///ChinesePainting
				// Perlin Noise
				// For the bias of the coordiante
				float3 worldNormal = normalize(IN.normalWS);
				float rim = 1 - saturate(dot(IN.normalWS, normalize(IN.viewDirWS)));
				rim = pow(rim, _RimPower);
				rim *= 0.5;
				rim = clamp(rim,0.3,0.8);
				// return rim;
				float4 burn = tex2D(_InteriorNoise, IN.uv * 2.5);

				half diff =  (color.r+color.b+color.g)*0.33;
				diff = (diff * 0.5 +_LightingAdjust);
				diff = saturate(diff + rim);
				diff = saturate(diff + _BrightnessAdjust);
				float2 k = tex2D(_StrokeTex, IN.uv).xy;
				float2 cuv = float2(diff, diff) + clamp( k * burn.xy * _InteriorNoiseLevel,0.0,0.3);

				// This iniminate the bias of the uv movement?
				if (cuv.x > 0.95)
				{
					cuv.x = 0.95;
					cuv.y = 1;
				}
				if (cuv.y >  0.95)
				{
					cuv.x = 0.95;
					cuv.y = 1;
				}
				cuv = clamp(cuv, 0, 1);

				//Guassian Blur
				float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                float2 tc = cuv ;
                //blur radius in pixels
                float blur = radius/resolution/4;     
                sum += tex2D(_Ramp, float2(tc.x - 4.0*blur*hstep, tc.y - 4.0*blur*vstep)) * 0.0162162162;
                sum += tex2D(_Ramp, float2(tc.x - 3.0*blur*hstep, tc.y - 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_Ramp, float2(tc.x - 2.0*blur*hstep, tc.y - 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_Ramp, float2(tc.x - 1.0*blur*hstep, tc.y - 1.0*blur*vstep)) * 0.1945945946;
                sum += tex2D(_Ramp, float2(tc.x, tc.y)) * 0.2270270270;
                sum += tex2D(_Ramp, float2(tc.x + 1.0*blur*hstep, tc.y + 1.0*blur*vstep)) * 0.1945945946;
                sum += tex2D(_Ramp, float2(tc.x + 2.0*blur*hstep, tc.y + 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_Ramp, float2(tc.x + 3.0*blur*hstep, tc.y + 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_Ramp, float2(tc.x + 4.0*blur*hstep, tc.y + 4.0*blur*vstep)) * 0.0162162162;
				sum *= half4(surfaceData.albedo,1);
				color.a = saturate(color.a);
				sum.r = saturate(sum.r - _GlobalBrightness);
				sum.g = saturate(sum.g - _GlobalBrightness);
				sum.b = saturate(sum.b - _GlobalBrightness);
				return float4(sum.rgb, 1.0);
				return float4(diff,diff,diff, 1.0);
				return color; // float4(inputData.bakedGI,1);
			}
			ENDHLSL
		}
 
		// UsePass "Universal Render Pipeline/Lit/ShadowCaster"
		// Note, you can do this, but it will break batching with the SRP Batcher currently due to the CBUFFERs not being the same.
		// So instead, we'll define the pass manually :
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }
 
			ZWrite On
			ZTest LEqual
 
			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x gles
			//#pragma target 4.5
 
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
 
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
 
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment
 
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
 
			// Note if we want to do any vertex displacment, we'll need to change the vertex function :
			/*
			//  e.g. 
			#pragma vertex vert
 
			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
 
				// Example Displacement
				input.positionOS += float4(0, _SinTime.y, 0, 0);
 
				output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
				output.positionCS = GetShadowPositionHClip(input);
				return output;
			}*/
 
			// Using the ShadowCasterPass means we also need _BaseMap, _BaseColor and _Cutoff shader properties.
			// Also including them in cbuffer, with the exception of _BaseMap as it's a texture.
 
			ENDHLSL
		}
 
		// Similarly, we should have a DepthOnly pass.
		// UsePass "Universal Render Pipeline/Lit/DepthOnly"
		// Again, since the cbuffer is different it'll break batching with the SRP Batcher.
 
		// The DepthOnly pass is very similar to the ShadowCaster but doesn't include the shadow bias offsets.
		// I believe Unity uses this pass when rendering the depth of objects in the Scene View.
		// But for the Game View / actual camera Depth Texture it renders fine without it.
		// It's possible that it could be used in Forward Renderer features though, so we should probably still include it.
		Pass {
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }
 
			ZWrite On
			ColorMask 0
 
			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x gles
			//#pragma target 4.5
 
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
 
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
 
			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment
 
			//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			// Note, the Lit shader that URP provides uses this, but it also handles the cbuffer which we already have.
			// We could change the shader to use their cbuffer, but we can also just do this :
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
 
			// Again, using the DepthOnlyPass means we also need _BaseMap, _BaseColor and _Cutoff shader properties.
			// Also including them in cbuffer, with the exception of _BaseMap as it's a texture.
 
			ENDHLSL
		}
 
		// URP also has a "Meta" pass, used when baking lightmaps.
		// UsePass "Universal Render Pipeline/Lit/Meta"
		// While this still breaks the SRP Batcher, I'm curious as to whether it matters.
		// The Meta pass is only used for lightmap baking, so surely is only used in editor?
		// Anyway, if you want to write your own meta pass look at the shaders URP provides for examples
		// https://github.com/Unity-Technologies/Graphics/tree/master/com.unity.render-pipelines.universal/Shaders
		
		
		Pass {
			NAME "OutlineBrush"
			Tags { "LightMode"="LightweightForward" }
 
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
				// scaledir += 0.5;
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
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.2 * _OutsideNoiseWidth ;
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
				// if (burn.x > _OutlineCut)
				// 	discard;
				return c;
			}
			ENDHLSL
		}

		Pass {
			NAME "Outline"
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
				// scaledir += 0.5;
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
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.8 * _OutsideNoiseWidth ;
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
				half3 burn = tex2D(_OutlineNoise, IN.uv * 20).rgb;
				if (burn.x > _OutlineCut)
					discard;
				return c;
			}
			ENDHLSL
		}

	}
}