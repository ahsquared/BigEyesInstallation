Shader "Hidden/VacuumShaders/Per-Vertex Ambient Occlusion" 
{
    SubShader 
	{
		Cull Off 
		Fog {Mode Off}
		
        Pass 
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			          
            float4 vert(float4 v:POSITION) : SV_POSITION
		    {
                return mul (UNITY_MATRIX_MVP, v);
            }

            fixed4 frag() : SV_TARGET 
			{
                return fixed4(0, 0, 0, 0);
            }

            ENDCG
        }
    }
}
