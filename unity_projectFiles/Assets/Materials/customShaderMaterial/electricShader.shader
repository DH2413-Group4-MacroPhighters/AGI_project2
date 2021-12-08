Shader "Unlit/electricShader"
{
    Properties
    {
        _CableColor ("Cable Color", Color) = (0.5,0.5,0.5,1)
        _ElecStart ("Electricity Start", Color) = (0.5,0.5,0.5,1)
        _ElecPeak("Electricity Peak", Color) = (0.5,0.5,0.5,1)
        
        _Freq("Freq", range(0.001, 100) ) = 5
        _Speed("Flow Speed" , range(0.001, 2)) = 0.1
        _SpikeF("Spike Factor", range(0, 20)) = 10
        _Step("Step Factor" , range(0.001, 0.5)) = 0.1
        
        _Exp("exponent" , range(0.15, 15)) = 1
        _F("Constant Speed Factor" , range(1, 15)) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {   
            CGPROGRAM
            // This shader is specific for a unity cylinder. Don't use on anything else. 
             
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"


            //uniforms. They will be the same for every vertex and fragments
            sampler2D _MainTex;
            float _Freq;
            float _Speed;
            float _SpikeF;
            float _Step;
            float4 _CableColor;
            float4 _ElecStart;
            float4 _ElecPeak;

            float _Exp;
            float _F;
            

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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normals = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            } 


            fixed4 frag (Interpolator i) : SV_Target
            {
                
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                
                if(length(abs(i.normals) - float3(0,1,0))< 0.001){

                    return _CableColor;
                }
                
                 //float timeValue = floor(_Time.y) + pow(_Time.y-floor(_Time.y),_Exp);

                
                const float timeValue = (_Time.y - (cos(_Time.y*5)+1)/2 + 0.35)*_Speed;
                
                //const float timeValue = exp(a*frac(_Time.y))/exp(a) + floor(_Time.y) + 0.2;
                   
                float wave = (sin((i.uv.x+timeValue)*_Freq) + 1)/2;

                

                
                wave = pow(wave, _SpikeF);

                
                wave = round(wave/_Step)*_Step; // To make steps in the gradient.
                
                //return float4(i.uv.x,0,0,1);
                
                if(wave>0){
                    wave = (wave-_Step)/(1-_Step);
                    
                    return lerp(_ElecStart, _ElecPeak, wave);
                }
                else{
                    return _CableColor;
                }

                //float4 finalColor = lerp(_CableColor, yellow, wave);
                
                //return _Color;
                //return float4(0,i.vertex.xy,1);
                
            }
            ENDCG
        }
    }
}
