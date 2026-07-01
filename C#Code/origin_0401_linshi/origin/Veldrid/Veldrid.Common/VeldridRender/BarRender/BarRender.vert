#version 450
#extension GL_KHR_vulkan_glsl : enable

layout(location =0) in float in_Position;

layout(location =0) out float out_Width;
layout(location =1) out float out_BaseValue;
layout(location =2) out float out_Orientation;
layout(location =3) out int Skip;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
layout(set=0,binding=1) uniform LineInfo
{
    float Start;
    float BaseValue;
    float AbsBaseValue;
    float Width;
    float AbsWidth;
    float YValueOffset;
    float XValueOffset;
    float Orientation;//0:Horizontal,1:Vertical
};

void main()
{
     out_Width = AbsWidth;
     out_Orientation = Orientation;
     out_BaseValue = AbsBaseValue;
     vec2 pos= vec2(0,0);
     vec4 size = vec4(0,0,0,0);
     if(Orientation ==0)
     {
         pos = vec2(in_Position,Start+gl_VertexIndex*Width);
     }
     else 
     {
         pos = vec2(gl_VertexIndex*Width+Start,in_Position);
     }
     Skip = int(in_Position <= BaseValue);
     
     gl_Position =Proj*View*vec4(pos,0,1);
}
