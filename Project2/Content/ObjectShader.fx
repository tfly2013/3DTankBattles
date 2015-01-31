// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
//Further Modified for COMP30019 Project 2 by Ha Jin Song, 18/10/2014
// these won't change in a given iteration of the shader
float4x4 World;
float4x4 View;
float4x4 Projection;
float4 cameraPos;
float4x4 worldInvTrp;
//Use tank's position to add glowing effect (simulate sun's reflection on metal surface
float updateX;
float updateZ;

//Ambient
float4 Color_ambient = float4(1.0f,1.0f, 1.0f, 1.0f);
float I_ambient = 0.5f;

//Diffuse
float3 Light_Dir_Diffuse = float3(1.0f, 0.4f, 0.7f);
float4 Color_diffuse = float4(1.0f,1.0f, 1.0f, 1.0f);
float I_diffuse = 0.3f;

//Specular Reflection
float Material_sre = 150;
float4 Color_spec = float4(1.0f,1.0f, 1.0f, 1.0f);
//Highlight straight < dim , > bright
float I_specular = 0.6;
//Direction of the view
float3 ViewVector = float3(1,1,1);

texture2D ModelTexture;
SamplerState textureSampler{
    AddressU = Wrap;
    AddressV = Wrap;
};

//


struct VertexShaderIn
{
	float4 Position : SV_POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0; // this is new
	// Other vertex properties, e.g. texture co-ords, surface Kd, Ks, etc
};

struct VertexShaderOut
{
	float4 Position : SV_POSITION0;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1; // this is new
};


VertexShaderOut VertexShaderFunct(VertexShaderIn input)
{
	VertexShaderOut output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

    float4 normal = mul(input.Normal, worldInvTrp);
	//Diffusion
    float lightIntensity = dot(normal, Light_Dir_Diffuse);
	//Set Color
    output.Color = saturate(Color_diffuse * I_diffuse * lightIntensity);
	//Set Normal
	output.Normal = normal;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderFunct(VertexShaderOut input) : SV_Target
{
	//Diffusion
    float3 light = normalize(Light_Dir_Diffuse);
    float3 normal = normalize(input.Normal);
	//Normalise vectors for easier and faster calculation
    float3 r = normalize(2 * dot(light, normal) * normal - light);
    float3 v = normalize(mul(normalize(ViewVector), World));

    float dotProduct = dot(r, v);
	//Apply Specular
    float4 specular = I_specular * Color_spec * max(pow(dotProduct, Material_sre), 0) * length(input.Color);
	//Get texture for the model
	float4 textureColor = ModelTexture.Sample(textureSampler, input.TextureCoordinate);
	textureColor.a = 1;
	//Apply ambient
	return saturate(textureColor * ((input.Color) + Color_ambient * I_ambient + specular * updateZ *updateX));

}


technique Textured
{
	pass Pass1
	{
		Profile = 9.1;
		VertexShader = VertexShaderFunct;
		PixelShader = PixelShaderFunct;
	}
}