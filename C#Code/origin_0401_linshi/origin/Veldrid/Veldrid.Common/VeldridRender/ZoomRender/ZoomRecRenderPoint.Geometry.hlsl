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
void main(point GSInput input[1] :SV_POSITION,
inout TriangleStream<GSOutput> output)
{
    GSOutput gsout;
    float2 size = input[0].Size * 4;
    gsout.Position = input[0].Position;
    gsout.Position.x -= size.x / 2;
    gsout.Position.y -= size.y / 2;
    output.Append(gsout);
    gsout.Position.x += size.x;
    output.Append(gsout);
    gsout.Position = input[0].Position;
    gsout.Position.x -= size.x / 2;
    gsout.Position.y += size.y / 2;
    output.Append(gsout);
    gsout.Position.x += size.x;
    output.Append(gsout);
    output.RestartStrip();

}