#version 450

layout(location =0) in vec4 fin_Color;
layout(location =0) out vec4 fout_Color;

void main()
{
    fout_Color = fin_Color;
}