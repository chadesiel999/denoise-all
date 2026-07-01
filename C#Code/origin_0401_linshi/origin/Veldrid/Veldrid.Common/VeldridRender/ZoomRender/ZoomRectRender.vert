#version 450
struct WindowsInfo
{
   vec4 in_Local;
   vec4 in_Color;
   vec4 in_BorderColor ;
   vec4 in_WindowSize;
};

layout(location = 0) in vec2 Position;
layout(location = 0) out vec4 out_Local;
layout(location = 1) out vec4 out_Color;
layout(location = 2) out vec4 out_BorderColor;
layout(location = 3) out vec4 out_WindowSize;
layout(location = 4) out vec2 out_Position;


layout(set=0,binding=0) uniform WindowInfo
{
    WindowsInfo in_WindowsInfo;
};

void main()
{

    out_Local = in_WindowsInfo.in_Local;
    out_Color =  in_WindowsInfo.in_Color;
    out_BorderColor = in_WindowsInfo.in_BorderColor;
    out_WindowSize = in_WindowsInfo.in_WindowSize;
    out_Position = Position;
    gl_Position = vec4(Position, 0, 1);
}
