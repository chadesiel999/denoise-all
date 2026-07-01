#version 450

layout(location =0) in vec2 in_Position;
layout(location =1) in vec2 in_Size;
layout(location =2) in float in_Polygon;
layout(location =3) in vec4 in_Color;

layout(location =0) out vec4 out_Color;
layout(location =1) out vec2 out_PolygonSize;
layout(location =2) out float out_Polygon;
layout(location =3) out float Skip;
layout(location=4) out float out_Slop;

layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
layout(set=0,binding=1) uniform LineInfo
{
    vec4 LineRange;
    float VerticalOffset;
    float HorizontalOffset;
    float Brightness;
    float Slop;
};

void main()
{
     out_Color = vec4(in_Color.xyz,Brightness/100 * in_Color.w);
     out_Polygon = in_Polygon;
     out_Slop = Slop;
     //out_PolygonSize =abs((Proj*View *vec4(in_Size.xy,0,1)).xy);
     out_PolygonSize.x = in_Size.x/(LineRange.y-LineRange.x)*2;
     out_PolygonSize.y = in_Size.y/(LineRange.w-LineRange.z)*2;
     float x = in_Position.x+HorizontalOffset;
     Skip = (x<LineRange.x || x>LineRange.y)?1:0;
     gl_Position =Proj*View*vec4(x,clamp(in_Position.y+VerticalOffset,LineRange.z,LineRange.w),0,1);
}
