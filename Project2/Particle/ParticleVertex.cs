// Authors: Fei Tang, Kimple Ke, Ha Jin Song
// Last Modified: 20/10/2014
// ParticleVertex file, structure for particle vertices
//Use SharpDX

using SharpDX;
using SharpDX.Toolkit.Graphics;
using System.Runtime.InteropServices;

namespace Project2
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticleVertex
    {
        [VertexElement("POSITION")]
        public Vector3 Position;

        [VertexElement("NORMAL")]
        public Vector3 Velocity;

        [VertexElement("COLOR")]
        public Vector4 Color;

        [VertexElement("TEXCOORD0")]
        public Vector2 TimerLifetime;

        [VertexElement("TEXCOORD1")]
        public uint Flags;

        [VertexElement("TEXCOORD2")]
        public Vector2 SizeStartEnd;
    }
}