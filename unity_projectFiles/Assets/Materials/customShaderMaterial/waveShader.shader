Shader "Unlit/waveShader"
{
    Properties
    {
        // THis is uniforms. They will be the same for every vertex and fragments
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("TestColor", Color) = (1,1,1,1)
        _WaveAmp ("Wave Amplitude", Float) = 0.1
        _WaveSpeed("Wave Speed", Float) = 0.5
        _WaveFreq( "Wave Frequency", Float) = 2.0
        _MaskAmp( "Mask AMplitude", Float) = 2.0
        _MaskSpeed( "Mask speed", Float) = 0
        _InflucenceFactor("Influence factor", Range(0,1)) = 0.1
        _NormalUpThreashold( "Normal up threshold", Range(0,1)) = 0.05
        _Glossy("Glossy", range(0, 1)) = 0.5
        _DiffusePart("Diffuse Strength", range(0, 1)) = 0.5
        _SpecPart("_SpecPart Strength ", range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            float _WaveAmp;
            float _WaveSpeed;
            float _WaveFreq;
            
            float _MaskAmp;
            float _MaskSpeed;
            float _InflucenceFactor;
            float _NormalUpThreashold;

            float _Glossy;
            float _DiffusePart;
            float _SpecPart;

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
                
                float2 subUV = v.uv;
                subUV.x += _MaskSpeed*_Time.y;
                
                const float mask = tex2Dlod(_MainTex, float4(subUV, 0, 0))*_MaskAmp;
                const float ofsetNormal = (length(v.normal - float3(0,1,0)) < _NormalUpThreashold);
                float wave = _WaveAmp*cos(_WaveFreq*(v.vertex.x + _Time.y*_WaveSpeed));
                wave*=  _WaveAmp*cos(_WaveFreq*(v.vertex.z + _Time.y*_WaveSpeed));
                
                v.vertex.y += ofsetNormal*_InflucenceFactor*(mask + wave);


                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normals = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            } 


            fixed4 frag (Interpolator i) : SV_Target
            {
                
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                float3 c = float3(0,0,0);

                float3 ddxPos = ddx(i.worldPos);
                float3 ddyPos = ddy(i.worldPos)  * _ProjectionParams.x;
                float3 normal = normalize( cross(ddxPos, ddyPos));


                
                float3 N = normal;//normalize(i.normals);
                float3 L = _WorldSpaceLightPos0.xyz;


                // diffuse
                const float diffuse_light = saturate(dot(N, L))*_DiffusePart;
                c += _Color*diffuse_light*_LightColor0.xyz;

                //specular

                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 R = reflect(-L, N);
                float spec_light = saturate(dot(R,V))*(diffuse_light>0)*_SpecPart;

                _Glossy = exp2(_Glossy*8 + 1);
                spec_light = pow(spec_light, _Glossy);

                c+=spec_light.xxx;
                return float4(c, 1);
                //return _Color;
                //return float4(0,i.vertex.xy,1);
                //return float4(i.uv,0,1);
            }
            ENDCG
        }
    }
}
