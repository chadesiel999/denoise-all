#version 450

layout(location=0) out vec4 fsout_Color;
layout(set=1,binding =0) uniform a_color
{
	vec4 Color;
};

void main()
{
	fsout_Color = Color;
}
