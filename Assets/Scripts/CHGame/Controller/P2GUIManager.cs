namespace CHGame
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Manages the GUI for the Voronoi game.
    /// Displays different panels based on turns and updates score text fields.
    /// </summary>
    public class P2GUIManager : MonoBehaviour
    {
        // panels to activate/deactivate
        //public GameObject m_StartPanel;
        public GameObject m_P1Panel;
        public GameObject m_P2Panel;

        // text fields for displaying scores
        public Text m_P1ScoreText;
        public Text m_P2ScoreText;
        public Text m_P1Text;
        public Text m_P2Text;

        //text field for turn
        public Text m_current_turn_number;
        public Text m_maximum_turn_number;

        void Awake()
        {
          m_P1Panel = GameObject.Find("P1TurnPanel");
          m_P2Panel = GameObject.Find("P2TurnPanel");
          m_P1ScoreText = GameObject.Find("P1Score").GetComponent<Text>();
          m_P2ScoreText = GameObject.Find("P2Score").GetComponent<Text>();
          m_P1Text = GameObject.Find("P1Text").GetComponent<Text>();
          m_P2Text = GameObject.Find("P2Text").GetComponent<Text>();
          m_current_turn_number = GameObject.Find("CurrentTurnNumber").GetComponent<Text>();
          m_maximum_turn_number = GameObject.Find("MaximumTurnNumber").GetComponent<Text>();
        }


        /// <summary>
        /// Called when a new turn has started.
        /// </summary>
        /// <param name="m_blueStart"></param>
        public void OnTurnStart(bool m_P1Start)
        {
            m_P1Panel.SetActive(m_P1Start);
            m_P2Panel.SetActive(!m_P1Start);
        }

        /// <summary>
        /// Called when last move has been played.
        /// </summary>
        public void OnLastMove()
        {
            //m_P1Panel.SetActive(false);
            //m_P2Panel.SetActive(true);
            m_P1Text.text = "Game Over";
            m_P2Text.text = "Game Over";
        }

        /// <summary>
        /// Sets the text fields with percentages of the given areas.
        /// </summary>
        /// <param name="a_Player1Area"></param>
        /// <param name="a_Player2Area"></param>
        public void SetPlayerAreaOwned(float a_Player1Area, float a_Player2Area)
        {

            // update text field with percentages
            m_P1ScoreText.text = a_Player1Area.ToString();
            m_P1ScoreText.text = a_Player2Area.ToString();
        }
    }
}
