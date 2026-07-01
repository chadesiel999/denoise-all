struct GSInput
{
    float Width : TEXCOORD0;
    float BaseValue : TEXCOORD1;
    float Orientation : TEXCOORD2;
    int Skip : TEXCOORD3;
    float4 Position : SV_POSITION;
};
struct GSOutput
{
    float4 Position : SV_POSITION;
};
[maxvertexcount(5)]
void main(point GSInput input[1] :SV_POSITION,
inout LineStream<GSOutput> output)
{
    GSInput inputdata = input[0];
    GSOutput gsout;
    if (inputdata.Skip != 0)
        return;
    if (inputdata.Orientation == 0)
    {
        gsout.Position = inputdata.Position;
        output.Append(gsout);
        gsout.Position.y -= inputdata.Width;
        output.Append(gsout);
        gsout.Position.x = inputdata.BaseValue;
        output.Append(gsout);
        gsout.Position.y = inputdata.Position.y;
        output.Append(gsout);
        gsout.Position = inputdata.Position;
        output.Append(gsout);

    }
    else
    {
        gsout.Position = inputdata.Position;
        output.Append(gsout);
        gsout.Position.x += inputdata.Width;
        output.Append(gsout);
        gsout.Position.y = inputdata.BaseValue;
        output.Append(gsout);
        gsout.Position.x = inputdata.Position.x;
        output.Append(gsout);
        gsout.Position = inputdata.Position;
        output.Append(gsout);
    }
    output.RestartStrip();

}