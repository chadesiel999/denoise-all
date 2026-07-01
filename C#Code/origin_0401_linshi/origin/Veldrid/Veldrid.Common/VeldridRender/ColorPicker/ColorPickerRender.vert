#version 450
layout(location =0) in vec2 in_Position;
layout(location =1) in vec4 in_Color;

layout(location =0) out vec4 out_Color;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
void main()
{
     out_Color = in_Color;
     gl_Position =Proj*View*vec4(in_Position,0,1);
}
