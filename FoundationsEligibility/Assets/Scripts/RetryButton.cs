using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Is adding a button component automatically when adding this script
[RequireComponent(typeof(Button))]
public class RetryButton : MonoBehaviour
{
	private void Awake()
	{
		// Is listening for the click event on the button component
		GetComponent<Button>().onClick.AddListener(Retry);
	}

	public void Retry()
	{
		// The SceneManager is loading the same scene over
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
