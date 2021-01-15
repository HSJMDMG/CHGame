namespace CHGame
{
    using General.Menu;
    using General.Model;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry.Polygon;
    using Util.Algorithms.Polygon;
    using Util.Geometry;
    using General.Controller;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    using System;
    using Util.Algorithms.DCEL;
    using Util.Algorithms.Triangulation;
    using Util.Geometry.DCEL;
    using Util.Geometry.Triangulation;
    using Util.Math;
    using Voronoi;

    /// <summary>
    /// Game controller for the voronoi game.
    /// </summary>
    public class P2Controller : MonoBehaviour, IController
    {


        //To load data from ipe
        [SerializeField]
        private List<P2Level> m_levels;
        protected int m_levelCounter = 0;



        // names of differnt victory scenes
        public string m_p1Victory;
        public string m_p2Victory;
        public string m_tie;

        public P2GUIManager m_GUIManager;
        //public MeshFilter m_meshFilter;

        // variables defining state of turns

        [SerializeField]
        private int m_maximumTurn;
        [SerializeField]
        private int m_turnCounter;
        [SerializeField]
        private bool player1Turn;
        [SerializeField]
        private int playerIndex;

        private float[] m_playerArea;



        public  GameObject PointPrefab;
        public List<GameObject> PlayerLineMeshesPrefab; // drag and drop 2 prefabs
        public List<GameObject> PlayerPolygonMeshPrefab; //drag and drop 2 prefabs

        //TODO: UPDATE m-points, m-segments;
        [SerializeField]
        private List<P2Point> m_points;

        [SerializeField]
        //m_segments only contains coordinates;
        private HashSet<LineSegment> m_segments;
        private List<GameObject> instantObjects;
        private float epsilon;

        [SerializeField]
        private List<P2Point> m_selected_points;
        [SerializeField]
        private List<P2Hull> m_selected_convexhulls;

        internal bool m_pointSelection;
        internal bool m_hullSelection;
        internal P2Point m_current_point;
        internal P2Hull m_current_hull;


        public LineRenderer[] PlayerLineMesh;


        public int m_totoalPointNum;

        private List<Polygon2D>[] PlayerPolygons;
        private List<GameObject>[] PlayerPolygonObjectes;
        [SerializeField]
        public List<Vector2>[] PlayerPoints;
        [SerializeField]
        private List<P2Segment>[] PlayerSegments;
        private List<GameObject>[] PlayerPolygonMeshes;
        private List<GameObject>[] PlayerLineMeshes;
        private int[] PlayerPolygonId;


        private GameObject[] PlayerPolygonMeshCollection; // 2 empty gameobject to hold polygon meshs
        private GameObject[] LineMeshCollection; //2 empty gameobject to hold line meshs



        private GameObject[] PlayerScoreText;

        [SerializeField]
        private GameObject[] PlayerTurnPanel;

        internal float[] PlayerScore;

        private bool operated;
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
            m_segments = new HashSet<LineSegment>();
          instantObjects = new List<GameObject>();

          m_selected_points = new List<P2Point>();
          m_selected_convexhulls = new List<P2Hull>();

            //parameter for affine transformation
            epsilon = 0.2f;

          //counter
          m_maximumTurn = 10;
          m_turnCounter = 0;
          player1Turn = true;


            operated = false;


        //initialize geometry lists for players
        PlayerPolygons = new List<P2Hull>[2] {new List<P2Hull>(), new List<P2Hull>()};
          PlayerPolygonId = new int[2] {0, 0};
          PlayerPoints = new List<Vector2>[2] {new List<Vector2>(), new List<Vector2>()};
          PlayerSegments = new List<P2Segment>[2]{new List<P2Segment>(), new List<P2Segment>()};
          PlayerPolygonMeshes = new List<GameObject>[2] {new List<GameObject>(), new List<GameObject>()};
          PlayerLineMeshes = new List<GameObject>[2] { new List<GameObject>(), new List<GameObject>() };


            //PlayerPolygonMeshPrefab = new GameObject[2];


        PlayerPolygonMeshCollection = new GameObject[2] {GameObject.Find("Player1PolygonCollection"),GameObject.Find("Player2PolygonCollection")};
        LineMeshCollection = new GameObject[2] {GameObject.Find("Player1LineCollection"),GameObject.Find("Player2LineCollection")};

          //initialize score panel
          PlayerScoreText = new GameObject[2] {GameObject.Find("P1Score"), GameObject.Find("P2Score")};
          PlayerScore = new float[2] { 0f, 0f};

            //initialize turn panel
            PlayerTurnPanel = new GameObject[2] { GameObject.Find("P1TurnPanel"), GameObject.Find("P2TurnPanel")};

            InitLevel();

        }

        private void Update()
        {
         /* Show Trapezoidal Decomposition
          *
          * if (Input.GetKeyDown("t"))
            {
                P2Drawer.EdgesOn = !P2Drawer.EdgesOn;
            }
            */


            //add point listener
            if (!operated) {
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
                        if (m_selected_points.Count < 2)
                        {
                            //change sprite
                            m_current_point.selected = true;
                            m_current_point.belongToPlayer1 = player1Turn;
                            //add the point into selected pointlist
                            if (!m_selected_points.Contains(m_current_point))
                            {
                                m_selected_points.Add(m_current_point);
                            }
                            m_pointSelection = false;
                        }
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
                      m_current_hull.selected = true;
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

              m_levels[m_levelCounter].Points[i] += new Vector2(epsilon * (UnityEngine.Random.Range(0f, 1f) - 0.5f), epsilon * (UnityEngine.Random.Range(0f, 0.1f)));

              var point = m_levels[m_levelCounter].Points[i];

              var obj = Instantiate(PointPrefab, point, Quaternion.identity) as GameObject;
              obj.transform.parent = GameObject.Find("PointCollection").transform;
              instantObjects.Add(obj);

            }

            m_pointSelection = false;


            m_maximumTurn = 10;
            m_turnCounter = 0;
            player1Turn = true;

            m_totoalPointNum = m_levels[m_levelCounter].Points.Count;

            GameObject.Find("CurrentTurnNumber").GetComponent<Text>().text = m_turnCounter.ToString();
            GameObject.Find("MaximumTurnNumber").GetComponent<Text>().text = m_maximumTurn.ToString();

            PlayerTurnPanel[0].SetActive(true);
            PlayerTurnPanel[1].SetActive(false);

            PlayerScore[0] = 0f;
            PlayerScore[1] = 1f;


        //Make vertex list
        //m_points = FindObjectsOfType<P2Point>().ToList();
    }


        public void ShowHideTrapezoidMap()
        {
          // show/hide trapezoid map
        //  P2Drawer.EdgesOn = !P2Drawer.EdgesOn;
        }



        /// <summary>
        /// Process next turn
        /// </summary>
        public void NextTurn()
        {
          m_turnCounter++;
            operated = false;

            //Debug.Log(player1Turn);

            player1Turn = !player1Turn;
            //Debug.Log(player1Turn);

          playerIndex = player1Turn ? 0 : 1;


            PlayerTurnPanel[playerIndex].SetActive(true);
            PlayerTurnPanel[1 - playerIndex].SetActive(false);



          //load victory scene
          if (m_turnCounter > m_maximumTurn)
          {
            if (PlayerScore[0] > PlayerScore[1])
            {
                SceneManager.LoadScene(m_p1Victory);
            }
            else
            {
                    if (PlayerScore[0] == PlayerScore[1]) {
                        SceneManager.LoadScene(m_tie);
                    }
                    else {
                        SceneManager.LoadScene(m_p2Victory);
                    }

            }
          }
          // load victory if screen clicked after every player has taken turn
          else
          {
              // clean selected point
              foreach (P2Point point in m_selected_points) {
                    point.selected = false;
              }

              foreach (P2Hull hull in m_selected_convexhulls)
                {
                    hull.selected = false;
                }

                m_selected_points.Clear();
              m_selected_convexhulls.Clear();


              // update player turn

              m_GUIManager.OnTurnStart(player1Turn);
                GameObject.Find("CurrentTurnNumber").GetComponent<Text>().text = m_turnCounter.ToString();
                GameObject.Find("MaximumTurnNumber").GetComponent<Text>().text = m_maximumTurn.ToString();


            }


        }

        public void Connect()
        {

            // check validness (1) 2pts, (2) line segment not exist; (3) not crossed with existing segments (5) not intersecting existing polygon


            if (m_selected_points.Count < 2) return;
          var seg = new P2Segment(m_selected_points[0].Pos, m_selected_points[1].Pos, player1Turn);
            var segLS = new LineSegment(m_selected_points[0].Pos, m_selected_points[1].Pos);


            //Debug.Log(seg.Segment.Point1 + "," + seg.Segment.Point2);


            if (m_segments.Contains(segLS)) return;



          foreach (P2Segment s in PlayerSegments[playerIndex])
            if (LineSegment.IntersectProper(s.Segment, seg.Segment) != null) return;

          if (SegIntersectPolygon(seg, playerIndex)) return;

            //points on existing polygon cannot be selected.
            //Some explanations: If we can select points on existing polygon, we need to deal with the case: one line segement connecting two polygon
            //If we want to find a largest circle, this is NP-hard problem(Hamilton Cycle)




            //[option]TODO: show info about invalid selection
            operated = true;

          //add both vertices and segment to current player
          PlayerPoints[playerIndex].Add(seg.Segment.Point1);
          PlayerPoints[playerIndex].Add(seg.Segment.Point2);
          PlayerPoints[playerIndex] = PlayerPoints[playerIndex].Distinct().ToList();

          PlayerSegments[playerIndex].Add(seg);
          m_segments.Add(segLS);

          //draw line segment,
          var mesh = Instantiate(PlayerLineMeshesPrefab[playerIndex], Vector3.forward, Quaternion.identity) as GameObject;
          mesh.transform.parent = LineMeshCollection[playerIndex].transform;
          mesh.GetComponent<P2Segment>().CopySegment(seg);

          instantObjects.Add(mesh);
          PlayerLineMeshes[playerIndex].Add(mesh);

          var meshScript = mesh.GetComponent<ReshapingMesh>();
          meshScript.CreateNewMesh(seg.Segment.Point1, seg.Segment.Point2);




          //TODO: add new convex hull (if exist) to current player, draw convex hull, update score

          P2Hull newpolygon = FindPolygon(playerIndex, seg.Segment.Point1);

          if (newpolygon != null) {

                Debug.Log("Miao?!!!");
            PlayerPolygons[playerIndex].Add(newpolygon);
            UpdateMesh(newpolygon.hull, playerIndex);

            if (newpolygon.hull.VertexCount > 0) {
              PlayerScore[playerIndex] += newpolygon.hull.Area;
            }

            //For the other player, disable the merging for polygon with intersection

            foreach (P2Hull polygon in PlayerPolygons[1 - playerIndex])
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

            operated = true;
          Polygon2D newhull = ConvexHull.ComputeConvexHull(m_selected_convexhulls[0].hull, m_selected_convexhulls[1].hull);

          UpdateMesh(newhull, playerIndex);

            P2Hull newpolygon = new P2Hull
            {
                hull = newhull,
                mergeChance = false
            };

            PlayerScore[playerIndex] += newpolygon.hull.Area;

          PlayerPolygons[playerIndex].Add(newpolygon);
          foreach (var v in newpolygon.hull.Vertices) {
            PlayerPoints[playerIndex].Add(v);
          }
          foreach (var seg in newpolygon.hull.Segments) {
            PlayerSegments[playerIndex].Add(new P2Segment(seg.Point1, seg.Point2, player1Turn));
          }


          UpdateScore();


        }

        bool SegIntersectPolygon(P2Segment seg, int playerIndex) {
          foreach (P2Hull p in PlayerPolygons[playerIndex]) {
            if (p.hull.Contains(seg.Segment.Point1)) return true;
            if (p.hull.Contains(seg.Segment.Point2)) return true;
          }
          return false;
        }

        bool InPolygon(Vector2 p1, Vector2 p2, int playerIndex) {
          foreach (P2Hull p in PlayerPolygons[playerIndex]) {
            if (p.hull.Contains(p1)) return true;
            if (p.hull.Contains(p2)) return true;
          }
          return false;
        }

        private P2Hull FindPolygon(int playerIndex, Vector2 startPoint){


          var edges = PlayerSegments[playerIndex];
          var points = PlayerPoints[playerIndex];


            //Debug.Log(edges.Count + " , "+ points.Count);

            if (points.Count < 3) return null;


            //turn the egdge set into nodelink list
            int[,] edgeList = new int[points.Count, points.Count];
          int[] edgeNum  = new int[points.Count];

          foreach (P2Segment edge in edges) {
            int i,j;
            i = points.IndexOf(edge.Segment.Point1);
            j = points.IndexOf(edge.Segment.Point2);
            edgeList[i, edgeNum[i]++] = j;
            edgeList[j, edgeNum[j]++] = i;
          }

          //DFS from startPoint to find cycle;
          bool[] visited = new bool[m_totoalPointNum];
          int[] pre =new int[m_totoalPointNum];
          bool[,] edgeUsed = new bool[m_totoalPointNum, m_totoalPointNum];

          List<int> cycle = new List<int>();


          for (var i = 0; i < m_totoalPointNum; i++) {
            visited[i] = false;
            pre[i] = -1;
                for (int j = 0; j < m_totoalPointNum; j++) {
                    edgeUsed[i, j] = false;
                }
            }

          int vi = points.IndexOf(startPoint);

          //There will be only one cycle created

          if (DFS(points.IndexOf(startPoint), points.IndexOf(startPoint), ref pre,ref visited, ref edgeUsed, edgeList, edgeNum,ref cycle)) {
                Debug.Log("Cycle!!");
            List<Vector2> CyclePoints;
            CyclePoints = new List<Vector2>();


                //Debug.Log(cycle.Count);

            foreach (var pointNum in cycle) {
              CyclePoints.Add(points[pointNum]);
            }


            //Copmute the Convex HUll
            Polygon2D newHull= ConvexHull.ComputeConvexHull(CyclePoints);
                //Debug.Log("HullVNum: " + newHull.VertexCount);
            P2Hull nhull = new P2Hull
            {
                hull = newHull
            };

            return nhull;
          }
            else
            {
          return null;
        }

        }

        private void UpdateMesh(Polygon2D Hull, int playerIndex) {

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
          mesh.triangles = triangles.ToArray();
          mesh.uv = newUVs.ToArray();
          mesh.RecalculateBounds();


          //create new object(new mesh filter) for this polygon

          var newPolygonInstance =
          Instantiate(PlayerPolygonMeshPrefab[playerIndex], PlayerPolygonMeshCollection[playerIndex].transform) as GameObject;



        newPolygonInstance.transform.parent = PlayerPolygonMeshCollection[playerIndex].transform;
          PlayerPolygonMeshes[playerIndex].Add(newPolygonInstance);

          newPolygonInstance.GetComponent<MeshFilter>().mesh = mesh;

          //TODO: newPolygonInstance.GetComponent<P2Hull>().polygon = Hull;

          //TODO: add selected material
        }

        bool  DFS(int current_point, int start_point, ref int[] pre, ref bool[] visited,  ref bool[,] edgeUsed, int[,] edgeList, int[] edgeNum, ref List<int> cycle) {

            Debug.Log("current_point:" + current_point);
            Debug.Log("edgeNum[current_point]" + edgeNum[current_point]);

            if (visited[current_point]) {
                int i = current_point;
                while ((pre[i] >= 0)  && (pre[i] != start_point)) {
                    //Debug.Log(i);
                    cycle.Add(i);
                    i = pre[i];
                }
                cycle.Add(i);

                if (pre[i] < 0)
                {
                    cycle.Clear();
                    return false;
                }
                else
                {
                    //Debug.Log("find!!!");
                    return true;
                }
            }

            visited[current_point] = true;

            for (int num = 0; num < edgeNum[current_point]; num++)
            {
                int i = edgeList[current_point, num];

                if (!edgeUsed[current_point, i])
                {
                    pre[i] = current_point;
                    //visited[i] = true;
                    edgeUsed[current_point, i] = true;
                    edgeUsed[i, current_point] = true;


                    if (DFS(i, start_point, ref pre, ref visited, ref edgeUsed, edgeList, edgeNum, ref cycle))
                    {
                        return true;
                    }

                    edgeUsed[current_point, i] = false;
                    edgeUsed[i, current_point] = false;
                    pre[i] = -1;
                    //visited[i] = false;
                }
            }


            visited[current_point] = false;
          return false;
        }





        /*

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
         */
        /// <summary>
        /// Clears hull and relevant game objects
        /// </summary>
        private void Clear()
        {

            m_selected_points.Clear();
            m_selected_convexhulls.Clear();


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
