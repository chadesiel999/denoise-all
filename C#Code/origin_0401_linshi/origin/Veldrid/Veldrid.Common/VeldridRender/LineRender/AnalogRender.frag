#version 450

const float gamma = 2.2;
layout(location =0) in vec4 in_Color;
layout(location =1) in float brightness;
layout(location =2) in float skip;
layout(location =3) in vec2 UnitsPerPx;
layout(location =0) out vec4 fsout_Color;

vec3 rgbToHsl(vec3 c)
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
vec4 gammaCorrect(vec4 color)
{
    return pow(color,vec4(1.0/gamma));
}
void main()
{
    if(skip==1) discard;
    fsout_Color = vec4(in_Color.xyz,brightness/100.0);
}
