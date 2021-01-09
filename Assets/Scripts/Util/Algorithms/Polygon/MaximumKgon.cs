namespace Util.Algorithms.Polygon
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry.Polygon;

    /// <summary>
    /// Collection of algorithms related to convex hulls.
    /// </summary>
    public static class MaximumKgon
    {
        /// <summary>
        /// Does a simple graham scan.
        /// </summary>
        /// <param name="a_points"></param>
        public static Polygon2D ComputeMaximumAreaKgon(IPolygon2D polygon, int pointLimit)
        {
            return ComputeMaximumAreaKgon(polygon.Vertices, pointLimit);
        }

        /// <summary>
        /// Performs a simple graham scan of the given vertices
        /// </summary>
        /// <param name="a_vertices"></param>
        /// <returns></returns>
        public static Polygon2D ComputeMaximumAreaKgon(IEnumerable<Vector2> a_vertices, int pointLimit)
        {
            var oldvertices = a_vertices.Distinct();
            var vertices = oldvertices.ToList();
            vertices.Sort((x, y) => (x.x * y.y - x.y * y.x).CompareTo(0));

            float[,,] f = new float [vertices.Count, vertices.Count, pointLimit];
            int[,,] g = new int [vertices.Count, vertices.Count, pointLimit];

            for (var i = 0; i < vertices.Count; i++) {
              for (var j = 0; j < vertices.Count; j++) {
                for (var k = 0; k < pointLimit; k++) {
                  f[i,j,k] = 0f;
                }
              }
            }

            for (var plimit = 3; plimit <= pointLimit; plimit++)
            {
              var limit = plimit - 3;
              for (var startPoint = 0; startPoint < vertices.Count; startPoint++)
              {
                for (var oldEndPoint = startPoint + 1; oldEndPoint < vertices.Count; oldEndPoint++)
                {
                  for (var newEndPoint = oldEndPoint + 1; newEndPoint < vertices.Count; newEndPoint++) {
                      float total;
                      if (plimit == 3)
                      {
                          total = TriangleArea(vertices[startPoint], vertices[oldEndPoint], vertices[newEndPoint]);
                      }
                      else
                      {
                          total = f[startPoint, oldEndPoint, limit - 1] + TriangleArea(vertices[startPoint], vertices[oldEndPoint], vertices[newEndPoint]);
                      }

                      if (total > f[startPoint, newEndPoint, limit])
                      {
                        f[startPoint, newEndPoint, limit] = total;
                        g[startPoint, newEndPoint, limit] = oldEndPoint;
                      }

                  }
                }
              }
            }

            var optStart = 0;
            var optEnd = 0;
            float optArea = 0;


            for (var startPoint = 0; startPoint < vertices.Count; startPoint++)
            {
                for (var endPoint = startPoint + 2; endPoint < vertices.Count; endPoint++)
                {
                  if (f[startPoint, endPoint, pointLimit - 3] > optArea)
                  {
                    optArea = f[startPoint, endPoint, pointLimit - 3];
                    optStart = startPoint;
                    optEnd = endPoint;
                  }
                }
            }


            Polygon2D m_optimalSolution = new Polygon2D();
            m_optimalSolution.AddVertex(vertices[optStart]);
            m_optimalSolution.AddVertex(vertices[optEnd]);

            for (var cnt = pointLimit - 3; cnt >= 0; cnt--)
            {
              m_optimalSolution.AddVertex(vertices[g[optStart, optEnd, cnt]]);
              optEnd = g[optStart, optEnd, cnt];
            }


            return m_optimalSolution;

        }



        public static float TriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            //float ans = 0.5f * (p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y));
            //if (ans < 0) {Debug.Log("Less than 0!!!!!");}
            return Mathf.Abs(0.5f * (p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)));
        }

    }
}
