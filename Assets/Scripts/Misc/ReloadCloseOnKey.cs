using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReloadCloseOnKey : MonoBehaviour
{

	Keyboard curKeyboard;

	private void Start()
	{
		curKeyboard = Keyboard.current; // Get current keyboard from new input system
	}

	private void Update()
    {
	    if (curKeyboard.escapeKey.isPressed)
	    {
#if !UNITY_WEBGL
		    Application.Quit();
#endif
		    Screen.fullScreen = !Screen.fullScreen;
	    }
	    if (curKeyboard.rKey.isPressed)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

#if UNITY_WEBGL
		if (Input.GetMouseButton(0))
	    {
		    // Get highest resolution supported by the current screen. 
		    var resolution = Screen.resolutions[Screen.resolutions.Length - 1];

		    // The last parameter "true" denotes if it should be fullscreen or not. 
		    Screen.SetResolution(resolution.width, resolution.height, true);
	    }
#endif
	}
}
