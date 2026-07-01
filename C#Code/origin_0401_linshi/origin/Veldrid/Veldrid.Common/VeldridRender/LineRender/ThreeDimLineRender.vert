#version 450

layout(location =0) in vec3 Position;
layout(location =0) out vec4 vout_Color;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
    mat4 Model;
};

layout(set=0,binding=1) uniform LineInfo
{
    vec4 Color;
};

void main()
{
     gl_Position =Proj * View* Model* vec4(Position,1.0);
     vout_Color = Color;
}