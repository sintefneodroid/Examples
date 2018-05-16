Shader "Custom/DepthCameraShaderDeprecated" {
  Properties{
    //_FarPlane("Far plane", Range(0.02,50)) = 20.07 // sliders
  }
  SubShader{
    Tags{ "RenderType" = "Opaque" }

    Pass{
      Fog{ Mode Off }
      CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _CameraDepthTexture;

  //float _FarPlane;

  struct v2f {
    float4 pos : SV_POSITION;
    float4 uv: TEXCOORD1;
  };

  v2f vert(appdata_base v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = ComputeScreenPos(o.pos);
    return o;
  }

  half4 frag(v2f i) : COLOR {
    //float depth01 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv))) * _FarPlane;
    //float depth01 = (depth * _ProjectionParams.z - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);
    //float depth = tex2D(_CameraDepthTexture, (i.uv)).r * 50;
    //float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv)) * 50;

    float depth = tex2D(_CameraDepthTexture, i.uv);
    #if defined(UNITY_REVERSED_Z)
      depth = 1.0f - depth;
    #endif

    //float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
    //depth = LinearEyeDepth(depth);
    //depth = Logarithmic01Depth(depth);
    //depth = Linear01Depth(depth);

    //half4 color = half4(depth, depth, depth, 1);
    half4 color = half4(1 - depth, 1 - depth, 1 - depth, 1);
  return color;
}
ENDCG
}
  }
    FallBack "Diffuse"
}