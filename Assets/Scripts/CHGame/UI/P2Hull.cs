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
        Polygon2D hull;

        public Sprite Player1HullMesh;
        public Sprite Player1HullMesh;
        public Sprite Player2HullMesh;
        public bool selected;
        public EOwnership ownership;
        public bool canMerge = true;
        private SpriteRenderer MeshR;
        public bool belongToPlayer1;

        private P2Controller m_controller;

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
              MeshR = GetComponent<SpriteRenderer>();
              ownership = UNOWNED;
          }

          void Update()
          {

            if (selected)
            {
              if (belongToPlayer1)
              {
                spriteR.sprite = Player1HullMesh;
              }
              else
              {
                spriteR.sprite = Player2HullMesh;
              }
              //spriteR.sprite = selectedPointSprite;
            }
            else
            {
              spriteR.sprite = originalHullMesh;
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
