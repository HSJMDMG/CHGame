namespace CHGame
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry;
    using Util.Geometry.Polygon;
    using Util.Math;

    public class P2Hull : MonoBehaviour
    {
        public bool mergeChance;
        public bool nested;
        public Polygon2D hull;
        public MeshRenderer MeshR;
        private P2Controller m_controller;
        public  bool selected;
        public bool belongToPlayer1;

        private enum EOwnership
        {
            UNOWNED,
            PLAYER1,
            PLAYER2
        }


        //TODO: add change mesh listener

        void Awake()
        {
            mergeChance = true;
            nested = false;
            selected = false;

            m_controller = FindObjectOfType<P2Controller>();
            MeshR = GetComponent<MeshRenderer>();

        }

        void Update()
        {

            if (selected)
            {
                MeshR.material = MeshR.materials[0];
            }
            else
            {
                MeshR.material = MeshR.materials[1];
            }

        }

        void OnMouseDown()
        {

            m_controller.m_hullSelection = true;

        }

        void OnMouseEnter()
        {


            m_controller.m_hullSelection = true;
            m_controller.m_current_hull = this;

            //m_controller.m_locked = true;
            //m_controller.m_secondPoint = this;
            //m_controller.m_line.SetPosition(1, Pos);
        }

        void OnMouseExit()
        {

            m_controller.m_hullSelection = false;

            //m_controller.m_locked = false;
            //m_controller.m_secondPoint = null;
            //var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + 10 * Vector3.forward);
            //m_controller.m_line.SetPosition(1, pos);
        }
    }
}