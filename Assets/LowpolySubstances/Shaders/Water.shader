Shader "LowpolySubstances/Water" {

Properties {
	_PrimaryColor("Primary color", Color) = (1,1,1,1)
	_SecondaryColor("Secondary color", Color) = (1,1,1,1)
    
    [Space]
    _SecondaryColorAngle("Secondary color angle", Range(0, 5)) = 1.0
    _SecondaryColorImpact("Secondary color impact", Range(0, 2)) = 1.0
    
    [Space]
    _Alpha("Opacity", Range(0, 1)) = 0.8
    
    [Space]
    _Intensity("Intensity", Range(0, 15)) = 1
    _Speed("Speed", Range(0, 15)) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }

	AlphaTest Off
	Blend One OneMinusSrcAlpha
	Cull Back
	Fog { Mode Off }
	Lighting Off
	ZTest LEqual
	ZWrite On
	
	SubShader {
		Pass {
			CGPROGRAM
			
            #pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_fog
            
			#include "UnityCG.cginc"

			struct appdata {
			    half4 position : POSITION;
			    half2 texcoord : TEXCOORD0;
			    half3 normal : NORMAL;
			};

			struct v2f {
			    half4 position : SV_POSITION;
			    half4 color : TEXCOORD0;
                UNITY_FOG_COORDS(1)
			};

			half4 _PrimaryColor;
			half4 _SecondaryColor;
            
            half _SecondaryColorImpact;
            half _SecondaryColorAngle;
            
            half _Alpha;

            half _Intensity;
            half _Speed;
            
			v2f vert(appdata vertex) {
			    v2f output;

                vertex.position.y += sin(_Time.y * _Speed + vertex.position.x) * _Intensity * 0.05;
                
			    half3 worldPosition = mul(_Object2World, vertex.position).xyz;
			    half3 cameraVector = normalize(worldPosition.xyz - _WorldSpaceCameraPos);
			    half3 worldNormal = normalize(mul(_Object2World, half4(vertex.normal,0)).xyz);
			    half blend = dot(worldNormal, cameraVector) * _SecondaryColorAngle + _SecondaryColorImpact;

			    output.color = _PrimaryColor * (1 - blend) + _SecondaryColor * blend;
                output.color.a = _Alpha;
			    output.color.rgb *= output.color.a;
                
                output.position = mul(UNITY_MATRIX_MVP, vertex.position);
                
                UNITY_TRANSFER_FOG(output, output.position);

			    return output;
			}
			
			half4 frag(v2f fragment) : COLOR {
                UNITY_APPLY_FOG(fragment.fogCoord, fragment.color);
				return fragment.color;
			}
            
			ENDCG
		}
	}
	
}
}
