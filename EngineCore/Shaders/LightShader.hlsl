cbuffer MatrixBuffer :register(b0)
{
	float4x4 world;
	float4x4 view;
	float4x4 projection;
}

cbuffer LightBuffer :register(b0)
{
	float4 diffuseColor;
	float4 lightDirection;
}

cbuffer AmbientBuffer :register(b1)
{
	float4 ambientColor;
}

struct VertexInput
{
	float4 position : POSITION;
	float4 normal : NORMAL;
	float4 color : COLOR;
};

struct PixelInput
{
	float4 position : SV_POSITION;
	float4 normal : NORMAL;
	float4 color : COLOR;
};

PixelInput VS(VertexInput input)
{
	PixelInput output = (PixelInput) 0;

	float4 worldPosition = mul(world, input.position);
	float4 viewPosition = mul(view, worldPosition);
	output.position = mul(projection, viewPosition);
	output.normal = mul(world, input.normal);
	output.color = input.color;

	return output;
}

float4 PS(PixelInput input) : SV_Target
{
	float4 color;
	float4 lightDir = -normalize(lightDirection);
	float lightEffectiveness = saturate(dot(input.normal, lightDir));
	float4 lightColor = saturate(diffuseColor * lightEffectiveness);
	return saturate((lightColor * input.color) + (ambientColor * input.color));
}