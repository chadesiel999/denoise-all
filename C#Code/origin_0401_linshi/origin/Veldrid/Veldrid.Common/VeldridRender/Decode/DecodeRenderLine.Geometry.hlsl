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
[maxvertexcount(7)]
void main(point GSInput input[1] :SV_POSITION,
inout LineStream<GSOutput> output)
{
    GSOutput gsout;
    gsout.Color = input[0].Color;
    gsout.Position = input[0].Position;
    gsout.Skip = input[0].Skip;
    //if (gsout.Skip == 1)
    //    return;
    //output.Append(gsout);
    //output.Append(gsout);
    //output.RestartStrip();
        switch (input[0].Polygon)
        {
            case 3: // line
                if (input[0].PolygonSize.x == 0)
                {
                    gsout.Position = float4(input[0].Position.x, input[0].Position.y - 0.5 * input[0].PolygonSize.y, input[0].Position.zw);
                    output.Append(gsout);
                    gsout.Position = float4(input[0].Position.x, input[0].Position.y + 0.5 * input[0].PolygonSize.y, input[0].Position.zw);
                    output.Append(gsout);
                    output.RestartStrip();
                }
                else
                {
                    gsout.Position = input[0].Position;
                    output.Append(gsout);
                    gsout.Position.x += input[0].PolygonSize.x;
                    output.Append(gsout);
                    output.RestartStrip();
                }
                break;
            case 4: //rectangle
                gsout.Position = float4(input[0].Position.x, input[0].Position.y - 0.5 * input[0].PolygonSize.y, input[0].Position.zw);
                output.Append(gsout);
                gsout.Position.x += input[0].PolygonSize.x;
                output.Append(gsout);
                gsout.Position.y += input[0].PolygonSize.y;
                output.Append(gsout);
                gsout.Position.x -= input[0].PolygonSize.x;
                output.Append(gsout);
                gsout.Position.y -= input[0].PolygonSize.y;
                output.Append(gsout);
                output.RestartStrip();
                break;
            case 5: //hexagon
                float xoffset = input[0].Slop;
                gsout.Position = input[0].Position;
                output.Append(gsout);
                gsout.Position.x += xoffset;
                gsout.Position.y += input[0].PolygonSize.y / 2;
                output.Append(gsout);
                gsout.Position.x += input[0].PolygonSize.x - xoffset * 2;
                output.Append(gsout);
                gsout.Position.x = input[0].Position.x + input[0].PolygonSize.x;
                gsout.Position.y = input[0].Position.y;
                output.Append(gsout);
                gsout.Position.x -= xoffset;
                gsout.Position.y -= input[0].PolygonSize.y / 2;
                output.Append(gsout);
                gsout.Position.x = input[0].Position.x + xoffset;
                output.Append(gsout);
                gsout.Position = input[0].Position;
                output.Append(gsout);
                output.RestartStrip();
                break;
        }

}