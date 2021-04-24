using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class ReturnButton : MonoBehaviour
{
	public bool IsClicked = false;
	public GameMain Main;
	public GameObject Visual;

	public bool IsEnabled = false;
	public int EnableAt = 5;
	
	private void Update()
	{
		if (!IsEnabled && Main.PlayerRowIndex > EnableAt)
		{
			Visual.SetActive(true);
			IsEnabled = true;
		}
	}

	private void OnMouseDown()
	{
		if (IsClicked)
		{
			return;
		}

		Main.OnReturnButtonClicked();
		
		Visual.SetActive(false);
		IsClicked = true;
	}
}
