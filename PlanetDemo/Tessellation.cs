using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlanetDemo
{
    public class Tessellation
    {
        const float HIGH_RESOLUTION = 5f;
        const float MEDIUM_RESOLUTION = 10f;
        const int HIGH_RESOLUTION_LOD = 7;
        const int MEDIUM_RESOLUTION_LOD = 6;
        const int LOW_REOLUTION_LOD = 4;

        Thread TessellationThread;
        bool runThread;

        Camera3D camera;
        List<Triangle> triangles;
        Planet planet;

        public Tessellation(Camera3D c, List<Triangle> t, Planet p)
        {
            camera = c;
            triangles = t;
            planet = p;

            runThread = true;
            TessellationThread = new Thread(new ThreadStart(Work));
            TessellationThread.Start();
        }
        public void Kill()
        {
            runThread = false;
        }
        void Work()
        {
            while (runThread)
            {
                List<Triangle> tri = new List<Triangle>();
                foreach (Triangle t in triangles)
                    tri.AddRange(t.GetLastTriangles());

                for(int i = 0; i < tri.Count; i++)
                {
                    float d = Vector3.Distance(Vector3.Transform(tri[i].Position, planet.Transform), camera.Position);
                    if(d <= HIGH_RESOLUTION)
                    {
                        if (tri[i].levelOfSubdivision < HIGH_RESOLUTION_LOD)
                        {
                            tri[i].Subdivide();
                        }
                    }
                    else if(d <= MEDIUM_RESOLUTION)
                    {
                        if(tri[i].levelOfSubdivision < MEDIUM_RESOLUTION_LOD)
                        {
                            tri[i].Subdivide();
                        }
                        else if(tri[i].levelOfSubdivision > MEDIUM_RESOLUTION_LOD)
                        {
                            tri[i].Parent.Subdivided = false;
                        }
                    }
                    else
                    {
                        if (tri[i].levelOfSubdivision < LOW_REOLUTION_LOD)
                        {
                            tri[i].Subdivide();
                        }
                        else if (tri[i].levelOfSubdivision > LOW_REOLUTION_LOD)
                        {
                            tri[i].Parent.Subdivided = false;
                        }
                    }
                }

                triangles = new List<Triangle>();
                foreach (Triangle t in tri)
                    if (!triangles.Contains(t.GetFirst()))
                        triangles.Add(t.GetFirst());
                List<VertexPositionColor> v = new List<VertexPositionColor>();
                foreach (Triangle t in triangles)
                    v.AddRange(t.GetVerticies());

                planet.VerticiesToDraw = planet.DisplaceVerts(v.ToArray());
            }
        }
    }
}
