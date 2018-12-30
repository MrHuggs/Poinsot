Shader "Unlit/CalcIntersection"
{
	Properties
	{
		_HighlightColor("Highlight Color", Color) = (1, 0, 1, .5) //Color when intersecting
		_WorldPos("World Position", Vector) = (0,0,0,0)
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"  }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Off
			Cull Back

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _HighlightColor;
		uniform float4 _WorldPos;

			struct v2f
			{
				float3 pos : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD1;
			};

			v2f vert (appdata_base  v)
			{
				v2f o;

				o.pos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.pos -= _WorldPos.xyz;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float d2 = i.pos.x * i.pos.x +
						  i.pos.y * i.pos.y +
						  (i.pos.z + 0) * (i.pos.z + 0);

				float diff = abs(d2 - 1);

				if (diff < .006)
					return half4(1, 1, 1, 1);
				else
					return half4(0, 0, 0, 0);

			}

			ENDCG
		}
	}
		FallBack "VertexLit"
}

