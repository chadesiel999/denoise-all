#version 450

layout(location =0) in vec4 in_Color;
layout(location =0) out vec4 fsout_Color;


void main()
{
    //if(in_Skip ==1)discard;
    fsout_Color =in_Color;
}
