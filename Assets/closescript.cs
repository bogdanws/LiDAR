using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class closescript : MonoBehaviour
{

	Keyboard curKeyboard;

	private void Start()
	{
		curKeyboard = Keyboard.current;
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
