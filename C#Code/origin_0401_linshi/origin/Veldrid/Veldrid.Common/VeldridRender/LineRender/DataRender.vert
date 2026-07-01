#version 450

layout(location = 0) in vec2 in_Position;
layout(location = 0) out vec4 out_Color;
layout(location = 1) out float out_Brightness;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
layout(set=0,binding=1) uniform LineInfo
{
    vec4 Color;
    vec4 LineRange;
    float VerticalOffset;
    float HorizontalOffset;
    float Brightness;
    float Spare;
};
void main()
{
     gl_Position =Proj*View*vec4(clamp(in_Position.x+HorizontalOffset,LineRange.x,LineRange.y),clamp(in_Position.y+VerticalOffset,LineRange.z,LineRange.w),0,1);
     out_Brightness = Brightness;
    out_Color= Color;
}
