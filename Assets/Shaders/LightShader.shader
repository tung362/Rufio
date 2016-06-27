Shader "Custom/LightShader" 
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
		_Normal1("NormalX", float) = 2.0
		_Normal2("NormalY", float) = 2.0
		_Normal3("NormalZ", float) = 2.0
		_View1("ViewX", float) = 2.0
		_View2("ViewY", float) = 2.0
		_View3("ViewZ", float) = 2.0
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
		float _Normal1;
		float _Normal2;
		float _Normal3;
		float _View1;
		float _View2;
		float _View3;
		
		struct Input
		{
			float2 uv_Texture;
			float3 viewDir;
			float3 lightDir;
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};
		
		//void surf (Input IN, inout SurfaceOutputStandard o) 
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 texColor = tex2D(_Texture, IN.uv_Texture);
			//fixed4 scalar = saturate(1.0 - dot(-normalize(IN.viewDir), IN.Normal));
			fixed4 scalar = saturate(0.99999 - dot(normalize(float3(IN.viewDir.x + _View1, IN.viewDir.y + _View2, IN.viewDir.z + _View3)), float3(o.Normal.x + _Normal1, o.Normal.y + _Normal2, o.Normal.z + _Normal3)));
			scalar = pow(scalar, _InnerRange) * _OuterIntensity;
			o.Albedo = texColor;
			//o.Albedo += scalar * _AdditiveColor * texColor;
			o.Albedo *= _Multiply;
			o.Alpha = -texColor.a + _Transparency;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
