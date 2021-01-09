namespace CHGame
{
    using UnityEngine;
    using Util.Geometry;

    public class P1HullSegment : MonoBehaviour
    {
        public LineSegment Segment { get; set; }

        private P1AreaGameController m_gameController;

        void Awake()
        {
            m_gameController = FindObjectOfType<P1AreaGameController>();
        }


    }
}
