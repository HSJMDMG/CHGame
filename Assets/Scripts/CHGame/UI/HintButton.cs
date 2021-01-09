namespace General.Menu
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Show one possible optimal solution
    /// </summary>
    public class HintButton : MonoBehaviour
    {
      
        public void LoadScene(string a_nextScene)
        {
            SceneManager.LoadScene(a_nextScene);
        }
    }
}
