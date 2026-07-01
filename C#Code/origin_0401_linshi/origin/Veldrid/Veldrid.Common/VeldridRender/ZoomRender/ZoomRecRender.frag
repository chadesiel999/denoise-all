#version 450

layout(location =0) out vec4 fsout_Color;


layout(set=0,binding =2) uniform ColorInfo
{
    vec4 Color;
};

void main()
{
    fsout_Color =Color;
}
