#version 450

const float gamma = 2.2;
layout(location =0) in float skip;
layout(location =0) out vec4 fsout_Color;

layout(set =0,binding =2) uniform ColorInfo
{
    vec4 Color;
    float Brightness;
    vec3 Spare;
};

vec4 gammaCorrect(vec4 color)
{
    return pow(color,vec4(1.0/gamma));
}
void main()
{
    if(skip==1)discard;
    fsout_Color =(vec4(Color.xyz,Brightness/100.0));//in_Color;
}
