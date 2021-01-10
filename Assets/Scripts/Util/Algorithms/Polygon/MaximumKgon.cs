namespace Util.Algorithms.Polygon
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry.Polygon;

    /// <summary>
    /// Collection of algorithms related to k-gon.
    /// </summary>
    public static class MaximumKgon
    {
        /// <summary>
        /// Looking for K-gon with maximum area
        /// </summary>
        /// <param name="a_points"></param>
        public static Polygon2D ComputeMaximumAreaKgon(IPolygon2D polygon, int pointLimit)
        {
            return ComputeMaximumAreaKgon(polygon.Vertices, pointLimit);
        }
        /// <summary>
        /// Looking for K-gon with maximum area
        /// </summary>
        /// <param name="a_points"></param>
        public static Polygon2D ComputeMaximumAreaKgon(IEnumerable<Vector2> a_vertices, int pointLimit)
        {
            var oldvertices = a_vertices.Distinct();
            var vertices = oldvertices.ToList();
            var x0 = vertices[0].x;
            var y0 = vertices[0].y;
            foreach (var vertex in vertices) {
              if (vertex.x < x0) x0 = vertex.x;
              if (vertex.y < y0) y0 = vertex.y;
            }
            vertices.Sort((x, y) => ((x.x - x0) * (y.y - y0) - (x.y - y0) * (y.x - x0)).CompareTo(0));

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


        /// <summary>
        /// Looking for K-gon with maximum point number
        /// </summary>
        /// <param name="a_points"></param>
        public static Polygon2D ComputeMaximumPointsKgon(IPolygon2D polygon, int pointLimit)
        {
            return ComputeMaximumPointsKgon(polygon.Vertices, pointLimit);
        }
        /// <summary>
        /// Looking for K-gon with maximum point number
        /// </summary>
        /// <param name="a_points"></param>
        public static Polygon2D ComputeMaximumPointsKgon(IEnumerable<Vector2> a_vertices, int pointLimit)
        {
            var oldvertices = a_vertices.Distinct();
            var vertices = oldvertices.ToList();
            var x0 = vertices[0].x;
            var y0 = vertices[0].y;
            foreach (var vertex in vertices) {
              if (vertex.x < x0) x0 = vertex.x;
              if (vertex.y < y0) y0 = vertex.y;
            }
            vertices.Sort((x, y) => ((x.x - x0) * (y.y - y0) - (x.y - y0) * (y.x - x0)).CompareTo(0));

            int [,,] f = new int [vertices.Count, vertices.Count, pointLimit];
            int [,,] g = new int [vertices.Count, vertices.Count, pointLimit];

            for (var i = 0; i < vertices.Count; i++) {
              for (var j = 0; j < vertices.Count; j++) {
                for (var k = 0; k < pointLimit; k++) {
                  f[i,j,k] = 0;
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
                      int total = 0;
                      if (plimit == 3)
                      {
                          total = 3 + TrianglePoints(vertices, startPoint, oldEndPoint, newEndPoint);
                      }
                      else
                      {
                          total = f[startPoint, oldEndPoint, limit - 1] +  1 + TrianglePoints(vertices, startPoint, oldEndPoint, newEndPoint);
                      }

                      Debug.Log("Total: "  + total);

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
            int optNumber = 0;


            for (var startPoint = 0; startPoint < vertices.Count; startPoint++)
            {
                for (var endPoint = startPoint + 2; endPoint < vertices.Count; endPoint++)
                {
                  if (f[startPoint, endPoint, pointLimit - 3] > optNumber)
                  {
                    optNumber = f[startPoint, endPoint, pointLimit - 3];
                    optStart = startPoint;
                    optEnd = endPoint;
                  }
                }
            }

            Debug.Log(optNumber);


            Polygon2D m_optimalSolution = new Polygon2D();
            m_optimalSolution.SetPointNumber(optNumber);
            m_optimalSolution.AddVertex(vertices[optStart]);
            m_optimalSolution.AddVertex(vertices[optEnd]);

            for (var cnt = pointLimit - 3; cnt >= 0; cnt--)
            {
              Debug.Log("ggggg:" + TrianglePoints(vertices, optStart, optEnd, g[optStart, optEnd, cnt]));
              m_optimalSolution.AddVertex(vertices[g[optStart, optEnd, cnt]]);
              optEnd = g[optStart, optEnd, cnt];
            }

            Debug.Log("Optimal Vertices:  " + m_optimalSolution.Vertices);

            return m_optimalSolution;
        }


        public static int TrianglePoints(List<Vector2> pts, int Anum, int Bnum, int Cnum)
        {
          int cnt = 0;
          var triangleABC = new Polygon2D();
          triangleABC.AddVertex(pts[Anum]);
          triangleABC.AddVertex(pts[Bnum]);
          triangleABC.AddVertex(pts[Cnum]);

          for (int i = 0; i < pts.Count; i++)
          {
            if ((i == Bnum) || (i == Anum) || (i == Cnum)) {continue;}
            if (triangleABC.ContainsInside(pts[i])) {cnt++;}
          }


          //Debug.Log(cnt);
          triangleABC.Clear();
          return cnt;

        }



    }
}
