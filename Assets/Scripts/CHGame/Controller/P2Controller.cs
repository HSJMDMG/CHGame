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

        public bool player1Turn;



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
        private int playerIndex;

        private float[] m_playerArea;

        private Triangulation m_delaunay;
        private FishManager m_fishManager;
        private Polygon2D m_meshRect;

        internal List<GameObject>[] PlayerLineMeshesPrefab; // drag and drop 2 prefabs
        internal List<GameObject>[] PlayerPolygonMeshPrefab; //drag and drop 2 prefabs


        [SerializeField]
        //TODO: UPDATE m-points, m-segments;

        private List<P2Point> m_points;
        //m_segments only contains coordinates;
        private HashSet<P2Segment> m_segments;
        private List<GameObject> instantObjects;

        private List<P2Point> m_selected_points;
        private List<P2Hull> m_selected_convexhulls;

        internal bool m_pointSelection;
        internal bool m_hullSelection;
        internal P2Point m_current_point;
        internal P2Hull m_current_hull;


        public LineRenderer[] PlayerLineMesh;



        private List<P2Hull>[] PlayerPolygons;
        private List<Verctor2>[] PlayerPoints;
        private List<P2Segment>[] PlayerSegments;
        private List<GameObject>[] PlayerPolygonMeshes;
        private int[] PlayerPolygonId;


        private GameObject[] PlayerPolygonMeshCollection;
        internal GameObject[] lineMeshCollection;

        internal GameObject[] PlayerScoreText;
        internal float[] PlayerScore;
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


          //initialize geometry lists for players
          PlayerPolygons = new List<P2Hull>[2] {new List<P2Hull>(), new List<P2Hull>()};
          PlayerPolygonId = new int[2] {0, 0};
          PlayerPoints = new List<Vector2>[2] {new List<Vector2>(), new List<Vector2>()};
          PlayerSegments = new List<P2Segment>[2]{new List<P2Segment>(), new List<P2Segment>()};
          PlayerPolygonMeshes = new List<GameObject>[2] {new List<GameObject>(), new List<GameObject>()};
          //PlayerPolygonMeshPrefab = new GameObject[2];
          PlayerPolygonMeshCollection = new GameObject[2] {GameObject.Find("Player1PolygonCollection"),GameObject.Find("Player2PolygonCollection")};
          lineMeshCollection = new GameObject[2] {GameObject.Find("Player1LineCollection"),GameObject.Find("Player2LineCollection")};

          //initialize score panel
          PlayerScoreText = new GameObject[2] {GameObject.Find("P1Score"), GameObject.Find("P2Score")};
          PlayerScore = new float[2];
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
            // TODO: change m_hullSelection to Point Location on Trapezoidal Decomposition

            if (Input.GetMouseButton(0)) {
              //brute force
              foreach (P2Hull p in PlayerPolygons[playerIndex])
                if (p.hull.Contains(Input.mousePosition)) {
                  m_current_hull = p;

                  if (m_selected_convexhulls.Contains(m_current_hull)) {

                      m_current_hull.selected = false;
                      m_selected_convexhulls.Remove(m_current_hull);
                  }
                  else {
                    if (m_selected_convexhulls.Count < 2) {
                      m_selected_hull.selected = true;
                      m_selected_convexhulls.Add(m_current_hull);

                    }
                  }

                  break;
                }

            }




            /*
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
            */
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
          playerIndex = Player1Turn ? 0 : 1;
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

          // check validness (1) 2pts, (2) line segment not exist; (3) not crossed with existing segments (5) not intersecting existing polygon
          if (m_selected_points.Count < 2) return;
          var seg1 = new P2Segment(m_selected_points[0], m_selected_points[1], player1Turn);
          var seg2 = new P2Segment(m_selected_points[0], m_selected_points[1], !player1Turn);

          if (m_segments.Contains(seg1)) return;
          if (m_segments.Contains(seg2)) return;

          P2Segment seg = seg1;

          foreach (P2Segment s in PlayerSegments[playerIndex])
            if (LineSegment.IntersectProper(s.Segment, seg.Segment) != null) return;

          if (segIntersectPolygon(seg, playerIndex)) return;

          //points on existing polygon cannot be selected.
          //Some explanations: If we can select points on existing polygon, we need to deal with the case: one line segement connecting two polygon
          //If we want to find a largest circle, this is NP-hard problem(Hamilton Cycle)




          //[option]TODO: show info about invalid selection


          //add both vertices and segment to current player
          PlayerPoints[playerIndex].Add(seg.Point1);
          PlayerPoints[playerIndex].Add(seg.Point2);
          PlayerPoints[playerIndex] = PlayerPoints[playerIndex].Distinct().ToList();

          PlayerSegments[playerIndex].Add(seg);
          m_segments.Add(seg);

          //draw line segment,
          var mesh = Instantiate(PlayerLineMeshesPrefab[playerIndex], Vector3.forward, Quaternion.identity) as GameObject;
          mesh.transform.parent = this.transform;
          mesh.GetComponent<P2Segment>() = seg;

          instantObjects.Add(mesh);
          lineMeshCollection[playerIndex].Add(mesh);
          var meshScript = mesh.GetComponent<ReshapingMesh>();
          meshScript.CreateNewMesh(seg.Segment.Point1, seg.Segment.Point2);




          //TODO: add new convex hull (if exist) to current player, draw convex hull, update score

          P2Hull newpolygon = FindPolygon(playerIndex, seg.Point1);

          if (newpolygon != null) {
            PlayerPolygons[playerIndex].Add(nHull);
            UpdateMesh(newpolygon.hull, playerIndex);

            if (newpolygon.hull.VertexCount > 0) {
              PlayerScore[playerIndex] += newpolygon.hull.Area;
            }

            //For the other player, disable the merging for polygon with intersection

            foreach (P2HULL polygon in Player[1 - playerIndex])
            {
              if (Intersector.IntersectConvex(polygon.hull, newpolygon.hull) != null) {
                polygon.mergeChance = false;
              }
            }

            UpdateScore();

          }

        }

        void UpdateScore()
        {
          PlayerScoreText[playerIndex].GetComponent<Text>().text = PlayerScore[playerIndex].ToString();
          PlayerScoreText[1 - playerIndex].GetComponent<Text>().text = PlayerScore[1 - playerIndex].ToString();
        }

        public void Merge()
        {


          //TODO: check validness (1) 2 polygon, (2) mergechance > 1;
          if (m_selected_convexhulls.Count < 2) return;
          foreach (P2Hull hull in m_selected_convexhulls) {
            if (!hull.mergeChance) return;
          }

          //TODO: compute new convex hull,  add new convex hull to current player, update score

          Polygon2D newhull = ConvexHull.CombineConvexHull(m_selected_convexhulls[0], m_selected_convexhulls[1]);

          UpdateMesh(newhull, playerIndex);

          P2Hull newpolygon = new P2Hull();
          newpolygon.hull = newhull;
          newpolygon.mergeChance = false;

          PlayerScore[playerIndex] += newpolygon.hull.Area;

          PlayerPolygons[playerIndex].Add(newpolygon);
          foreach (var v in newpolygon.Vertices) {
            PlayerPoints[playerIndex].Add(v);
          }
          foreach (var seg in newpolygon.Segments) {
            PlayerSegments[playerIndex].Add(new P2Segment(seg.Point1, seg.Point2));
          }


          UpdateScore();
          //TODO: check convex hull intersection, update intersection area;

        }

        bool segIntersectPolygon(P2Hull seg, int playerIndex) {
          foreach (P2Hull p in Player[playerIndex]) {
            if (p.hull.Contains(p1)) return true;
            if (p.hull.Contains(p2)) return true;
          }
          return false;
        }

        bool InPolygon(Vector2D p1, Vector2D p2, int playerIndex) {
          foreach (P2Hull p in Player[playerIndex]) {
            if (p.hull.Contains(p1)) return true;
            if (p.hull.Contains(p2)) return true;
          }
          return false;
        }

        private void FindPolygon(int playerIndex, Vector2 startPoint){


          //turn the set into nodelink list
          var edges = PlayerSegments[playerIndex];
          var points = PlayerPoints[playerIndex];
          int[,] edgeList = new int[points.Count, points.Count];
          int[] edgeNum  = new int[points.Count];

          foreach (P2Segment edge in edges) {
            var seg = edge.Segments;

            int i, j;
            i = points.FindIndex(seg.Point1);
            j = points.FindIndex(seg.Point2);
            edgeList[i][edgeNum[i]++] = j;
            edgeList[j][edgeNum[j]++] = i;
          }

          //DFS from startPoint to find cycle;
          bool[] visited = new bool[points.Count];
          int[] pre =new int[points.Count];
          List<int> cycle = new List<int>();


          for (var i = 0; i < points.Count; i++) {
            visited[i] = false;
            pre[i] = -1;
          }

          int vi = points.FindIndex(startPoint);

          //There will be only one cycle created

          if (DFS(vi, pre, visited, edgeList, edgeNum, cycle)) {
            List<Vector2> CyclePoints;
            CyclePoints = new List<Vector2>();
            foreach (var pointNum in cycle) {
              CyclePoints.Add(points[pointNum]);
            }
            //Copmute the Convex HUll
            Polygon2D newHull= ConvexHull.ComputeConvexHull(CyclePoints);
            P2Hull nhull = new P2Hull();
            nhull.hull = newHull;


            return nhull;
          }
        else {
          return null;
        }

        }

        void UpdateMesh(Polygon2D Hull, int playerIndex) {

          // build vertices and triangle list
          var vertices = new List<Vector3>();
          var triangles = new List<int> ();
          Triangulation triangulation = Triangulator.Triangulate(Hull);

          // add triangles to correct list
          foreach (var triangle in triangulation.Triangles)
          {
              int curCount = vertices.Count;

              // add triangle vertices
              vertices.Add(new Vector3(triangle.P0.x, 0, triangle.P0.y));
              vertices.Add(new Vector3(triangle.P1.x, 0, triangle.P1.y));
              vertices.Add(new Vector3(triangle.P2.x, 0, triangle.P2.y));

              // add triangle to mesh according to owner
              triangles.Add(curCount);
              triangles.Add(curCount + 1);
              triangles.Add(curCount + 2);
          }


          // TODO: set correct uv ???? not sure if this is correct
          var newUVs = new List<Vector2>();
          foreach (var vertex in vertices)
          {
              newUVs.Add(new Vector2(vertex.x, vertex.z));
          }



          Mesh mesh = new Mesh();
          // update mesh
          mesh.vertices = vertices.ToArray();
          mesh.triagnles = triangles;
          mesh.uv = newUVs.ToArray();
          mesh.RecalculateBounds();


          //create new object(new mesh filter) for this polygon

          var newPolygonInstance =
          Instantiate(PlayerPolygonMeshPrefab[playerIndex], PlayerPolygonMeshCollection[playerIndex].transform) as GameObject;

          PlayerPolygonMeshes[playerIndex].Add(newPolygonInstance);

          newPolygonInstance.GetComponent<MeshFilter>().mesh = mesh;

          //TODO: newPolygonInstance.GetComponent<P2Hull>().polygon = Hull;

          //TODO: add selected material
        }

        bool  DFS(Vector2 current_point, ref int[] pre, ref bool[] visited,  int[,] edgeList, int[] edgeNum, ref List<int> cycle) {
          if (visited[current_point]) {

              int v = current_point;
              while (pre[v] != current_point) {
                cycle.Add(v);
                v = pre[v];
              }
            return true;
          }

          for (int i = 0; i > edgeList[edgeNum[current_point]])
          {
            if (!visited[i]) {
              pre[i] = current_point;
              visited[i] = true;
              if (DFS(i))
              {
                return true;
              }
              pre[i] = -1;
              visited[i] = false;
            }
          }

          return false;
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
