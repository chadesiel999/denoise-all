#version 450
layout(location =0) out uint out_vertexIndex;

void main()
{
    out_vertexIndex = uint(gl_VertexIndex);
}
