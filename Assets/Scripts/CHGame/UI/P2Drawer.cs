namespace CHGame
{
    using UnityEngine;
    using Util.Geometry;
    using Util.Geometry.Trapezoid;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Util.Algorithms.Triangulation;
    using Util.Geometry.Triangulation;
    using Util.Math;

    /// <summary>
    /// Static class responsible for displaying trapezoid decomposition
    /// </summary>
    public class P2Drawer
    {
        // toggle variables for displaying trapezoid map edges
        public  bool EdgesOn { get; set; }


        // line material for Unity shader
        private  Material m_lineMaterial;

        public void CreateLineMaterial()
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
        /// <param name="m_Trapezoid"></param>
        private  void DrawEdges(List<LineSegment> VerticalLines)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.green);

            Debug.Log("Hey!");
            foreach (var line in VerticalLines)
            {
                // draw line
                GL.Vertex3(line.Point1.x, line.Point1.y, 1);
                GL.Vertex3(line.Point2.x, line.Point2.y, 1);
            }

            GL.End();
        }



        /// <summary>
        /// Main drawing function that calls other auxiliary functions.
        /// </summary>
        /// <param name="m_Trapezoid"></param>
        public  void Draw(List<LineSegment> lines)
        {
            m_lineMaterial.SetPass(0);
            DrawEdges(lines);

            // call functions that are set to true
          //  if (EdgesOn) DrawEdges(m_map);
        }
    }
}
