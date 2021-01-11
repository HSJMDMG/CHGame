namespace CHGame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Util.Algorithms.DCEL;
    using Util.Algorithms.Polygon;
    using Util.Algorithms.Triangulation;
    using Util.Geometry.DCEL;
    using Util.Geometry.Polygon;
    using Util.Geometry.Triangulation;
    using Util.Math;
    using Voronoi;

    /// <summary>
    /// Game controller for the voronoi game.
    /// </summary>
    public class P2Controller : MonoBehaviour, IController
    {

        public bool isPlayer1Turn;



        //To load data from ipe
        [SerializeField]
        private List<P1AreaLevel> m_levels;
        protected int m_levelCounter = 0;

        // controller parameters
        public int m_turns;

        // names of differnt victory scenes
        public string m_p1Victory;
        public string m_p2Victory;

        public P2GUIManager m_GUIManager;
        public MeshFilter m_meshFilter;

        // variables defining state of turns

        [SerializeField]
        private int m_maximumTurn;
        private int m_turnCounter;
        private bool player1Turn;

        private float[] m_playerArea;

        private Triangulation m_delaunay;
        private FishManager m_fishManager;
        private Polygon2D m_meshRect;

        [SerializeField]
        private List<P2Point> m_points;
        //m_segments only contains coordinates;
        private HashSet<P2Segment> m_segments;
        private List<GameObject> instantObjects;
        public List<GameObject> P1lineMeshes;
        public List<GameObject> P2LineMeshes;


        private List<P2Point> m_selected_points;
        private List<P2Hull> m_selected_convexhulls;

        internal bool m_pointSelection;
        internal bool m_hullSelection;
        internal P2Point m_current_point;
        internal P2Hull m_current_hull;


        public LineRenderer Player1LineMesh;
        public LineRenderer Player2LineMesh;


        private List<Verctor2> Player1Points;
        private List<P2Hull> Player1Polygons;
        private List<P2Segment> Player1Segements;


        private List<Vector2> Player2Points;
        private List<P2Hull> Player2Polygons;
        private List<P2Segment> Player2Segements;

        // dcel
        // calculated after every turn
        private DCEL m_DCEL;

        // mapping of vertices to ownership enum
        private readonly Dictionary<Vector2, EOwnership> m_ownership = new Dictionary<Vector2, EOwnership>();

        private enum EOwnership
        {
            UNOWNED,
            PLAYER1,
            PLAYER2
        }



        // Use this for initialization
        public void Start()
        {

          m_points = new List<P2Point>();
          m_segments = new HashSet<P2Segment>();
          instantObjects = new List<GameObject>();

          m_selected_points = new List<P2Point>();
          m_rateList = new List<float>();

          //parameter for affine transformation
          epsilon = 0.2f;

          //counter
          m_maximumTurn = 10;
          m_turnCounter = 0;
          player1Turn = true;

          InitLevel();

        }

        private void Update()
        {

            if (Input.GetKeyDown("t"))
            {
                P2Drawer.EdgesOn = !P2Drawer.EdgesOn;
            }

            //add point listener
            if (m_pointSelection && Input.GetMouseButton(0))
            {
              if (m_current_point.selected)
              {
                //change sprite
                m_current_point.selected = false;
                //remove the point from selected point list
                if (m_selected_points.Contains(m_current_point))
                {
                  m_selected_points.Remove(m_current_point);
                }

                m_pointSelection = false;
              }
              else
              {
                if (m_selected_points.Count <  2) {
                  //change sprite
                  m_current_point.selected = true;
                  m_current_point.belongToPlayer1 = player1Turn;
                  //add the point into selected pointlist
                  if (! m_selected_points.Contains(m_current_point))
                  {
                    m_selected_points.Add(m_current_point);
                  }
                  m_pointSelection = false;
                }
              }

            }
            //  else

            // add polygon listener
            if (m_hullSelection && Input.GetMouseButton(0))
            {
              if (m_current_hull.selected)
              {
                //change hull mesh
                m_current_hull.selected = false;
                //remove the point from selected point list
                if (m_selected_hull.Contains(m_current_hull))
                {
                  m_selected_hull.Remove(m_current_hull);
                }

                m_hullSelection = false;
              }
              else
              {
                if (m_selected_hulls.Count <  2) {
                  if ((m_current_hull.belongToPlayer1 == Player1Turn) && (m_current_hull.canMerge)) {
                    //change sprite
                    m_current_hull.selected = true;
                    //add the point into selected pointlist
                    if (! m_selected_hulls.Contains(m_current_hull))
                    {
                      m_selected_hulls.Add(m_current_hull);
                    }
                    m_hullSelection = false;

                  }
                }
              }

            }

        }

        public void InitLevel()
        {
            //import data points from ipe
            // clear old level
            Clear();

            // initialize settlements
            for (var i = 0; i < m_levels[m_levelCounter].Points.Count; i++)  {

              m_levels[m_levelCounter].Points[i] += new Vector2(epsilon * (Random.Range(0f, 1f) - 0.5f), epsilon * (Random.Range(0f, 0.1f)));

              var point = m_levels[m_levelCounter].Points[i];

              var obj = Instantiate(m_pointPrefab, point, Quaternion.identity) as GameObject;
              obj.transform.parent = GameObject.Find("PointCollection").transform;
              instantObjects.Add(obj);

            }

            m_pointSelection = false;

            //Make vertex list
            m_points = FindObjectsOfType<P2Point>().ToList();
        }


        public void showTrapezoidMap()
        {
          // show/hide trapezoid map
          P2Drawer.EdgesOn = !P2Drawer.EdgesOn;
        }

        /// <summary>
        /// Process next turn
        /// </summary>
        public void NextTurn()
        {
          m_turnCounter++;
          player1Turn = !Player1Turn;

          //load victory scene
          if (m_turnCounter > m_maximumTurn)
          {
            if (m_playerArea[0] > m_playerArea[1])
            {
                SceneManager.LoadScene(m_p1Victory);
            }
            else
            {
                SceneManager.LoadScene(m_p2Victory);
            }
          }
          // load victory if screen clicked after every player has taken turn
          else
          {
              // clean selected point
              m_selected_points.Clear();
              m_selected_convexhulls.Clear();


              // update player turn
              player1Turn = !player1Turn;
              m_GUIManager.OnTurnStart(player1Turn);

          }
        }

        public void Connect()
        {

          // check validness (1) 2pts, (2) line segment not exist;
          if (m_selected_points.Count < 2) return;
          var seg1 = new P2Segment(m_selected_points[0], m_selected_points[1], isPlayer1Turn);
          var seg2 = new P2Segment(m_selected_points[0], m_selected_points[1], !isPlayer1Turn);

          if (m_segments.Contains(seg1)) return;
          if (m_segments.Contains(seg2)) return;

          var seg = seg1.Segment;

          //draw line segment, add segment to current Player,
          if (isPlayer1Turn)
          {
            Player1Segements.Add(seg);
            m_segments.Add(seg);

            var P1mesh = Instantiate(m_P1LineMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;

            P1mesh.transform.parent = this.transform;
            P1mesh.GetComponent<P2Segment>().Segment = seg;

            instantObjects.Add(P1mesh);
            P1lineMeshes.Add(P1mesh);
            var P1meshScript = P1mesh.GetComponent<ReshapingMesh>();
            P1meshScript.CreateNewMesh(seg.Point1, seg.Point2);
          }
          else
          {
              Player2Segements.Add(seg);
              m_segments.Add(seg);

              var P2mesh = Instantiate(m_P2LineMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;

              P2mesh.transform.parent = this.transform;
              P2mesh.GetComponent<P2Segment>().Segment = seg;

              instantObjects.Add(P2mesh);
              P2lineMeshes.Add(P2mesh);
              var P2meshScript = P2mesh.GetComponent<ReshapingMesh>();
              P2meshScript.CreateNewMesh(seg.Point1, seg.Point2);
            }


          //TODO: check possible convex hull, add convex hull to current player, update score
          if (isPlayer1Turn)
          {
            if (Player1Points.Contains(seg.Point1) || Player1Points.Contains(seg.Point2))
            {
              CheckPolygon(seg.Point1);
            }
          }
          Player1Points.Add(seg.Point1);
          Player1Points.Add(seg.Point2);

          Player1Points = Player1Points.Distinct().ToList();
          //TODO: check convex hull intersection, update intersection area;


        }

        public void Merge()
        {
          //TODO: check validness (1) 2 polygon, (2) mergechance > 1;
          //TODO: compute new convex hull, remove old 2 convex hulls, add new convex hull to current player, update score
          //TODO: check convex hull intersection, update intersection area;

        }

        private void CheckPolygon(Vector2 startPoint){
          var edges;
          var points;
          if (isPlayer1Turn) {
            edges = Player1Segements;
            points = Player1Points;
          }
          else {
            edges = Player2Segements;
            points = Player2Points;
          }

          //TODO: turn the set into nodelink list
          int [,] edgeList = new int[points.Count, points.Count];

          //TODO: DFS from startPoint to find cycle;


        }

        private void OnRenderObject()
        {
            GL.PushMatrix();

            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            VoronoiDrawer.Draw(m_delaunay);

            GL.PopMatrix();
        }

        /// <summary>
        /// Creates new voronoi and updates mesh and player area.
        /// </summary>
        private void UpdateVoronoi()
        {
            // create voronoi diagram from delaunay triangulation
            m_DCEL = Voronoi.Create(m_delaunay);

            UpdateMesh();
            UpdatePlayerAreaOwned();
        }

        /// <summary>
        /// Updates the mesh according to the Voronoi DCEL.
        /// </summary>
        private void UpdateMesh()
        {
            if (m_meshFilter.mesh == null)
            {
                // create initial mesh
                m_meshFilter.mesh = new Mesh
                {
                    subMeshCount = 2
                };
                m_meshFilter.mesh.MarkDynamic();
            }
            else
            {
                // clear old mesh
                m_meshFilter.mesh.Clear();
                m_meshFilter.mesh.subMeshCount = 2;
            }

            // build vertices and triangle list
            var vertices = new List<Vector3>();
            var triangles = new List<int>[2] {
                new List<int>(),
                new List<int>()
            };

            // iterate over vertices and create triangles accordingly
            foreach (var inputNode in m_delaunay.Vertices)
            {
                // dont draw anything for unowned vertices
                if (m_ownership[inputNode] == EOwnership.UNOWNED) continue;

                // get ownership of node
                var playerIndex = m_ownership[inputNode] == EOwnership.PLAYER1 ? 0 : 1;

                var face = m_DCEL.GetContainingFace(inputNode);

                // cant triangulate outer face
                if (face.IsOuter) continue;

                // triangulate face polygon
                var triangulation = Triangulator.Triangulate(face.Polygon.Outside);

                // add triangles to correct list
                foreach (var triangle in triangulation.Triangles)
                {
                    int curCount = vertices.Count;

                    // add triangle vertices
                    vertices.Add(new Vector3(triangle.P0.x, 0, triangle.P0.y));
                    vertices.Add(new Vector3(triangle.P1.x, 0, triangle.P1.y));
                    vertices.Add(new Vector3(triangle.P2.x, 0, triangle.P2.y));

                    // add triangle to mesh according to owner
                    triangles[playerIndex].Add(curCount);
                    triangles[playerIndex].Add(curCount + 1);
                    triangles[playerIndex].Add(curCount + 2);
                }
            }

            // update mesh
            m_meshFilter.mesh.vertices = vertices.ToArray();
            m_meshFilter.mesh.SetTriangles(triangles[0], 0);
            m_meshFilter.mesh.SetTriangles(triangles[1], 1);
            m_meshFilter.mesh.RecalculateBounds();

            // set correct uv
            var newUVs = new List<Vector2>();
            foreach (var vertex in vertices)
            {
                newUVs.Add(new Vector2(vertex.x, vertex.z));
            }
            m_meshFilter.mesh.uv = newUVs.ToArray();
        }

        /// <summary>
        /// Calculates total area owned by each player separately.
        /// </summary>
        private void UpdatePlayerAreaOwned()
        {
            m_playerArea = new float[2] { 0, 0 };

            foreach (var inputNode in m_delaunay.Vertices)
            {
                // get dcel face containing input node
                var face = m_DCEL.GetContainingFace(inputNode);

                if (m_ownership[inputNode] != EOwnership.UNOWNED)
                {
                    // update player area with face that intersects with window
                    var playerIndex = m_ownership[inputNode] == EOwnership.PLAYER1 ? 0 : 1;
                    m_playerArea[playerIndex] += Intersector.IntersectConvex(m_meshRect, face.Polygon.Outside).Area;
                }
            }

            // update GUI to reflect new player area owned
            m_GUIManager.SetPlayerAreaOwned(m_playerArea[0], m_playerArea[1]);
        }

        /// <summary>
        /// Clears hull and relevant game objects
        /// </summary>
        private void Clear()
        {

            m_points.Clear();
            m_segments.Clear();
            m_selected_points.Clear();
            m_rateList.Clear();

            hintMeshes.Clear();
            lineMeshes.Clear();

            // destroy game objects created in level
            foreach (var obj in instantObjects)
            {
                // destroy immediate
                // since controller will search for existing objects afterwards
                DestroyImmediate(obj);
            }
        }


        public void CheckSolution()
        {}

        public void AdvanceLevel()
        {}

    }
}
