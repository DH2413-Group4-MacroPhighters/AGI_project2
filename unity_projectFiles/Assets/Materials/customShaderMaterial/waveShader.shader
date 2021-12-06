Shader "Unlit/waveShader"
{
    Properties
    {
        // THis is valus that is set by the user, The material settings.
        _Color ("TestColor", Color) = (1,1,1,1)
        
        _WaveAmp ("Wave Amplitude", Float) = 0.1
        _WaveSpeed("Wave Speed", Float) = 0.5
        _WaveFreq( "Wave Frequency", Float) = 2.0

        _NormalUpThreashold( "Normal up threshold", Range(0,1)) = 0.05
        _Glossy("Glossy", range(0, 1)) = 0.5
        _DiffusePart("Diffuse Strength", range(0, 1)) = 0.5
        _SpecPart("_SpecPart Strength ", range(0, 1)) = 0.5
        
        _DepthExpV("LightAbsorbtion factor" , range(0,0.1)) = 0.01
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {   
            Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
            CGPROGRAM
             
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"


            //uniforms. They will be the same for every vertex and fragments
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            float _WaveAmp;
            float _WaveSpeed;
            float _WaveFreq;
            
            float _NormalUpThreashold;

            float _Glossy;
            float _DiffusePart;
            float _SpecPart;

            float _DepthStrength;
            float _DepthExpV;

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Interpolator
            {
                float4 vertex : SV_POSITION;
                float3 normals: TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 worldPos: TEXCOORD2;
            };
            

            Interpolator vert (MeshData v)
            {
                Interpolator o;
                
                o.normals = UnityObjectToWorldNormal(v.normal);
                const float offsetNormal = length(o.normals - float3(0,1,0)) < _NormalUpThreashold;

                // We go to WorldCoordinates
                v.vertex =  mul(unity_ObjectToWorld, v.vertex);

                // Calculated Wave factor
                float wave = cos(_WaveFreq*(v.vertex.z + _Time.y*_WaveSpeed));
                wave *= cos(_WaveFreq*(v.vertex.x + _Time.y*_WaveSpeed));

                // Add the offset with the normals
                v.vertex.xyz += o.normals*offsetNormal*(wave*_WaveAmp);
               
                o.worldPos = v.vertex;
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, v.vertex));
                o.uv = v.uv;
                
                return o;
            } 


            fixed4 frag (Interpolator i) : SV_Target
            {
                
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                float3 c = float3(0,0,0);

                float3 ddxPos = ddx(i.worldPos);
                float3 ddyPos = ddy(i.worldPos)  * _ProjectionParams.x;
                float3 normal = normalize(cross(ddxPos, ddyPos));
                
                float3 N = normal;//normalize(i.normals);
                float3 L = _WorldSpaceLightPos0.xyz;
                
                // diffuse
                const float diffuse_light = saturate(dot(N, L))*_DiffusePart;
                c += _Color*diffuse_light*_LightColor0.xyz;

                //specular
                float3 ViewVector = _WorldSpaceCameraPos - i.worldPos;
                
                float3 V = normalize(ViewVector);
                float3 R = reflect(-L, N);
                float3 H = normalize(L+V);
                float spec_light = saturate(dot(H,V))*(diffuse_light>0)*_SpecPart;

                _Glossy = exp2(_Glossy*8 + 1);
                spec_light = pow(spec_light, _Glossy);

                float alpha = 1 - exp(-_DepthExpV*length(ViewVector));

                c+=spec_light*_LightColor0.xyz; 
                return float4(c, alpha);
                //return _Color;
                //return float4(0,i.vertex.xy,1);
                //return float4(i.uv,0,1);
            }
            ENDCG
        }
    }
}
