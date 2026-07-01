struct GSOutput
{
    float skip : TEXCOORD0;
	float4 pos : SV_POSITION;
};

[maxvertexcount(3)]
void main(line GSOutput input[2] : SV_POSITION, 
	inout PointStream<GSOutput> output)
{
    GSOutput element;
    element = input[0];
    output.Append(element);

    if (input[0].pos.y!= input[1].pos.y)
    {
        element.pos = float4(input[1].pos.x, input[0].pos.y, input[0].pos.z, input[0].pos.w);
        element.skip = input[1].skip;
        output.Append(element);
    }

    element = input[1];
    output.Append(element);
	
    output.RestartStrip();
}