namespace CHGame
{
    using UnityEngine;
    using Util.Geometry.Triangulation;

    /// <summary>
    /// Static class responsible for displaying trapezoid decomposition
    /// </summary>
    public static class TrapezoidDrawer
    {
        // toggle variables for displaying trapezoid map edges
        public static bool EdgesOn { get; set; }


        // line material for Unity shader
        private static Material m_lineMaterial;

        public static void CreateLineMaterial()
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            m_lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            // Turn on alpha blending
            m_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            // Turn backface culling off
            m_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

            // Turn off depth writes
            m_lineMaterial.SetInt("_ZWrite", 0);
        }


        //TODO: change this part to show trapezoid decomposition
        /// <summary>
        /// Draw edges of Trapezoid Decomposition
        /// </summary>
        /// <param name="m_Delaunay"></param>
        private static void DrawEdges(Triangulation m_Delaunay)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.green);

            foreach (var halfEdge in m_Delaunay.Edges)
            {
                // dont draw edges to outer vertices
                if (m_Delaunay.ContainsInitialPoint(halfEdge.T))
                {
                    continue;
                }

                // draw edge
                GL.Vertex3(halfEdge.Point1.x, 0, halfEdge.Point1.y);
                GL.Vertex3(halfEdge.Point2.x, 0, halfEdge.Point2.y);
            }

            GL.End();
        }



        /// <summary>
        /// Main drawing function that calls other auxiliary functions.
        /// </summary>
        /// <param name="m_Delaunay"></param>
        public static void Draw(Triangulation m_Delaunay)
        {
            m_lineMaterial.SetPass(0);

            // call functions that are set to true
            if (EdgesOn) DrawEdges(m_Delaunay);
        }
    }
}
