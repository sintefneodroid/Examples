Shader "Custom/IgnoreZ"{ //Draws objects with shader on top always
  Properties{
    _Color("Color", Color) = (1,1,1,1)
    _Color2("Color2", Color) = (1,1,1,1)
  }
    SubShader{
      Tags{
        "Queue" = "Geometry+1"
      }
      Pass{
        ZTest Greater
        Color[_Color]
      }
      Pass{
        ZTest Less
        Color[_Color2]
      }
  }
}