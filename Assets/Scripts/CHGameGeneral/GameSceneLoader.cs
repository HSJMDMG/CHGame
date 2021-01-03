namespace CHGameGeneral
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GameSceneLoader : MonoBehaviour
	{
		//Load a different scence by name
	    public void LoadScene(string nextSceneName)
	    {
	        SceneManager.LoadScene(nextSceneName);
	    }
	}
}
