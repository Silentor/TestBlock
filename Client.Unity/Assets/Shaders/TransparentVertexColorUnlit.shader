Shader "Custom/Transparent vertex color unlit" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True"  }
	//Blend SrcAlpha OneMinusSrcAlpha
	Lighting Off
	Cull off
	//ZWrite On
	AlphaTest Greater .1
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				Combine texture * primary 
			}
		}
	}
}
}