namespace CHGame
{
    using UnityEngine;

    public class P1HullPoint : MonoBehaviour

    {
        public Vector2 Pos { get; private set; }

        public Sprite originalPointSprite;
        public Sprite selectedPointSprite;
        public bool selected;
        private SpriteRenderer spriteR;

        private P1AreaGameController m_controller;
        private P1PointsGameController m_controller_point_game;

        void Awake()
        {
            Pos = new Vector2(transform.position.x, transform.position.y);
            m_controller = FindObjectOfType<P1AreaGameController>();
            m_controller_point_game = FindObjectOfType<P1PointsGameController>();
            selected = false;
            spriteR = GetComponent<SpriteRenderer>();
        }

        void Update()
        {

          if (selected)
          {
            spriteR.sprite = selectedPointSprite;
          }
          else
          {
            spriteR.sprite = originalPointSprite;
          }
        }

        void OnMouseDown()
        {
            if (m_controller != null) {
              m_controller.m_pointSelection = true;
            }
            else {
              m_controller_point_game.m_pointSelection = true;
            }

        }

        void OnMouseEnter()
        {

          if (m_controller != null) {
            m_controller.m_pointSelection = true;
            m_controller.m_current_point = this;
          }
          else {
            m_controller_point_game.m_pointSelection = true;
            m_controller_point_game.m_current_point = this;
          }


            //m_controller.m_locked = true;
            //m_controller.m_secondPoint = this;
            //m_controller.m_line.SetPosition(1, Pos);
        }

        void OnMouseExit()
        {
          if (m_controller != null) {
            m_controller.m_pointSelection = false;
          }
          else {
            m_controller_point_game.m_pointSelection = false;
          }



            //m_controller.m_locked = false;
            //m_controller.m_secondPoint = null;
            //var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + 10 * Vector3.forward);
            //m_controller.m_line.SetPosition(1, pos);
        }

    }

}
