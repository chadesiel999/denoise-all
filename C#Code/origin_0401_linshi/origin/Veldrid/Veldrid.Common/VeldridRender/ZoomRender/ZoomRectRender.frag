#version 450


layout(location = 0) in vec4 out_Local;
layout(location = 1) in vec4 out_Color;
layout(location = 2) in vec4 out_BorderColor;
layout(location = 3) in vec4 out_WindowSize;
layout(location = 4) in vec2 out_Position;

layout(location = 0) out vec4 fsout_Color;

void main()
{
  vec2 tempposition =vec2((out_Position.x+1)/2*out_WindowSize.x,(out_Position.y*(-1)+1)/2*out_WindowSize.y);
    if((((tempposition.x >= out_Local.x-2)&&tempposition.x <= out_Local.x) ||((tempposition.x >= out_Local.y)&&(tempposition.x <= out_Local.y+2))) &&(tempposition.y >=out_Local.z-2 && tempposition.y <=out_Local.w+2))    
    {
        fsout_Color = out_BorderColor;
    }
    else if((((tempposition.y >= out_Local.z-2)&&tempposition.y <= out_Local.z )||((tempposition.y >= out_Local.w)&&(tempposition.y <= out_Local.w+2))) && (tempposition.x >=out_Local.x-2 &&  tempposition.x <=out_Local.y+2))    
    {
        fsout_Color = out_BorderColor;
    }

    else if((tempposition.x > out_Local.x && tempposition.x <out_Local.y) && (tempposition.y > out_Local.z &&tempposition.y < out_Local.w))
    //else if((tempposition.x >-0.5f && tempposition.x <0.5f) && (tempposition.y >-0.5f &&tempposition.y <0.5f))
    {
       discard;
    }
    else
    {
         fsout_Color = out_Color;
    }
}
