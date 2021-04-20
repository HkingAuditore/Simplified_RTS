Shader "Unlit/MixPicture"{
	Properties {
		_BaseMap ("Base Texture", 2D) = "white" {}
		_BaseColor ("Base Colour", Color) = (0, 0.66, 0.73, 1)
		_MixMap ("Mix Texture", 2D) = "white" {}
		_MixColor ("Mix Colour", Color) = (0, 0.66, 0.73, 1)

	}
	SubShader {
		Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
 
		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 
			CBUFFER_START(UnityPerMaterial)
			half4 _BaseMap_ST;
			half4 _BaseColor;
			half4 _MixColor;
			//float4 _ExampleDir;
			//float _ExampleFloat;
			CBUFFER_END
		ENDHLSL
 
		Pass {
			Name "Example"
			Tags { "LightMode"="UniversalForward" }
 
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			struct Attributes {
				float4 positionOS	: POSITION;
				float2 uv		: TEXCOORD0;
				float4 color		: COLOR;
			};
 
			struct Varyings {
				float4 positionCS 	: SV_POSITION;
				float2 uv		: TEXCOORD0;
				float4 color		: COLOR;
			};
 
			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);
			TEXTURE2D(_MixMap);
			SAMPLER(sampler_MixMap);
 
			Varyings vert(Attributes IN) {
				Varyings OUT;
 
				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				OUT.positionCS = positionInputs.positionCS;
				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
				OUT.color = IN.color;
				return OUT;
			}
 
			half4 frag(Varyings IN) : SV_Target {
				half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
				half4 mixMap = SAMPLE_TEXTURE2D(_MixMap, sampler_MixMap, IN.uv)* _MixColor;
				half4 color = baseMap*(1-mixMap.a)+mixMap*(mixMap.a);
				return color;
			}
			ENDHLSL
		}
	}
}