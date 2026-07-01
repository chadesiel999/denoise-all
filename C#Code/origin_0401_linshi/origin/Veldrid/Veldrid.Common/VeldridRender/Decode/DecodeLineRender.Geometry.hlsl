struct GSInput
{
    uint VertextIndex : TEXCOORD0;
    float4 Position : SV_POSITION;
};
struct GSOutput
{
    float4 Color : TEXCOORD0;
    float4 Position : SV_POSITION;
};
cbuffer ProjView : register(b0)
{
    float4x4 View;
    float4x4 Proj;
};
Buffer<uint> DataBuffer : register(t0);

cbuffer DecodeDataInfo : register(b1)
{
    uint ChannelCount;
    uint PerChannelDataLength;
    int InterwovenBitCount;
    float DataInterval;
    int TriggerIndex;
    float VerticalOffset;
    int Chindex;
    float Spare;
}

[maxvertexcount(33)]
void main(point GSInput input[1] :SV_POSITION,
inout LineStream<GSOutput> output)
{
    GSOutput gsout;
    GSInput gsin = input[0];
    uint data = DataBuffer[gsin.VertextIndex];
    int i = 0;
    switch (ChannelCount)
    {
        case 1:
            for (i = 0; i < 32;i++)
            {
                bool state = bool((data >> (31 - i)) & 0x01);
                float pos = VerticalOffset + (state ? 0 : 500);
                gsout.Color = state ? float4(0, 1, 0, 1) : float4(0, 0, 1, 1);
                int bitindex = gsin.VertextIndex * 32 + i - TriggerIndex;
                gsout.Position = mul(mul(float4(bitindex * DataInterval, pos, 0, 1), View), Proj);
                output.Append(gsout);

            }
            break;
        case 2:
            for (i = 0; i < 16;i++)
            {
                bool state = bool((data >> (31 - i - 16 * (1 - Chindex))) & 0x01);
                float pos = VerticalOffset + (state ? 0 : 500);
                gsout.Color = state ? float4(0, 1, 0, 1) : float4(0, 0, 1, 1);
                int bitindex = gsin.VertextIndex * 16 + i - TriggerIndex;
                gsout.Position = mul(mul(float4(bitindex * DataInterval, pos, 0, 1), View), Proj);
                output.Append(gsout);
            }
            break;
        case 3:
        case 4:
            for (i = 0; i < 8;i++)
            {
                bool state = bool((data >> (31 - i - (3 - ChannelCount)*8)) & 0x01);
                float pos = VerticalOffset + (state ? 0 : 500);
                gsout.Color = state ? float4(0, 1, 0, 1) : float4(0, 0, 1, 1);
                int bitindex = gsin.VertextIndex * 8 + i - TriggerIndex;
                gsout.Position = mul(float4(bitindex * DataInterval, pos, 0, 1), mul(View, Proj));
                output.Append(gsout);
            }
            break;
    }

}