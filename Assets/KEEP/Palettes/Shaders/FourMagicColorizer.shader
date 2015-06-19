Shader "PixelPirate/sprites/FourCol" {
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color1 ("Replace Black", Color) = (0, 0, 0, 0)
		_Color2 ("Replace Light-Grey", Color) = (0, 0, 0, 0)
		_Color3 ("Replace Dark-Grey", Color) = (0, 0, 0, 0)
		_Color4 ("Replace White", Color) = (0, 0, 0, 0)
	}

	SubShader {

		Tags 
		{ 
			"RenderType" = "Opaque" 
			"Queue" = "Transparent+1" 
		}

		Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha 

		Pass {
			// Name "ColorReplacement"

			// CGPROGRAM
			// #pragma vertex vert
			// #pragma fragment frag
			// #pragma fragmentoption ARB_precision_hint_fastest
			// #include "UnityCG.cginc"

			//ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha 
  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			v2f vert (appdata_tan v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			sampler2D _MainTex;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color4;

			float4 frag(v2f i) : COLOR
			{
				// Note: Mip-mapping should be disabled
				// or r may shift into the wrong
				// range for some pixels
				float4 diffuse = tex2D(_MainTex, i.uv);
				float r = diffuse.r;
				float alpha = diffuse.a;

				// Note: The GPU will not branch
				// on these ifs. Instead it will
				// run all branchs but zero out
				// the logically not taken branches.
				// This is important because branching
				// is expensive on GPUs.

				// Note: If you would like to learn
				// more about the above please see SIMD
				// http://en.wikipedia.org/wiki/SIMD
	
				// Black
				float4 result = _Color1;

				// Light-grey
				if (r > 0.00)
					result = _Color2;

				// Dark-grey
				if (r > 0.50)
					result = _Color3;

				// White
				if (r == 1.00)
					result = _Color4;

				result.a = alpha;
				return result;
			}
			ENDCG
		}
	}
}