using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class ReturnButton : MonoBehaviour
{
	public GameMain Main;

	private void OnMouseDown()
	{
		Main.OnReturnButtonClicked();
		gameObject.SetActive(false);
	}
}
