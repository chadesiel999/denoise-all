#version 450
#extension GL_KHR_vulkan_glsl : enable

layout (constant_id = 0) const bool InvertY = false;

layout(location = 0) in vec2 Position;
layout(location = 0) out vec2 tex_coord;
layout(location = 1) out vec4 bounds;
layout(location = 2) out vec2 pos;
layout(location =3) out vec4 color;

struct Item 
{
    mat4 source;
    vec4 color;
    mat4 model;
    mat4 projection;
    vec4 scissor;
};

layout(std430, binding = 0) readonly buffer Items
{
    mat4 view;
    Item items[];
};

void main()
{
   
    Item item = items[gl_InstanceIndex];
    tex_coord = (item.source * vec4(Position, 0, 1)).xy;
    color = item.color/255.0;
    // scissor bounds
    vec2 start = item.scissor.xy;
    vec2 end = start + item.scissor.zw;
    bounds = vec4(start, end);
    gl_Position = item.projection * view * item.model * vec4(Position, 0, 1);
    pos = gl_Position.xy;

    if(!InvertY)
        gl_Position.y = -gl_Position.y;


}
