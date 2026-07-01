#version 450

layout (constant_id = 0) const bool InvertY = false;

// Attributes
layout(location=0) in vec3 a_position;
layout(location=1) in vec4 a_color;
layout(location=2) in vec2 a_texCoords0;

// Uniforms
layout(set=0,binding =0) uniform a_MatrixTransform
{
    mat4x4 MatrixTransform;
};



void main()
{
	gl_Position = MatrixTransform * vec4(a_position, 1.0);
}
