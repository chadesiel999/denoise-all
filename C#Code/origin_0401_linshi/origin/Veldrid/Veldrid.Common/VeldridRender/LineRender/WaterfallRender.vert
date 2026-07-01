#version 450
#extension GL_KHR_vulkan_glsl : enable

layout(location =0) in float Position;
layout(location =0) out vec4 vout_Color;
layout(location =1) out float vout_skip;


layout(set=0,binding=0) uniform LineInfo
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
};

void main()
{

     vout_skip = gl_VertexIndex % int(FrameLenght);
     float y=0;	 
     float index = ceil((gl_VertexIndex +1) / FrameLenght);
     if(index<=FrameIndex)
     {
         y= (FrameIndex - index)/FrameCount*2-1;
     }
     else 
     {
         y=(FrameCount+ FrameIndex - floor((gl_VertexIndex +1) / FrameLenght))/FrameCount*2-1;
     }
     gl_Position = vec4(vout_skip/FrameLenght*2-1,y,0,1.0);
     //gl_Position = vec4(1.0/SampleRate *gl_VertexIndex/1000.0,Position,0.0,1.0);
     vec4 color1;
     if(Position<=MinValue)color1 = MinColor;
     else if(Position>MinValue && Position<=MiddleValue) color1= mix(MinColor,MiddleColor,(Position - MinValue)/(MiddleValue-MinValue));
     else if(Position>MiddleValue&& Position<=LastValue) color1 = mix(MiddleColor,LastColor,(Position-MiddleValue)/(LastValue-MiddleValue));
     else color1 = LastColor;
     vout_Color = vec4(color1.xyz,Brightness/100.0);
}