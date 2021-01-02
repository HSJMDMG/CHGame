namespace CHGame
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Simple class encapsulating load scene method.
    /// </summary>
    public class GameSceneLoader : MonoBehaviour
	{
	    public void LoadScene(string a_nextScene)
	    {
	        SceneManager.LoadScene(a_nextScene);
	    }
	}
}






