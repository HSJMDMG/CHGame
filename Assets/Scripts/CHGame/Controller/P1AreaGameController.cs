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

   public class P1AreaGameController : MonoBehaviour, IController
    {
        public LineRenderer m_line;

        [SerializeField]
        private GameObject m_userLineMeshPrefab;
        [SerializeField]
        private GameObject m_hintLineMeshPrefab;
        [SerializeField]
        private GameObject m_pointPrefab;
        [SerializeField]
        private ButtonContainer m_advanceButton;

        [SerializeField]
        private List<P1AreaLevel> m_levels;
        [SerializeField]
        private string m_victoryScene;
        [SerializeField]
        private int m_pointNumberLimit;


        //private bool m_pointSelection;
        internal bool m_pointSelection;
        internal P1HullPoint m_current_point;

        private List<P1HullPoint> m_points;
        private HashSet<LineSegment> m_segments;
        private Polygon2D m_solutionHull;



        private List<float> m_rateList;
        [SerializeField]
        private List<P1HullPoint> m_selected_points;
        //private Polygon2D m_selected_Hull;
        public Polygon2D m_selected_Hull;
        public List<GameObject> hintMeshes;
        public List<GameObject> lineMeshes;


        private List<GameObject> instantObjects;
        private GameObject m_hint;


        protected int m_levelCounter = 0;


        void Start()
        {
            // get unity objects
            m_points = new List<P1HullPoint>();
            m_segments = new HashSet<LineSegment>();
            instantObjects = new List<GameObject>();

            m_selected_points = new List<P1HullPoint>();
            m_rateList = new List<float>();



            InitLevel();
        }

        void Update()
        {

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
                  GameObject.Find("SelectedNumber").GetComponent<Text>().text = m_selected_points.Count.ToString();
                }

                m_pointSelection = false;
              }
              else
              {
                if (m_selected_points.Count <  m_pointNumberLimit) {
                  //change sprite
                  m_current_point.selected = true;
                  //add the point into selected pointlist
                  if (! m_selected_points.Contains(m_current_point))
                  {
                    m_selected_points.Add(m_current_point);
                    GameObject.Find("SelectedNumber").GetComponent<Text>().text = m_selected_points.Count.ToString();
                  }
                  m_pointSelection = false;

                }
              }


              if (m_selected_points.Count >=3 )
              {
                CheckSolution();
              }
              else
              {
                //delete user lines
                foreach (var segmesh in lineMeshes)
                {
                    Destroy(segmesh);
                }
                lineMeshes.Clear();
              }
            }

        }

        public void InitLevel()
        {


            if (m_levelCounter >= m_levels.Count)
            {
                SceneManager.LoadScene(m_victoryScene);
                return;
            }

            // clear old level
            Clear();

            // initialize settlements
            foreach (var point in m_levels[m_levelCounter].Points)
            {
                var obj = Instantiate(m_pointPrefab, point, Quaternion.identity) as GameObject;
                obj.transform.parent = this.transform;
                instantObjects.Add(obj);
            }

            m_pointSelection = false;


            //Make vertex list
            m_points = FindObjectsOfType<P1HullPoint>().ToList();


            // set maximum number of selected m_points
            var convexhulltemp = ConvexHull.ComputeConvexHull(m_points.Select(v => v.Pos));
            m_pointNumberLimit = Random.Range(3, convexhulltemp.VertexCount + 1);
            // set selected number panel
            GameObject.Find("MaximumNumber").GetComponent<Text>().text = m_pointNumberLimit.ToString();
            GameObject.Find("SelectedNumber").GetComponent<Text>().text = "0";


            // compute maximum k-gon (Area)
            if (m_pointNumberLimit >= convexhulltemp.VertexCount)
            {
              m_solutionHull = convexhulltemp;
            }
            else
            {
              m_solutionHull = MaximumKgon.ComputeMaximumAreaKgon(m_points.Select(v => v.Pos), m_pointNumberLimit);
            }


            // set hint button
            m_hint = GameObject.Find("OptimalSolution");
            foreach (var seg in m_solutionHull.Segments)
            {
              //Add a line renderer
              // instantiate new road mesh
              var hintmesh = Instantiate(m_hintLineMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
              hintmesh.transform.parent = m_hint.transform;
              instantObjects.Add(hintmesh);
              hintMeshes.Add(hintmesh);

              hintmesh.GetComponent<P1HullSegment>().Segment =seg;

              var hintmeshScript = hintmesh.GetComponent<ReshapingMesh>();
              hintmeshScript.CreateNewMesh(seg.Point1, seg.Point2);
            }
            m_hint.SetActive(false);

            //set stars information
            GameObject.Find("Area").GetComponent<Text>().text = "0";
            m_rateList = new List<float>() { 0.6f, 0.8f, 0.95f, 1.0f };
            Debug.Log(m_rateList.Count);
            for (int i = 1; i < m_rateList.Count ; i++) {
              var objName = "Star" + i;
              Debug.Log(objName);
              var starArea = m_rateList[i - 1] * m_solutionHull.Area;
              GameObject.Find(objName).GetComponent<Text>().text = starArea.ToString();
            }



            m_advanceButton.Disable();
        }

        public void AdvanceLevel()
        {
            // increase level index
            m_levelCounter++;
            InitLevel();
        }

        public void ShowHideHint()
        {
          if (m_hint.activeSelf)
          {
            m_hint.SetActive(false);
          }
          else
          {
            m_hint.SetActive(true);
          }
        }

        public void CheckSolution()
        {
            var stars = HullRate();
            if (stars == 0) {
              m_advanceButton.Disable();
            }
            else
            {
              m_advanceButton.Enable();
            }


            //TODO: add solution rating information (both UI/Scence) (show a text, make life easier)



          }

        private int HullRate()
        {

            m_selected_Hull = ConvexHull.ComputeConvexHull(m_selected_points.Select(v => v.Pos));

            if (m_selected_Hull.VertexCount < 3)
            {
              return 0;
            }

            //Debug.Log(m_selected_Hull.Segments.Count);

            //Draw new convex hull line segments.
            foreach (var segmesh in lineMeshes)
            {
                Destroy(segmesh);
            }
            lineMeshes.Clear();
            foreach (var seg in m_selected_Hull.Segments)
            {

              //Add a line renderer
              // instantiate new road mesh
              var roadmesh = Instantiate(m_userLineMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
              roadmesh.transform.parent = this.transform;
              instantObjects.Add(roadmesh);
              lineMeshes.Add(roadmesh);

              //roadmesh.GetComponent<P1HullSegment>().Segment =seg;

              var roadmeshScript = roadmesh.GetComponent<ReshapingMesh>();
              roadmeshScript.CreateNewMesh(seg.Point1, seg.Point2);
            }

            //Update current area in information panel
            GameObject.Find("Area").GetComponent<Text>().text = m_selected_Hull.Area.ToString();

            // 60%, 80%, 95% pass level
            for (int i = 0; i < m_rateList.Count; i++) {
              if (m_selected_Hull.Area < m_rateList[i] * m_solutionHull.Area) {
                return i;
              }
            }

            return m_rateList.Count - 1;
        }


        /// <summary>
        /// Clears hull and relevant game objects
        /// </summary>
        private void Clear()
        {
            m_solutionHull = null;
            m_selected_Hull = null;

            m_points.Clear();
            m_segments.Clear();
            m_selected_points.Clear();
            m_rateList.Clear();

            // destroy game objects created in level
            foreach (var obj in instantObjects)
            {
                // destroy immediate
                // since controller will search for existing objects afterwards
                DestroyImmediate(obj);
            }
        }
    }


/*
     public class P1AreaGameController : MonoBehaviour
     {
         public LineRenderer m_line;

         [SerializeField]
         private GameObject m_userLineMeshPrefab;
         [SerializeField]
         private ButtonContainer m_advanceButton;

         internal P1HullPoint m_firstPoint;
         internal P1HullPoint m_secondPoint;
         internal bool m_locked;

         private List<P1HullPoint> m_points;
         private HashSet<LineSegment> m_segments;
         private Polygon2D m_solutionHull;

         void Start()
         {
             // get unity objects
             m_points = FindObjectsOfType<P1HullPoint>().ToList();
             m_segments = new HashSet<LineSegment>();

             // compute convex hull
             m_solutionHull = ConvexHull.ComputeConvexHull(m_points.Select(v => v.Pos));

             // disable advance button
             m_advanceButton.Disable();
         }

         void Update()
         {
             if (m_locked && !Input.GetMouseButton(0))
             {
                 // create road
                 AddSegment(m_firstPoint, m_secondPoint);
             }
             else if (Input.GetMouseButton(0))
             {
                 // update road endpoint
                 var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + 10 * Vector3.forward);
                 m_line.SetPosition(1, pos);
             }

             // clear road creation variables
             if ((m_locked && !Input.GetMouseButton(0)) || Input.GetMouseButtonUp(0))
             {
                 m_locked = false;
                 m_firstPoint = null;
                 m_secondPoint = null;
                 m_line.enabled = false;
             }
         }

         public void AddSegment(P1HullPoint a_point1, P1HullPoint a_point2)
         {
             var segment = new LineSegment(a_point1.Pos, a_point2.Pos);

             // dont add double segments
             if (m_segments.Contains(segment) || m_segments.Contains(new LineSegment(a_point2.Pos, a_point1.Pos)))
                 // also check reverse
                 return;

             m_segments.Add(segment);

             // instantiate new road mesh
             var roadmesh = Instantiate(m_userLineMeshPrefab, Vector3.forward, Quaternion.identity) as GameObject;
             roadmesh.transform.parent = this.transform;

             roadmesh.GetComponent<P1HullSegment>().Segment = segment;

             var roadmeshScript = roadmesh.GetComponent<ReshapingMesh>();
             roadmeshScript.CreateNewMesh(a_point1.transform.position, a_point2.transform.position);

             CheckSolution();
         }

         public void RemoveSegment(P1HullSegment a_segment)
         {
             m_segments.Remove(a_segment.Segment);
             CheckSolution();
         }

         public void CheckSolution()
         {
             if (CheckHull())
             {
                 m_advanceButton.Enable();
             }
             else
             {
                 m_advanceButton.Disable();
             }
         }

         private bool CheckHull()
         {
             // quick return counts not equal
             if (m_solutionHull.Segments.Count != m_segments.Count)
                 return false;

             return m_solutionHull.Segments.All(seg => m_segments.Contains(seg) ||
                         m_segments.Contains(new LineSegment(seg.Point2, seg.Point1)));  // also check reverse
         }
     }
*/

}
