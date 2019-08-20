Shader "App/SRP/BoardChara"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ExpectedRect("Expected",Vector) = (0,0,0.25,0.25)
		_RectValue("Rect",Vector) = (0,0,0,0)
		[HDR]_Color ("Color Tint", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent"}
		LOD 100
		
		// Z prepass
		Pass
		{
            Tags { "LightMode" = "ZPrepass"}
			ZWrite On
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _ExpectedRect;
			float4 _MainTex_ST;
			
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float4,_RectValue)			
			UNITY_INSTANCING_BUFFER_END(Props)

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			#include "../Cginc/BoardChara.cginc"

			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				clip( col.a - 0.5);
				return col ;
			}
			ENDCG
		}

	
		Pass
		{
            Tags { "LightMode" = "BasicPass"}
			ZWrite Off
			ZTest Equal 
	        Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"			
			#include "UnityLightingCommon.cginc" 
			
			sampler2D _MainTex;
			float4 _ExpectedRect;
			float4 _MainTex_ST;
			
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float4,_RectValue)
			UNITY_DEFINE_INSTANCED_PROP(fixed4,_Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
		
			#include "../Cginc/BoardChara.cginc"
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i); 
				fixed4 col = tex2D(_MainTex, i.uv);
				return col * UNITY_ACCESS_INSTANCED_PROP(Props,_Color) * _LightColor0  ;
			}
			ENDCG
		}
	}
}
