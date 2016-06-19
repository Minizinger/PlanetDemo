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
    public class Ocean
    {
        public Vector3 Scale;
        public Vector3 Position;

        Color waterColor;

        Matrix world { get { return Matrix.CreateWorld(Position, Vector3.Forward, Vector3.Up); } }
        Matrix transform { get { return Matrix.Multiply(Matrix.CreateTranslation(Position), Matrix.CreateScale(Scale)); } }

        Effect waterEffect;

        List<Triangle> Triangles;
        public VertexPositionColor[] VerticiesToDraw;

        int levelOfSubdivision = 4;

        public Ocean(Vector3 position, Vector3 scale, Color color)
        {
            waterColor = color;

            Position = position;
            Scale = scale;
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
        }

        public void LoadContent(ContentManager content)
        {
            waterEffect = content.Load<Effect>("Water");
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

            DisplaceVerts();
        }

        void DisplaceVerts()
        {
            VertexPositionColor vert = new VertexPositionColor();
            List<VertexPositionColor> vrts = new List<VertexPositionColor>();

            foreach (VertexPositionColor vp in VerticiesToDraw)
            {
                vert = vp;
                vert.Position = Vector3.Transform(vert.Position, transform);
                vrts.Add(vert);
            }
            VerticiesToDraw = vrts.ToArray();
        }

        public void Draw(Camera3D camera, GraphicsDeviceManager graphics, GameTime gameTime)
        {
            waterEffect.Parameters["World"].SetValue(world);
            waterEffect.Parameters["View"].SetValue(camera.View);
            waterEffect.Parameters["Projection"].SetValue(camera.Projection);

            waterEffect.Parameters["LightDirection"].SetValue(new Vector4(1, 0, 0, 1));
            waterEffect.Parameters["LightIntensity"].SetValue(0.5f);

            waterEffect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            waterEffect.Parameters["WaterColor"].SetValue(waterColor.ToVector4());

            waterEffect.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, VerticiesToDraw, 0, (VerticiesToDraw.Length / 3));
        }
    }
}
