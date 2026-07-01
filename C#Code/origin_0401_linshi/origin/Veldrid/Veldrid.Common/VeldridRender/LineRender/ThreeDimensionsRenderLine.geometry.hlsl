struct GSInput
{
    float4 color : TEXCOORD0;
    float skip : TEXCOORD1;
	float4 pos : SV_POSITION;
};
struct GSOutput
{
    float4 color : TEXCOORD0;
	float4 pos : SV_POSITION;
};

[maxvertexcount(3)]
void main(line GSInput input[2] : SV_POSITION, 
	inout LineStream<GSOutput> output)
{
    if (input[1].skip < input[0].skip)
    {
        return;
    }
    else
    {
        GSOutput element;
        element.color = input[0].color;
        element.pos = input[0].pos;
        output.Append(element);
    
        element.color = input[1].color;
        element.pos = input[1].pos;
        output.Append(element);
	
        output.RestartStrip();
    }
}