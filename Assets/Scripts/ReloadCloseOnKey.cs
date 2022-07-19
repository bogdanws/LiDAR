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
			Application.Quit();
		}

		if (curKeyboard.rKey.isPressed)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
