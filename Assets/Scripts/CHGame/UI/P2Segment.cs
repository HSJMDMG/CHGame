namespace CHGame
{
    using UnityEngine;
    using Util.Geometry;


    public class P2Segment : MonoBehaviour
    {
        public LineSegment Segment;

        public bool belongToPlayer1;
        private P2Controller m_controller;

        private enum EOwnership
        {
            UNOWNED,
            PLAYER1,
            PLAYER2
        }

        public void CopySegment(P2Segment seg) 
        {
            Segment = seg.Segment;
            belongToPlayer1 = seg.belongToPlayer1;
        }

        public P2Segment(Vector2 a_point1, Vector2 a_point2, bool P1turn)
        {
            if (a_point1.x < a_point2.x || (a_point1.x == a_point2.x && a_point1.y < a_point2.y)) {
                Segment = new LineSegment(a_point1, a_point2);
                belongToPlayer1 = P1turn;
            }
            else {
                Segment = new LineSegment(a_point2, a_point1);
                belongToPlayer1 = P1turn;
            }

        }

        void Awake()
        {
            m_controller = FindObjectOfType<P2Controller>();
        }


    }
}
