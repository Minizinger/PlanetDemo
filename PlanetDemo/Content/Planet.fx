#define COAST 0.1

sampler s0 : register(s0);

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 LightDirection;
float LightIntensity;

float4 GroundColor;
float4 CoastColor;

struct VertexShaderInput
{
	float4 Position : SV_Position;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position;
	float4 Pos : TEXCOORD0;
	float4 Normal : TEXCOORD1;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Pos = worldPosition;

	output.Color = input.Color;
	//output.Color = GroundColor * DisplacementLength;

	//output.Normal = normalize(mul(input.Normal, World));
	output.Normal = normalize(worldPosition);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 light = saturate(dot(LightDirection, input.Normal));
	float4 color = GroundColor * 1 - input.Color;
	if (input.Color.x / 4 < 0.1)
		color = saturate(GroundColor * 1.1);

	return saturate(color + float4(1, 1, 1, 1) * light * LightIntensity);
}

technique PlanetShader
{
	pass P0
	{
		VertexShader = compile vs_5_0 MainVS();
		PixelShader = compile ps_5_0 MainPS();
	}
};