struct GSInput
{
    float4 Color : TEXCOORD0;
    float2 PolygonSize : TEXCOORD1;
    float Polygon : TEXCOORD2;
    float Skip : TEXCOORD3;
    float Slop : TEXCOORD4;
    float4 Position : SV_POSITION;
};
struct GSOutput
{
    float4 Color : TEXCOORD0;
    float Skip : TEXCOORD1;
    float4 Position : SV_POSITION;
};
[maxvertexcount(6)]
void main(point GSInput input[1] : SV_POSITION,
inout TriangleStream<GSOutput> output)
{
    float slope = 0.2;
    GSOutput gsout;
    gsout.Position = input[0].Position;
    gsout.Color = input[0].Color;
    gsout.Skip = input[0].Skip;
    //if (gsout.Skip == 1)
    //    return;
    switch (input[0].Polygon)
    {
        case 0: // line
        case 1: //rectangle
            gsout.Position = float4(input[0].Position.x, input[0].Position.y - 0.5 * input[0].PolygonSize.y, input[0].Position.zw);
            output.Append(gsout);
            gsout.Position.x += input[0].PolygonSize.x;
            output.Append(gsout);
            gsout.Position.y += input[0].PolygonSize.y;
            gsout.Position.x = input[0].Position.x;
            output.Append(gsout);
            gsout.Position.x += input[0].PolygonSize.x;
            output.Append(gsout);
            output.RestartStrip();
            break;
        case 2://hexagon
            float xoffset = input[0].Slop;
            gsout.Position.x = input[0].Position.x + xoffset;
            gsout.Position.y = input[0].Position.y - input[0].PolygonSize.y / 2;
            output.Append(gsout);
            gsout.Position.x = input[0].Position.x + input[0].PolygonSize.x - xoffset;
            output.Append(gsout);
            gsout.Position = input[0].Position;
            output.Append(gsout);
            gsout.Position.x = input[0].Position.x + input[0].PolygonSize.x;
            output.Append(gsout);
            gsout.Position.x = input[0].Position.x + xoffset;
            gsout.Position.y += input[0].PolygonSize.y / 2;
            output.Append(gsout);
            gsout.Position.x = input[0].Position.x + input[0].PolygonSize.x - xoffset;
            output.Append(gsout);
            output.RestartStrip();
            break;
    }

}