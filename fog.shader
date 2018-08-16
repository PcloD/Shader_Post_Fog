Shader "dreambuffer/Hidden/fog"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_FogDensity ("_FogDensity", float) = 1.0
		_FogColor ("FogColor", Color) = (0.5, 0, 0, 0.5)
		_FogStart("Fog Start", float) = 0  
		_FogEnd("Fog End", float) = 300  
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "../../CgIncludes/func.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			float4x4 _FrustumCornersRay;


			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv_depth = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
					if(_MainTex_TexelSize < 0)
						o.uv_depth.y = 1-o.uv_depth.y;
				#endif

				int index = 0;
				if(v.uv.x < 0.5 && v.uv.y < 0.5){
					index = 0;
				}else if(v.uv.x > 0.5 && v.uv.y < 0.5){
					index = 1;
				}else if(v.uv.x > 0.5 && v.uv.y > 0.5){
					index = 2;
				}else{
					index = 3;
				}

				#if UNITY_UV_STARTS_AT_TOP
				if(_MainTex_TexelSize.y < 0)
					index = 3 - index;
				#endif

				o.interpolatedRay = _FrustumCornersRay[index];

				return o;
			}
			
			sampler2D _MainTex;
			half4 _MainTex_TexelSize;

			fixed4 _FogColor;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 finalColor = tex2D(_MainTex, i.uv);				

				finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogDensity(i.uv_depth, i.interpolatedRay.xyz));

				return finalColor;
			}
			ENDCG
		}
	}
}
