#version 450
layout(location =0) in float in_Position;
layout(location =1) in int in_iskey;

layout(location =0) out float VSOut_Width;
layout(location =1) out float VSOut_Ratio;


layout(set=0,binding=0) uniform ProjView
{
    mat4 View;
    mat4 Proj;
};
layout(set=0,binding=1) uniform LineInfo
{
    float Top;
    float Width;
    float AspectRatio;
    float HorizontalOffset;
};

void main()
{    
      if(in_iskey!=0)
     {
        VSOut_Width = 2 * Width;
     }
     else
     {
        VSOut_Width = Width;
     }
     VSOut_Ratio = AspectRatio;
     gl_Position =Proj*View*vec4(in_Position+HorizontalOffset,Top,0,1);
}
