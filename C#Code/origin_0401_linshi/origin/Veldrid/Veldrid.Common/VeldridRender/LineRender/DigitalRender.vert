#version 450
#extension GL_KHR_vulkan_glsl : enable

layout(location =0 ) in float in_Position;
layout(location =1) out float skip;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
layout(set=0,binding=1) uniform LineInfo
{
    vec4 LineRange;
    float VerticalOffset;
    float HorizontalOffset;
    float SampleRate;
    float ValueScale;
    float VerticalPos;
};
void main()
{
    float x = gl_VertexIndex/SampleRate+HorizontalOffset;
    skip = (x<LineRange.x || x>LineRange.y)?1:0;
    gl_Position =Proj*View*vec4(clamp(x,LineRange.x,LineRange.y),clamp((in_Position - VerticalPos)/ValueScale+VerticalOffset,LineRange.z,LineRange.w),0,1);
}
