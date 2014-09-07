Shader "Custom/Vertex color unlit" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}

Category {
	Lighting Off
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