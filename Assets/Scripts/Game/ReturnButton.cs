using Game;
using UnityEngine;

public class ReturnButton : MonoBehaviour
{
	public GameMain Main;

	private void OnMouseDown()
	{
		Main.OnReturnButtonClicked();
		Destroy(gameObject);
	}
}
