Shader "Custom/Planet"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _BumpMap("Normal", 2D) = "bump" {}
        _WaterMaskTex("Water Mask", 2D) = "gray" {}
        _Clouds("Clouds", 2D) = "black" {}
        _Night("Night", 2D) = "black" {}
        _NightColor("Night Color", Color) = (1.0, 0.9, 0.6, 1.0)

        _GroundMetallic("Ground Metallic", Range(0, 1)) = 0.0
        _GroundSmoothness("Ground Smoothness", Range(0, 1)) = 0.0
        _WaterMetallic("Water Metallic", Range(0, 1)) = 0.1
        _WaterSmoothness("Water Smoothness", Range(0, 1)) = 0.7
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM

        #pragma surface surf Planet
        #include "UnityPBSLighting.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Clouds;
            float2 uv_Night;
        };

        struct SurfaceOutputPlanet
        {
            fixed3 Albedo;
            fixed3 Normal;
            half3 Emission;
            half Metallic;
            half Smoothness;
            half Occlusion;
            fixed Alpha;
            half Clouds;
            half3 Night;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _WaterMaskTex;
        sampler2D _Clouds;
        sampler2D _Night;
        fixed3 _NightColor;
        float _GroundSmoothness;
        float _GroundMetallic;
        float _WaterSmoothness;
        float _WaterMetallic;

        inline half4 LightingPlanet(SurfaceOutputPlanet s, float3 viewDir, UnityGI gi)
        {
            SurfaceOutputStandard ls;
            ls.Albedo = s.Albedo;
            ls.Normal = s.Normal;
            ls.Emission = s.Emission;
            ls.Metallic = s.Metallic;
            ls.Smoothness = s.Smoothness;
            ls.Occlusion = s.Occlusion;
            ls.Alpha = s.Alpha;
            half4 color = LightingStandard(ls, viewDir, gi);

            fixed night = 1.0 - saturate(saturate(dot(s.Normal, gi.light.dir))*16.0);
            color.rgb += _NightColor*s.Night*night*(1 - s.Clouds);
            return color;
        }

        inline void LightingPlanet_GI(SurfaceOutputPlanet s, UnityGIInput data, inout UnityGI gi)
        {
            SurfaceOutputStandard ls;
            ls.Albedo = s.Albedo;
            ls.Normal = s.Normal;
            ls.Emission = s.Emission;
            ls.Metallic = s.Metallic;
            ls.Smoothness = s.Smoothness;
            ls.Occlusion = s.Occlusion;
            ls.Alpha = s.Alpha;

            LightingStandard_GI(ls, data, gi);
        }

        void surf(Input IN, inout SurfaceOutputPlanet o)
        {
            fixed4 ground = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 clouds = tex2D(_Clouds, IN.uv_Clouds);
            fixed4 waterMask = tex2D(_WaterMaskTex, IN.uv_MainTex);
            
            o.Albedo = lerp(ground.rgb, clouds.rgb, clouds.a);
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            o.Metallic = lerp(_GroundMetallic, _WaterMetallic, waterMask.a);
            o.Smoothness = lerp(_GroundSmoothness, _WaterSmoothness, waterMask.a);
            o.Clouds = clouds.a;
            o.Night = tex2D(_Night, IN.uv_Night.xy).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
