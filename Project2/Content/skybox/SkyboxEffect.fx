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

float4 CameraPosition;

TextureCube SkyBoxText;

//Sampler for Skybox
SamplerState textureSampler{
	Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

//

//Data structure for VS input
struct VertexShaderIn
{
	float4 Position : SV_POSITION0;

};
//Data structure for VS output
struct VertexShaderOut
{
	float4 Position : SV_POSITION0;
	float4 TextureCoordinate : TEXCOORD0; 
};

//App to Vertex Shader
VertexShaderOut VertexShaderFunct(VertexShaderIn input)
{
	VertexShaderOut output;
	//Position of world
	float4 worldPosition = mul(input.Position, World);
	//Viewer's position
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	output.TextureCoordinate = VertexPosition - CameraPosition;



	return output;
}
//Vertex Shader to Pixel
float4 PixelShaderFunct(VertexShaderOut input) : SV_Target
{

	return SkyBoxText.Sample(textureSampler, normalize(input.TextureCoordinate));

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