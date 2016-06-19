using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlanetDemo
{
    public class Triangle
    {
        bool subdivided;
        int levelOfSubdivision;

        Triangle[] triangles;
        VertexPositionColor[] verticies;

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, int los = 0, bool sub = false)
        {
            verticies = new VertexPositionColor[3];
            verticies[0].Position = p1;
            verticies[1].Position = p2;
            verticies[2].Position = p3;

            levelOfSubdivision = los;
            subdivided = sub;
        }

        public void Subdivide()
        {
            if (subdivided)
                foreach (Triangle t in triangles)
                    t.Subdivide();
            else
            {
                triangles = new Triangle[4];

                Vector3 a = MiddlePoint(verticies[0].Position, verticies[1].Position);
                Vector3 b = MiddlePoint(verticies[1].Position, verticies[2].Position);
                Vector3 c = MiddlePoint(verticies[2].Position, verticies[0].Position);

                triangles[0] = new Triangle(verticies[0].Position, a, c, levelOfSubdivision + 1);
                triangles[1] = new Triangle(verticies[1].Position, b, a, levelOfSubdivision + 1);
                triangles[2] = new Triangle(verticies[2].Position, c, b, levelOfSubdivision + 1);
                triangles[3] = new Triangle(a, b, c, levelOfSubdivision + 1);

                subdivided = true;
            }
        }

        Vector3 MiddlePoint(Vector3 p1, Vector3 p2)
        {
            Vector3 ret = new Vector3((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, (p1.Z + p2.Z) / 2);
            ret.Normalize();
            return ret;
        }

        public List<VertexPositionColor> GetVerticies()
        {
            List<VertexPositionColor> output = new List<VertexPositionColor>();

            if (subdivided)
                foreach (Triangle t in triangles)
                    output.AddRange(t.GetVerticies(los));
            else
                foreach (VertexPositionColor v in verticies)
                    output.Add(v);
            return output;
        }
    }
}
