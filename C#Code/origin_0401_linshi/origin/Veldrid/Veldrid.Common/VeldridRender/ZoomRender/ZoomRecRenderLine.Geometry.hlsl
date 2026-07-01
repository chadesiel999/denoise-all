struct GSInput
{
    float2 Size : TEXCOORD0;
    float4 Position : SV_POSITION;
};
struct GSOutput
{
    float4 Position : SV_POSITION;
};
[maxvertexcount(4)]
void main(line GSInput input[2] :SV_POSITION,
inout TriangleStream<GSOutput> output)
{
    GSOutput gsout;
    if (input[0].Position.x == input[1].Position.x)
    {
        float4 val1 = input[0].Position;
        float4 val2 = input[1].Position;
        if (val1.y>val2.y)
        {
            val1 = input[1].Position;
            val2 = input[0].Position;
        }
        gsout.Position = val1;
        output.Append(gsout);
        gsout.Position.x += input[0].Size.x;
        output.Append(gsout);
        gsout.Position = float4(val1.x, val2.yzw);
        output.Append(gsout);
        gsout.Position.x += input[0].Size.x;
        output.Append(gsout);

    }
    else
    {
        float4 val1 = input[0].Position;
        float4 val2 = input[1].Position;
        if (val2.x<val1.x)
        {
            val1 = input[1].Position;
            val2 = input[0].Position;
        }
        gsout.Position = val1;
        output.Append(gsout);
        gsout.Position = val2;
        output.Append(gsout);
        gsout.Position = float4(val1.x,val1.y+input[0].Size.y, val1.zw);
        output.Append(gsout);
        gsout.Position = float4(val2.x,val2.y + input[0].Size.y, val1.zw);
        output.Append(gsout);
    }
    output.RestartStrip();

}