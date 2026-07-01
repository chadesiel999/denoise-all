#version 450
#extension GL_KHR_vulkan_glsl : enable

layout(location =0) in float Position;
layout(location =0) out vec4 vout_Color;
layout(location =1) out float vout_skip;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
    mat4 Model;
};

layout(set=0,binding=1) uniform LineInfo
{
    vec4 MinColor;
    vec4 MiddleColor;
    vec4 LastColor;
    float MinValue;
    float MiddleValue;
    float LastValue;
    float Brightness;

    float TotalFrameCount;
    float FrameLenght;
    float FrameIndex;
    float FrameCount;
    

    float Max;
    float Min;
    float First;
};

void main()
{
     float value = mix(1,-1,(Max-Position)/(Max-Min));
     vout_skip = gl_VertexIndex % int(FrameLenght);
     vec3 tempvalue = vec3(vout_skip/FrameLenght*2-1,value,0);
     float index = ceil((gl_VertexIndex +1) / FrameLenght);
     if(First ==0)
     {
         tempvalue.z = (FrameIndex - index)/FrameCount*-2+1;
     }
     else 
     {
         tempvalue.z = (FrameCount - index)/FrameCount*-2+1;
     }
     gl_Position =Proj * View* Model* vec4(tempvalue,1.0);
     vec4 color1;
     if(Position<=MinValue)color1 = MinColor;
     else if(Position>MinValue && Position<=MiddleValue) color1= mix(MinColor,MiddleColor,(Position - MinValue)/(MiddleValue-MinValue));
     else if(Position>MiddleValue&& Position<=LastValue) color1 = mix(MiddleColor,LastColor,(Position-MiddleValue)/(LastValue-MiddleValue));
     else color1 = LastColor;
     vout_Color = vec4(color1.xyz,Brightness/100.0);
}