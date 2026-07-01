struct GSInput
{
    float Width : TEXCOORD0;
    float AspectRatio : TEXCOORD1;
    float4 Position : SV_POSITION;
};
struct GSOutput
{
    float4 Position : SV_POSITION;
};
[maxvertexcount(3)]
void main(point GSInput input[1] :SV_POSITION,
inout TriangleStream<GSOutput> output)
{
    GSOutput gsout;
    gsout.Position = float4(input[0].Position.x - input[0].Width / 2, input[0].Position.y, input[0].Position.zw);
    output.Append(gsout);
    gsout.Position.x += input[0].Width;
    output.Append(gsout);
    gsout.Position = float4(input[0].Position.x, input[0].Position.y - input[0].Width*input[0].AspectRatio, input[0].Position.zw);
    output.Append(gsout);
    output.RestartStrip();

}