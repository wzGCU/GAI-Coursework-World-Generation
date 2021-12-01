Shader "VertexColorLit" {
Properties{
	_MainTex("Texture", 2D) = "white" {}
	_ColorTint("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
}
SubShader{
	Tags{ "RenderType" = "Opaque" }
	CGPROGRAM
	#pragma surface surf Lambert
	struct Input {
		float2 uv_MainTex;
		fixed4 color : COLOR;
	};
	fixed4 _ColorTint;
	sampler2D _MainTex;
	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color * _ColorTint;
	}
	ENDCG
	}
	Fallback "Diffuse"
}