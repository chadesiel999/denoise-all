#version 450

layout(location = 0) in vec2 tex_coord;
layout(location = 1) in vec4 bounds;
layout(location = 2) in vec2 pos;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Tex;
layout(set = 1, binding = 1) uniform sampler Sampler;
layout(set=2,binding =0) uniform InfiniteInfo
{
    float Status;
    vec3 LocalColor;
};
layout(set=2,binding=1) uniform Colors
{
    float Transparency1;
    float Transparency2;
    float Transparency3;
    float Transparency4;
    vec4 Color1;
    vec4 Color2;
    vec4 Color3;
    vec4 Color4;
};
void main()
{
    float left = bounds.x;
    float top = bounds.y;
    float right = bounds.z;
    float bottom = bounds.w;

    if(!(left <= pos.x && right >= pos.x &&
        top <= pos.y && bottom >= pos.y))
    {
        discard;
    }
    vec4 first = vec4(0,0,0,0);
    vec4 last = vec4(0,0,0,0);
    vec4 tempcolor = texture(sampler2D(Tex, Sampler), tex_coord);
    if(Status !=0)
    {
        if(tempcolor.w<=Transparency1)
        {
            last= Color1;
        }
        else if(tempcolor.w>Transparency1 && tempcolor.w<=Transparency2)
        {
            first = Color1;
            last = Color2;
        }
        else if(tempcolor.w>Transparency2 && tempcolor.w<=Transparency3)
        {
            first = Color2;
            last = Color3;
        }
        else if(tempcolor.w>Transparency3 && tempcolor.w<=Transparency4)
        {
             first =Color3;
             last = Color4;
        }
        else if(tempcolor.w>Transparency4)
        {
            first = Color4;
            last = Color4;
        }
        fsout_Color =mix(first,last,tempcolor.w);
    }
    else
    {
        fsout_Color =tempcolor;
    }
}
