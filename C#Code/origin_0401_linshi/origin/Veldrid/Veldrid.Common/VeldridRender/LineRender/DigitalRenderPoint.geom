#version 450

layout(lines) in;
layout(points,max_vertices =3) out;

layout(location =0) in float skips[];
layout(location =0) out float skip;

void main()
{
        gl_Position = gl_in[0].gl_Position;
        skip = skips[0];
        EmitVertex();
        if(gl_in[0].gl_Position.y!=gl_in[1].gl_Position.y)
        {
            gl_Position = vec4(gl_in[1].gl_Position.x,gl_in[0].gl_Position.y,gl_in[0].gl_Position.z,gl_in[0].gl_Position.w);
            skip = skips[1];
            EmitVertex();
        }
	    gl_Position = gl_in[1].gl_Position;
        skip = skips[1];
        EmitVertex();
        EndPrimitive();
}