Shader "Planet"
{
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB)", 2D) = "white" {}
        _SpecularTex ("Specular (RGB)", 2D) = "gray" {}
        _BumpMap ("Normal (Normal)", 2D) = "bump" {}
		_Clouds ("Clouds", 2D) = "gray" {}
		_Night ("Night", 2D) = "gray" {}
        _Gloss ("Gloss Value", Range(0,1)) = 0.5
        _CloudsGloss ("Clouds Gloss Value", Range(0,1)) = 0.1
    }

    SubShader{
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM

		struct SurfaceOutputPlanet {
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Specular;
			half Gloss;
			half Alpha;
			half3 Night;
		};

		// ќбычный BlinnPhong из Lighting.cginc с добавлением блеска и текстурой на ночной стороне
		inline fixed4 LightingPlanet (SurfaceOutputPlanet s, fixed3 lightDir, half3 viewDir, fixed atten)
		{
			half3 h = normalize (lightDir + viewDir);
			fixed diff = saturate (dot (s.Normal, lightDir));
			float nh = saturate (dot (s.Normal, h));
			float spec = pow (nh, s.Gloss*128.0) * s.Specular;
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten * 2;
			// ƒобавл€ем текстуру на ночную сторону с помощью (1 - saturate(16 * diff))
			c.rg += s.Night * _LightColor0.rgb * (1 - saturate(16 * diff));
			c.b += s.Night * 0.7 * _LightColor0.rgb * (1 - saturate(16 * diff));
			c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
			// TODO: —делать обработку точечного света
			return c;
		}

        #pragma surface surf Planet

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_Clouds;
			float2 uv_Night;
        };

        sampler2D _MainTex, _SpecularTex, _BumpMap, _Clouds, _Night;
        float _Gloss, _CloudsGloss;

        void surf (Input IN, inout SurfaceOutputPlanet o)
        {
			// —мешиваем текстуру земли с текстурой облаков
			o.Albedo = lerp (tex2D(_MainTex, IN.uv_MainTex).rgb, tex2D(_Clouds, IN.uv_Clouds).rgb, tex2D(_Clouds, IN.uv_Clouds).a);
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			// —мешиваем карту отражений с картой облааков так, чтобы облака закрывали воду и глушили отражени€
            o.Specular = lerp (tex2D(_SpecularTex, IN.uv_MainTex).rgb, _CloudsGloss, tex2D(_Clouds, IN.uv_Clouds).a);
            o.Gloss = _Gloss;
			o.Night = tex2D(_Night,IN.uv_Night.xy).rgb;
        }
        ENDCG
    }
	FallBack "Diffuse"
}