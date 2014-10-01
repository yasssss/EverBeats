Shader "Effects/Glass/Cutout/CutoutDiffuse" {
   Properties {
	  _Color ("Main Color", Color) = (1,1,1,1)
	  _ColorGlass ("Color Glass", Color) = (1,1,1,1)
	  _MainTex ("Main Tex",2D) = "white" {}
	  _CutoutTex ("Cutout Tex (A)",2D) = "white" {}
	  _ColorIntensity ("Color Intensity", Range (0, 3)) = 1
	  _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Range (2, 20)) = 15
      _Cube("Reflection Map", Cube) = "" {}
	  _NormalMap ("Normal Map", 2D) = "bump" {}
	  _RefractiveStrength ("Refractive Strength", Range (1.5, 2)) = 1
	  _RimPower ("RimPower", Range (0, 1)) = 0.7
	  _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
	  _Cutout ("Cutout", Range (0, 1)) = 0.5
   }

    SubShader {
        //Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            //Blend One One
            //Fog { Color(0,0,0,0) }

            CGPROGRAM
            
			#pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

			float4 _LightColor0;
			float4 _Color;
			float4 _ColorGlass;
			float4 _SpecColor; 
            float _Shininess;
			sampler2D _MainTex;
			sampler2D _CutoutTex;
            samplerCUBE _Cube;  
			sampler2D _NormalMap; 
			float4 _NormalMap_ST;
			float _RefractiveStrength;
			float _ColorIntensity;
			float _RimPower;
			float4 _RimColor;
			float _Cutout;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
				float4 posWorld: TEXCOORD2;
				float4 tex : TEXCOORD3;
				float3 tangentWorld : TEXCOORD4;  
				float3 normalWorld : TEXCOORD5;
				float3 binormalWorld : TEXCOORD6;
				float3 color : COLOR;
            };

            v2f vert (vertexInput v) {
                v2f o;
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				o.posWorld = mul(modelMatrix, v.vertex);
				o.viewDir = o.posWorld - _WorldSpaceCameraPos;
				o.normalDir = normalize((mul(float4(v.normal, 0.0), modelMatrixInverse).xyz));
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.tangentWorld = normalize((mul(modelMatrix, float4((v.tangent).xyz, 0.0))).xyz);
				o.normalWorld = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse));
				o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * v.tangent.w); // tangent.w is specific to Unity
				o.tex = v.texcoord;

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float dotProduct = 1 - dot(v.normal, viewDir);
				o.color = smoothstep(1 - _RimPower, 1.0, dotProduct);
				o.color *= _RimColor;

               
                return o;
            }

            float4 frag (v2f i) : COLOR {
				float4 encodedNormal = tex2D(_NormalMap, _NormalMap_ST.xy * i.tex.xy + _NormalMap_ST.zw);
				float3 localCoords = float3(2.0 * encodedNormal.ag - 1, 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
				float3x3 local2WorldTranspose = float3x3(
				   i.tangentWorld, 
				   i.binormalWorld, 
				   i.normalWorld);
				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));
				//float3 normalDirection = normalize(i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - (i.posWorld).xyz);
				float3 lightDirection;
				float attenuation;
				if (_WorldSpaceLightPos0.w == 0.0) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize((_WorldSpaceLightPos0).xyz);
				} 
				else // point or spot light
				{
				   float3 vertexToLightSource = (_WorldSpaceLightPos0 - i.posWorld).xyz;
				   float distance = length(vertexToLightSource);
				   attenuation = 1.0 / distance; // linear attenuation 
				   lightDirection = normalize(vertexToLightSource);
				}

				float3 ambientLighting = (UNITY_LIGHTMODEL_AMBIENT).xyz * (_Color).xyz;
				float3 diffuseReflection = attenuation * (_LightColor0).xyz * (_Color).xyz * max(0.0, dot(normalDirection, lightDirection));
				
				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) // light source on the wrong side?
				{
				   specularReflection = float3(0.0, 0.0, 0.0); // no specular reflection
				}
				else // light source on the right side
				{
				   specularReflection = attenuation * (_LightColor0).xyz
					  * (_SpecColor).xyz * pow(max(0.0, dot(
					  reflect(-lightDirection, normalDirection), 
					  viewDirection)), _Shininess);
				}
				float4 spec = float4((ambientLighting + diffuseReflection + specularReflection)*2, 1.0);

			    float3 refractedDir = refract(normalize(i.viewDir), normalize(i.normalDir), 1.0 / _RefractiveStrength);
				float3 albedo = tex2D(_MainTex, i.tex.xy ).rgb;
				float cutout = tex2D(_CutoutTex, i.tex.xy ).a;
				
				float4 color1 = _Color;
				float4 color2 = texCUBE(_Cube, refractedDir) * _ColorGlass;
				
				color2.rgb += i.color;
				color1 += spec*color1;
				color2 += spec*color2;
				color1.rgb*=albedo;
				color1 *= _ColorIntensity * _LightColor0;
				color2 *= _ColorIntensity * _LightColor0;
				float4 col;
				if(cutout>_Cutout) col = color1;
				else col = color2;
				//col = color1 * cutout + color2*(1-cutout);
			    return col;
            }

            ENDCG

        } //Pass
		 Pass {   
	   Tags { "LightMode" = "ForwardAdd" } 
	  Blend one one
         CGPROGRAM
            
			#pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

			float4x4 _LightMatrix0; // transformation 
			sampler2D _LightTextureB0; 

			float4 _LightColor0;
			float4 _Color;
			float4 _ColorGlass;
			float4 _SpecColor; 
            float _Shininess;
			sampler2D _MainTex;
			sampler2D _CutoutTex;
            samplerCUBE _Cube;  
			sampler2D _NormalMap; 
			float4 _NormalMap_ST;
			float _RefractiveStrength;
			float _ColorIntensity;
			float _RimPower;
			float4 _RimColor;
			float _Cutout;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
				float4 posWorld: TEXCOORD2;
				//float4 posLight : TEXCOORD3;
				float4 tex : TEXCOORD3;
				float3 tangentWorld : TEXCOORD4;  
				float3 normalWorld : TEXCOORD5;
				float3 binormalWorld : TEXCOORD6;
				float3 color : COLOR;
            };

            v2f vert (vertexInput v) {
                v2f o;
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				o.posWorld = mul(modelMatrix, v.vertex);
				o.viewDir = o.posWorld - _WorldSpaceCameraPos;
				o.normalDir = normalize((mul(float4(v.normal, 0.0), modelMatrixInverse).xyz));
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.posLight = mul(_LightMatrix0, o.posWorld);

				o.tangentWorld = normalize((mul(modelMatrix, float4((v.tangent).xyz, 0.0))).xyz);
				o.normalWorld = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse));
				o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * v.tangent.w); // tangent.w is specific to Unity
				o.tex = v.texcoord;

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float dotProduct = 1 - dot(v.normal, viewDir);
				o.color = smoothstep(1 - _RimPower, 1.0, dotProduct);
				o.color *= _RimColor;

               
                return o;
            }

            float4 frag (v2f i) : COLOR {
				float4 encodedNormal = tex2D(_NormalMap, _NormalMap_ST.xy * i.tex.xy + _NormalMap_ST.zw);
				float3 localCoords = float3(2.0 * encodedNormal.ag - 1, 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
				float3x3 local2WorldTranspose = float3x3(
				   i.tangentWorld, 
				   i.binormalWorld, 
				   i.normalWorld);
				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

				float3 viewDirection = normalize(_WorldSpaceCameraPos - (i.posWorld).xyz);
				float3 lightDirection;
				float attenuation;
				if (_WorldSpaceLightPos0.w == 0.0) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize((_WorldSpaceLightPos0).xyz);
				} 
				else // point or spot light
				{
				    float3 vertexToLightSource =  (_WorldSpaceLightPos0 - i.posWorld).xyz;
					lightDirection = normalize(vertexToLightSource);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
				}
				float3 diffuseReflection = attenuation * (_LightColor0).xyz * (_Color).xyz 
				* max(0.0, dot(normalDirection, lightDirection));
				float3 specularReflection;
				
				if (dot(normalDirection, lightDirection) < 0.0) // light source on the wrong side?
				{
				   specularReflection = float3(0.0, 0.0, 0.0);  // no specular reflection
				}
				else // light source on the right side
				{
				   specularReflection = attenuation * (_LightColor0).xyz
					  * (_SpecColor).xyz * pow(max(0.0, dot(
					  reflect(-lightDirection, normalDirection), 
					  viewDirection)), _Shininess);
				}
 
				float4 spec = float4((diffuseReflection + specularReflection)*2, 1.0);
				float3 refractedDir = refract(normalize(i.viewDir), normalize(i.normalDir), 1.0 / _RefractiveStrength);
				float3 albedo = tex2D(_MainTex, i.tex.xy ).rgb;
				float cutout = tex2D(_CutoutTex, i.tex.xy ).a - _Cutout;
				if(cutout>1) cutout = 1;
				if(cutout<0) cutout = 0;
				float4 color1 = _Color;
				float4 color2 = texCUBE(_Cube, refractedDir) * _ColorGlass;
				
				color2.rgb += i.color;
				color1 += spec*color1;
				color2 += spec*color2;
				color1.rgb*=albedo;
				color1 *= _ColorIntensity * _LightColor0;
				color2 *= _ColorIntensity * _LightColor0;
				float4 col;
				if(cutout>_Cutout) col = color1;
				else col = color2;
				//col = color1 * cutout + color2*(1-cutout);
				return col*(attenuation-0.2)*3;
            }

            ENDCG
      }



    } //SubShader
    
	FallBack "Diffuse" //note: for passes: ForwardBase, ShadowCaster, ShadowCollector
}