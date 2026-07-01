#version 450
layout(location =0) in vec2 in_Position;

layout(location =0) out vec2 VSOut_Size;


layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
layout(set=0,binding=1) uniform LineInfo
{
    vec2 Size;
    vec2 Spare;
};

void main()
{
     VSOut_Size = Size;
     gl_Position =Proj*View*vec4(in_Position.xy,0,1);
}
