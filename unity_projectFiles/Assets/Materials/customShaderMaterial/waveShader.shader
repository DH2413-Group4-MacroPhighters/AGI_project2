Shader "Unlit/waveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("TestColor", Color) = (1,1,1,1)
        _WaveAmp ("Wave Amplitude", Float) = 0.1
        _WaveSpeed("Wave Speed", Float) = 0.5
        _WaveFreq( "Wave Frequency", Float) = 2.0
        _MaskAmp( "Mask AMplitude", Float) = 2.0
        _MaskSpeed( "Mask speed", Float) = 0
        _InflucenceFactor("Influence factor", Range(0,1)) = 0.1
        _NormalUpThreashold( "Normal up threshold", Range(0,1)) = 0.05
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

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Interpolator
            {
                float4 vertex : SV_POSITION;
                float3 normals: NORMAL;
                float2 uv : TEXCOORD0;
            };
            

            Interpolator vert (MeshData v)
            {
                Interpolator o;
                float2 subUV = v.uv;
                subUV.x += _MaskSpeed*_Time.y;
                
                const float mask = tex2Dlod(_MainTex, float4(subUV, 0, 0))*_MaskAmp;
                
                float wave = _WaveAmp*cos(_WaveFreq*(v.vertex.x + _Time.y*_WaveSpeed));
                wave*=  _WaveAmp*cos(_WaveFreq*(v.vertex.z + _Time.y*_WaveSpeed));
                

                v.vertex.y += (length(v.normal - float3(0,1,0)) < _NormalUpThreashold)*_InflucenceFactor*(mask + wave);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normals = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            } 


            fixed4 frag (Interpolator i) : SV_Target
            {
                
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                
                //return float4(normalize(i.normals), 1);
                return _Color;
                //return float4(0,i.vertex.xy,1);
            }
            ENDCG
        }
    }
}
