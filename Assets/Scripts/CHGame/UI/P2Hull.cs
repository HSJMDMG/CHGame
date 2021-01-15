namespace CHGame
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry;
    using Util.Geometry.Polygon;
    using Util.Math;

    public static class P2Hull
    {
        public bool mergeChance = true;
        public bool nested = false;
        Polygon2D hull;
        MeshRenderer MeshR;
        private P2Controller m_controller;
        bool selected;
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
              Pos = new Vector2(transform.position.x, transform.position.y);
              m_controller = FindObjectOfType<P2Controller>();

              selected = false;
              MeshR = GetComponent<MeshRenderer>();
              ownership = UNOWNED;
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

            //TODO: render the convex hull with mesh
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
