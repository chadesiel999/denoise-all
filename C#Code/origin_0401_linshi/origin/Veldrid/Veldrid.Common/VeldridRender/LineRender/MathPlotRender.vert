#version 450
#extension GL_KHR_vulkan_glsl : enable

layout (constant_id = 0) const bool InvertY = false;
layout(location =0 ) in float in_Position;
layout(location = 0) out vec4 out_Color;
layout(location =1) out float out_Brightness;
layout(location =2) out float skip;

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
    float SampleRate;
};
void main()
{
    float x = 1.0/SampleRate*gl_VertexIndex+HorizontalOffset;
    skip = (x<LineRange.x || x>LineRange.y)?1:0;
    if(in_Position ==-3.40282347E+38F)
    {
        gl_Position = vec4(0.0);
        out_Brightness = 0.0;
        out_Color= vec4(0.0);
    }
    else
    {
        gl_Position = Proj*View*vec4(clamp(x,LineRange.x,LineRange.y),clamp(in_Position+VerticalOffset,LineRange.z,LineRange.w),0,1);
        out_Brightness = Brightness;
        out_Color= Color;
    }
}
