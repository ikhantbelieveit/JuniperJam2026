Shader "Custom/Waves"
{
    Properties
    {
        _BaseColor ("Color", Color) = (1,1,1,1)
        _BaseMap ("Albedo", 2D) = "white" {}

        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Amplitude ("Amplitude", float) = 1
        _Frequency ("Frequency", float) = 2
        _Speed ("Speed", float) = 1
        _Wavelength("Wavelength", float) = 10

    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardLit"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            float _Frequency;
            float _Speed;
            float _Amplitude;
            float _Wavelength;

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float _Smoothness;
                float _Metallic;
            CBUFFER_END

            float GetWaveHeight(float x, float z)
            {

                float pi = 3.14;
                float k = 2 * pi / _Wavelength;
                return _Amplitude * sin(k * (x - _Speed * _Time.y));
                //return sin(x * _Frequency - _Time.y * _Speed) * _Amplitude;
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionOS = IN.positionOS.xyz;

                // Future wave displacement goes here:
                // positionOS.y += sin(positionOS.x + _Time.y);

                positionOS.y += GetWaveHeight(positionOS.x, positionOS.z);

                OUT.positionHCS = TransformObjectToHClip(positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 albedo =
                    SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv)
                    * _BaseColor;

                return albedo;
            }

            ENDHLSL
        }
    }
}