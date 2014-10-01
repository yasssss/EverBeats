Shader "Effects/Glass/Self-Illumin/Simple" {
   Properties {
	  _Color ("Main Color", Color) = (1,1,1,1)
	  _ColorIntensity ("Color Intensity", Range (0, 3)) = 1
	  _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Range (2, 20)) = 15
      _Cube("Reflection Map", Cube) = "" {}
	  _RefractiveStrength ("Refractive Strength", Range (1.5, 2)) = 1
	  _RimPower ("RimPower", Range (0, 1)) = 0.7
	  _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
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
			float4 _SpecColor; 
            float _Shininess;
            samplerCUBE _Cube;   
			float _RefractiveStrength;
			float _ColorIntensity;
			float _RimPower;
			float4  _RimColor;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
				float4 posWorld: TEXCOORD2;
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

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float dotProduct = 1 - dot(v.normal, viewDir);
				o.color = smoothstep(1 - _RimPower, 1.0, dotProduct);
				o.color *= _RimColor;

               
                return o;
            }

            float4 frag (v2f i) : COLOR {
				float3 normalDirection = normalize(i.normalDir);
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
				float4 c = texCUBE(_Cube, refractedDir) * _Color;
				c.rgb += i.color;
				c += spec*c;

			    return c * _ColorIntensity;
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
			float4 _SpecColor; 
            float _Shininess;
            samplerCUBE _Cube;   
			float _RefractiveStrength;
			float _ColorIntensity;
			float _RimPower;
			float4  _RimColor;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

            struct v2f {
                float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
				float4 posWorld: TEXCOORD2;
				float4 posLight : TEXCOORD3;
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
				o.posLight = mul(_LightMatrix0, o.posWorld);

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float dotProduct = 1 - dot(v.normal, viewDir);
				o.color = smoothstep(1 - _RimPower, 1.0, dotProduct);
				o.color *= _RimColor;

               
                return o;
            }

            float4 frag (v2f i) : COLOR {
				float3 normalDirection = normalize(i.normalDir);
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
				float4 c = texCUBE(_Cube, refractedDir) * _Color;
			
				c.rgb += i.color;
				c += spec*c;
				return c*(attenuation-0.2)*3;
            }

            ENDCG
      }



    } //SubShader
    
	FallBack "Diffuse" //note: for passes: ForwardBase, ShadowCaster, ShadowCollector
}