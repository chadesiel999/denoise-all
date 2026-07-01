#version 450


// Uniforms
layout(set=1,binding=0) uniform texture2D Texture;
layout(set=1,binding =1) uniform sampler TextureSampler;

// Varyings
layout(location=0) in vec4 v_color;
layout(location =1) in vec2 v_texCoords;
layout(location=0) out vec4 fsout_Color;

void main()
{
	fsout_Color = v_color * texture(sampler2D(Texture, TextureSampler), v_texCoords);
}
