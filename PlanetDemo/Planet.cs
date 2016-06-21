using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PlanetDemo
{
    public class Planet
    {
        Random rng;
        Ocean ocean;

        Vector4 GroundColor, CoastColor;

        Camera3D camera;

        public Vector3 Scale;
        public Vector3 Position;
        public Quaternion Rotation;

        Matrix world { get { return Matrix.CreateWorld(Position, Vector3.Transform(Vector3.Forward, Rotation), Vector3.Transform(Vector3.Up, Rotation)); } }
        public Matrix Transform { get { return Matrix.Multiply(Matrix.CreateTranslation(Position), Matrix.CreateScale(Scale)); } }
        
        OpenSimplexNoise noise;

        Effect planetEffect;

        List<Triangle> Triangles;
        public VertexPositionColor[] VerticiesToDraw;

        Tessellation Tessellation;

        public Planet(Vector3 position, Quaternion rotation, Vector3 scale, Camera3D cam)
        {
            rng = new Random();

            Position = position;
            Rotation = rotation;
            Scale = scale;
            noise = new OpenSimplexNoise();
            camera = cam;

            float t = (float)((1 + Math.Sqrt(5.0)) / 2.0);

            Vector3 p0 = new Vector3(-1, t, 0);
            Vector3 p1 = new Vector3(1, t, 0);
            Vector3 p2 = new Vector3(-1, -t, 0);
            Vector3 p3 = new Vector3(1, -t, 0);

            Vector3 p4 = new Vector3(0, -1, t);
            Vector3 p5 = new Vector3(0, 1, t);
            Vector3 p6 = new Vector3(0, -1, -t);
            Vector3 p7 = new Vector3(0, 1, -t);

            Vector3 p8 = new Vector3(t, 0, -1);
            Vector3 p9 = new Vector3(t, 0, 1);
            Vector3 p10 = new Vector3(-t, 0, -1);
            Vector3 p11 = new Vector3(-t, 0, 1);

            p0.Normalize();
            p1.Normalize();
            p2.Normalize();
            p3.Normalize();
            p4.Normalize();
            p5.Normalize();
            p6.Normalize();
            p7.Normalize();
            p8.Normalize();
            p9.Normalize();
            p10.Normalize();
            p11.Normalize();

            Triangles = new List<Triangle>();
            Triangles.Add(new Triangle(p0, p5, p11));
            Triangles.Add(new Triangle(p0, p1, p5));
            Triangles.Add(new Triangle(p0, p7, p1));
            Triangles.Add(new Triangle(p0, p10, p7));
            Triangles.Add(new Triangle(p0, p11, p10));

            Triangles.Add(new Triangle(p1, p9, p5));
            Triangles.Add(new Triangle(p5, p4, p11));
            Triangles.Add(new Triangle(p11, p2, p10));
            Triangles.Add(new Triangle(p10, p6, p7));
            Triangles.Add(new Triangle(p7, p8, p1));

            Triangles.Add(new Triangle(p3, p4, p9));
            Triangles.Add(new Triangle(p3, p2, p4));
            Triangles.Add(new Triangle(p3, p6, p2));
            Triangles.Add(new Triangle(p3, p8, p6));
            Triangles.Add(new Triangle(p3, p9, p8));

            Triangles.Add(new Triangle(p4, p5, p9));
            Triangles.Add(new Triangle(p2, p11, p4));
            Triangles.Add(new Triangle(p6, p10, p2));
            Triangles.Add(new Triangle(p8, p7, p6));
            Triangles.Add(new Triangle(p9, p1, p8));

            for (int i = 0; i < 4; i++)
                SubdivideSphere();

            UpdateVertsToDraw();
            ocean = new Ocean(position, scale * 1.095f, new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1));
            GroundColor = new Vector4((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1);
            CoastColor = new Vector4((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1);

            Tessellation = new Tessellation(cam, Triangles, this);
        }

        public void LoadContent(ContentManager content)
        {
            planetEffect = content.Load<Effect>("Planet");
            ocean.LoadContent(content);
        }

        public void UnloadContent()
        {
            Tessellation.Kill();
        }

        public void SubdivideSphere()
        {
            foreach (Triangle t in Triangles)
                t.Subdivide();
        }

        void UpdateVertsToDraw()
        {
            List<VertexPositionColor> v = new List<VertexPositionColor>();
            foreach (Triangle t in Triangles)
                v.AddRange(t.GetVerticies());
            VerticiesToDraw = v.ToArray();

            //DisplaceVerts();
            VerticiesToDraw = DisplaceVerts(VerticiesToDraw);
        }

        public VertexPositionColor[] DisplaceVerts(VertexPositionColor[] vertsToDisplace)
        {
            VertexPositionColor vert = new VertexPositionColor();
            List<VertexPositionColor> vrts = new List<VertexPositionColor>();

            foreach (VertexPositionColor vp in vertsToDisplace)
            {
                vert = vp;
                vert.Position = Vector3.Transform(vert.Position, Transform);
                vert.Position = Vector3.Transform(vert.Position, Rotation);
                float fDisplace = Terrain(vert.Position);
                Vector3 vDisplace = Vector3.Multiply(vert.Position, fDisplace);
                vert.Position += vDisplace;
                vert.Color = new Color(fDisplace * 4, fDisplace * 4, fDisplace * 4, 1);
                vrts.Add(vert);
            }
            //VerticiesToDraw = vrts.ToArray();
            return vrts.ToArray();
        }

        public void Draw(GraphicsDeviceManager graphics, GameTime gameTime, bool wireframe = false)
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //WIREFRAME DEBUG
            if (wireframe)
            {
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.FillMode = FillMode.WireFrame;
                rasterizerState.CullMode = CullMode.None;
                graphics.GraphicsDevice.RasterizerState = rasterizerState;
            } else
            {
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.FillMode = FillMode.Solid;
                graphics.GraphicsDevice.RasterizerState = rasterizerState;
            }
            //WIREFRAME DEBUG

            planetEffect.Parameters["World"].SetValue(world);
            planetEffect.Parameters["View"].SetValue(camera.View);
            planetEffect.Parameters["Projection"].SetValue(camera.Projection);

            planetEffect.Parameters["LightDirection"].SetValue(new Vector4(1, 0, 0, 1));
            planetEffect.Parameters["LightIntensity"].SetValue(0.5f);

            planetEffect.Parameters["GroundColor"].SetValue(GroundColor);
            planetEffect.Parameters["CoastColor"].SetValue(CoastColor);

            planetEffect.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, VerticiesToDraw, 0, (VerticiesToDraw.Length / 3));

            ocean.Draw(camera, graphics, gameTime);
        }

        float Noise(Vector3 p)
        {
            return (float)(0.5 * noise.Evaluate(p.X, p.Y, p.Z) + 0.5);
        }
        float Terrain(Vector3 p, int steps = 6, float _scale = 36) //Scale = 36
        {
            Vector3 displace = Vector3.Zero;
            for(int i = 0; i < steps; i++)
            {
                displace = new Vector3(
                    Noise(p * _scale + displace),
                    Noise(new Vector3(p.Y, p.Z, p.X) * _scale + displace),
                    Noise(new Vector3(p.Z, p.X, p.Y) * _scale + displace));
                _scale *= 0.5f;
            }
            float e = Noise(p * _scale + displace);

            e = (float)Math.Pow(e, 2);
            //float continent = Noise(p * .2f) > 0.5 ? .1f : /*(float)Math.Pow((Noise(p * .2f) * 2), 3) * .1f*/ 0;
            //float continent = (float)Math.Pow(Noise(p * .2f), 2) * .3f;
            float continent = Noise(p * .2f) * .2f;
            int mask = continent > .1f ? 1 : 0;

            //float e = Noise(p) + .5f * Noise(p * 2) + .25f * Noise(p * 4) * mask;

            return continent + e * mask * .05f;
        }
    }
}
