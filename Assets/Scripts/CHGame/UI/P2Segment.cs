namespace CHGame
{
    using UnityEngine;
    using Util.Geometry;

    public class P2Segment : MonoBehaviour
    {
        public LineSegment Segment { get; set; }

        public bool belongToPlayer1;
        private P2Controller m_controller;

        private enum EOwnership
        {
            UNOWNED,
            PLAYER1,
            PLAYER2
        }

        public P2Segment(Vector2 a_point1, Vector2 a_point2)
        {
            if (a_point1.x < a_point2.x || (a_point1.x == a_point2.x && a_point1.y < a_point2.y)) {
              Segment = new LineSegment{a_point1, a_point2};
            }
            else {
              Segment = new LineSegment{a_point1, a_point2};
            }

        }

        void Awake()
        {
            //m_gameController = FindObjectOfType<P1AreaGameController>();
        }


    }
}
