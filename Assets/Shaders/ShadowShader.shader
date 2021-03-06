﻿Shader "Custom/ShadowShader" 
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_InnerRange("Inner Range", float) = 0.0
		_OuterIntensity("Outer Intensity", float) = 0.0
		_Multiply("Multiply", float) = 2.0
		_AdditiveColor("Additive Color", Color) = (1,1,1,1)
		_SurfaceColor("Surface Color", Color) = (1,1,1,1)
		_Transparency("Transparency", Range(-1,1)) = 0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		ZWrite Off
		Blend One One
		LOD 200

		CGPROGRAM
		#pragma surface surf BlinnPhong alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _Texture;
		float _InnerRange;
		float _OuterIntensity;
		float _Multiply;
		fixed4 _AdditiveColor;
		fixed4 _SurfaceColor;
		half _Transparency;
		
		struct Input
		{
			float2 uv_Texture;
			float3 viewDir;
			//float3 lightDir;
			//float3 worldPos;
			//float3 worldNormal;
			//float4 screenPos;
		};
		
		//void surf (Input IN, inout SurfaceOutputStandard o) 
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 texColor = tex2D(_Texture, IN.uv_Texture);
			//fixed4 scalar = saturate(1.0 - dot(-normalize(IN.viewDir), IN.Normal));
			fixed4 scalar = saturate(1.0 - dot(normalize(IN.viewDir), o.Normal));
			scalar = pow(scalar, _InnerRange) * _OuterIntensity;
			o.Albedo = _SurfaceColor;
			o.Albedo += scalar * _AdditiveColor * texColor;
			o.Albedo *= _Multiply;
			o.Alpha = -texColor.a + _Transparency;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
