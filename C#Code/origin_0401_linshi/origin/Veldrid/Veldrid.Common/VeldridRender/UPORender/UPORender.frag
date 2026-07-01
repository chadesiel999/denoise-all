#version 450
const float gamma = 2.2;
const float angle = 0.041;
layout(location = 0) in vec2 tex_coord;
layout(location = 1) in vec4 bounds;
layout(location = 2) in vec2 pos;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Tex;
layout(set = 1, binding = 1) uniform sampler Sampler;

layout(set =2,binding =0) uniform ChInfoBuffer
{
    int MaxValue;
    int MinValue;
    int ChOnCount;
    //当启用1个通道时表示启用的通道序号，从0开始，当启用两个通道时表示第一个启用的通道需要
    int ChIndex1;
    //当启用两个通道时表示第一个启用的通道需要
    int ChIndex2;
    int Brightness;
    vec2 Spare1;
    mat4x4 Colors;
};
vec3 rgbToHsl(vec3 c)
{ 
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0); 
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g)); 
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r)); 
    
    float d = q.x - min(q.w, q.y); 
    float e = 1.0e-10; 
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x); 
} 
vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 hue_to_rgb(float hue)
{
    float R = abs(hue * 6.0 - 3.0) - 1.0;
    float G = 2.0 - abs(hue * 6.0 - 2.0);
    float B = 2.0 - abs(hue * 6.0 - 4.0);
    return clamp(vec3(R,G,B),0,1);
}
vec3 hsl_to_rgb(vec3 hsl)
{
    vec3 rgb = hue_to_rgb(hsl.x);
    float C = (1.0 - abs(2.0 * hsl.z - 1.0)) * hsl.y;
    return (rgb - 0.5) * C + hsl.z;
}
vec3 hsv2rgb(vec3 c )
{
    vec3 rgb = clamp( abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );

    return c.z * mix( vec3(1.0), rgb, c.y);
}
vec4 gammaCorrect(vec4 color)
{
    return pow(color,vec4(1.0/gamma));
}
//smoothstep
void main()
{;
    vec4 color = texture(sampler2D(Tex, Sampler), tex_coord);
    int grayscale =  int(floor(color.x*255.0+0.5));//灰度值，0~255
    //color.w = grayscale/float(MaxValue);
    float left = bounds.x;
    float top = bounds.y;
    float right = bounds.z;
    float bottom = bounds.w;

    if(!(left <= pos.x && right >= pos.x &&
        top <= pos.y && bottom >= pos.y))
    {
        discard;
    }
    vec3 hsvcolor = rgb2hsv(color.xyz);
    float proportion = (grayscale - MinValue)/float(MaxValue-MinValue);
    color = vec4(hsv2rgb(mix(hsvcolor,clamp(hsvcolor+vec3(angle,0,0),vec3(0,0,0),vec3(1,1,1)),proportion)),color.w*(Brightness/100.0));
    //if(grayscale==0)color = vec4(0,0,0,0);
    fsout_Color = color;
    //fsout_Color =vec4(color.xyz,0.5f);
}

