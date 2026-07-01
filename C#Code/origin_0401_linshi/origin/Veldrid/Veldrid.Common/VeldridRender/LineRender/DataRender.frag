#version 450

const float gamma = 2.2;
layout(location =0) in vec4 in_Color;
layout(location =1) in float in_Brightness;
layout(location =0) out vec4 fsout_Color;

vec4 gammaCorrect(vec4 color)
{
    return pow(color,vec4(1.0/gamma));
}
void main()
{
    fsout_Color =(vec4(in_Color.xyz,in_Brightness/100.0));//in_Color;
}
