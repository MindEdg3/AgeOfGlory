Shader "Custom/Limit Palette" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorsDiv ("Colors division", float) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert finalcolor:limitpalette

		sampler2D _MainTex;
		float _ColorsDiv;

		struct Input {
			float2 uv_MainTex;
		};
        
        half my_func (uniform half origin, uniform half div) {
        	half remainder = fmod(origin, div);

//        	return origin - remainder;	// Simple color adjusting to lower value
//        	return origin - remainder + div;	// Simple color adjusting to higher value
        	return remainder < div*0.5 ? origin - remainder : origin - remainder + div; 	// More complicated color adjusting to neares value
        }
		
		void limitpalette (Input IN, SurfaceOutput o, inout fixed4 color)
        {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			
            color.r = my_func(color.r, _ColorsDiv);
            color.b = my_func(color.b, _ColorsDiv);
            color.g = my_func(color.g, _ColorsDiv);
        }
        
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
