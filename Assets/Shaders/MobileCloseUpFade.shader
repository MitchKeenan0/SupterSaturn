Shader "Hidden/MobileCloseUpFade"
{
	Properties{
		_DiffuseTex("Diffuse (RGBA)", 2D) = "white" {}
	_SpecularTex("Specular (RGB)", 2D) = "white" {}
	_SpecularPower("Specular Power", float) = 10
		_Opacity("Opacity", Range(0,1)) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" "LightMode" = "ForwardBase" }
		ZWrite Off
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _DiffuseTex;
			uniform sampler2D _SpecularTex;
			uniform float4 _DiffuseTex_ST;
			uniform float4 _SpecularTex_ST;
			uniform float4 _LightColor0;
			uniform float _SpecularPower;
			uniform float _Opacity;


			struct vIn
			{
				float4 vertex : POSITION;
				half4 normal : NORMAL;
				half2 coord0 : TEXCOORD0;
				half2 coord1 : TEXCOORD1;
			};
			struct vOut
			{
				float4 pos : SV_POSITION;
				half3 worldPos : COLOR;
				fixed3 normal : NORMAL;
				half2 diffuseCoord : TEXCOORD0;
				half2 specularCoord : TEXCOORD1;
			};

			vOut vert(vIn v)
			{
				vOut o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = normalize(mul(v.normal, unity_WorldToObject).xyz);

				o.diffuseCoord = v.coord0 * _DiffuseTex_ST.xy + _DiffuseTex_ST.zw;
				o.specularCoord = v.coord1 * _SpecularTex_ST.xy + _SpecularTex_ST.zw;

				return o;
			}

			float4 frag(vOut i) : COLOR
			{
				float3 diffuseColor = tex2D(_DiffuseTex, i.diffuseCoord);
				float3 specularColor = tex2D(_SpecularTex, i.specularCoord);
				fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos).xyz;
				float3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb * diffuseColor;
				float3 diffuseRef = _LightColor0.rgb * diffuseColor * max(dot(i.normal, lightDirection), 0.0);
				float3 specularRef = _LightColor0.rgb * specularColor * pow(max(dot(viewDirection,reflect(-lightDirection, i.normal)), 0.0),_SpecularPower);


				return float4(ambientLight + diffuseRef + specularRef,_Opacity);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
