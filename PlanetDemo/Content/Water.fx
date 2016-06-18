sampler s0 : register(s0);

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 LightDirection;
float LightIntensity;

float4 WaterColor;

float Time;

struct VertexShaderInput
{
	float4 Position : SV_Position;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position;
	float4 Pos : TEXCOORD0;
	float4 Normal : TEXCOORD1;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);

	float4 newWolrdPosition = worldPosition + (normalize(worldPosition) * sin(worldPosition * 50 + Time) * 0.02);

	//float4 displace = normalize(newWolrdPosition) - worldPosition;

	//float4 newNormal = worldNormal + displace;

	float4 viewPosition = mul(newWolrdPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Pos = newWolrdPosition;

	//output.Normal = -normalize(newNormal);
	output.Normal = normalize(newWolrdPosition);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 diffuse = saturate(dot(LightDirection, input.Normal));

	return saturate(WaterColor + float4(1, 1, 1, 1) * diffuse * LightIntensity);
}

technique WaterShader
{
	pass P0
	{
		VertexShader = compile vs_5_0 MainVS();
		PixelShader = compile ps_5_0 MainPS();
	}
};