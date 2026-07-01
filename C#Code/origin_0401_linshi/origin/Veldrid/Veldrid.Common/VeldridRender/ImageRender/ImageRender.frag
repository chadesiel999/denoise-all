#version 450

layout(location = 0) in vec2 tex_coord;
layout(location = 1) in vec4 bounds;
layout(location = 2) in vec2 pos;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Tex;
layout(set = 1, binding = 1) uniform sampler Sampler;

void main()
{
//    float left = bounds.x;
//    float top = bounds.y;
//    float right = bounds.z;
//    float bottom = bounds.w;
//
//    if(!(left <= pos.x && right >= pos.x &&
//        top <= pos.y && bottom >= pos.y))
//    {
//        discard;
//    }
    fsout_Color =texture(sampler2D(Tex, Sampler), tex_coord);
    //fsout_Color = vec4(fsout_Color.z, fsout_Color.w,fsout_Color.x,fsout_Color.y);
}
