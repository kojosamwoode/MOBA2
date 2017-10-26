using UnityEngine;
using System.Collections;
using System;

public class MenuManager : MonoSingleton<MenuManager> {

	// Use this for initialization

	public Menu CurrentMenu;
	public GameObject loadingScreenObj;
    public Menu[] m_menuList;

	public void Start () {
        m_menuList = GetComponentsInChildren<Menu>(true);
		ShowMenu(CurrentMenu);
	}

	public void LoadingScreens (bool status) {
		loadingScreenObj.SetActive(status);
	}

    public void ShowMenu<T>()
    {
        foreach (Menu menu in m_menuList)
        {
            if (typeof(T) == menu.GetType())
            {
                if (CurrentMenu != null)
                    CurrentMenu.CloseMenu();

                CurrentMenu = menu;
                CurrentMenu.OpenMenu();
            }
        }
      
    }

	public void ShowMenu(Menu menu)
	{
		if (CurrentMenu != null)
			CurrentMenu.CloseMenu();

		CurrentMenu = menu;
		CurrentMenu.OpenMenu();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
