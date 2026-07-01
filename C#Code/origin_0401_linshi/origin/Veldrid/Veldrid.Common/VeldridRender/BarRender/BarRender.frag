#version 450
const float gamma = 2.2;
layout(location =0) out vec4 fsout_Color;

layout(set =0,binding =2) uniform ColorInfo
{
    vec4 Color;
};

void main()
{
    fsout_Color =vec4(Color.xyz,pow(Color.w,1.0/gamma));
}
