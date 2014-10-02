Shader "Custom/Glow" {
	Properties {
		_Distance ("Distance", Range(0,1.0)) = 0.5
		_Brightness ("Brightness", Range(0.0,2.0)) = 1.0
		_MainColor ("MainColor", Color) = (0.26,0.19,0.16,0.0)
		_GlowColor ("GlowColor", Color) = (0.26,0.19,0.16,0.0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("BumpMap", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "AlphaTest"}

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _MainColor;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_BumpMap));
			o.Albedo = c.rgb * _MainColor.rgb;
			o.Alpha = c.a;
		}
		ENDCG
		
		Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
		ZWrite Off
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha			
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		float _Distance;
		float _Brightness;
		float4 _GlowColor;
		
		struct Input {
			float3 viewDir;
			float3 viewDir2;
			float3 vNormal;
		};
		
		void vert(inout appdata_full v, out Input o){
			float4 vertPos = v.vertex + float4(v.normal * _Distance, 0);
			o.viewDir2 = ObjSpaceViewDir(vertPos);
			o.vNormal = v.normal;
			v.vertex = vertPos;
		}
		

		void surf (Input IN, inout SurfaceOutput o) {
			half VdotN = saturate(_Brightness - dot(normalize(IN.viewDir2), IN.vNormal));
			half4 gColor = _GlowColor * VdotN;
			o.Albedo = gColor.rgb;
			o.Alpha = gColor.a;
			o.Emission = gColor.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}