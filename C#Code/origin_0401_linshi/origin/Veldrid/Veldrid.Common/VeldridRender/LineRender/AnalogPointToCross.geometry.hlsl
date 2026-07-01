struct GSOutput
{
    float4 color : TEXCOORD0;
    float Brightness : TEXCOORD1;
    float skip : TEXCOORD2;
    float2 unitsPerPx : TEXCOORD3;
	float4 pos : SV_POSITION;
};

[maxvertexcount(4)]
void main(point GSOutput input[1] : SV_POSITION, 
	inout LineStream<GSOutput> output)
{
    GSOutput element;
    element = input[0];
    element.pos = float4(input[0].pos.x - input[0].unitsPerPx.x/2, input[0].pos.yzw);
    output.Append(element);
    element.pos = float4(element.pos.x + input[0].unitsPerPx.x, input[0].pos.yzw);
    output.Append(element);
    output.RestartStrip();
    
    
    element.pos = float4(input[0].pos.x, input[0].pos.y - input[0].unitsPerPx.y/2, input[0].pos.zw);
    output.Append(element);
    element.pos = float4(input[0].pos.x, element.pos.y + input[0].unitsPerPx.y, input[0].pos.zw);
    output.Append(element);
    output.RestartStrip();
    
}