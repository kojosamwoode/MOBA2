using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public enum GameKey
{
    Select,
    Action, 
    AbilityQ,
    AbilityW, 
    AbilityE,
    AbilityR,
    Stop,
}

public class InputManager : MonoSingleton<InputManager> {

    public delegate void OnTapEvent(Touch touch);
    public event OnTapEvent OnTap;

    public delegate void GetKeyDownEvent(GameKey key);
    public event GetKeyDownEvent OnGetKeyDown;

    private int fingerID = -1;
    private void Awake()
    {
#if !UNITY_EDITOR
     fingerID = 0; 
#endif
    }
    // Update is called once per frame
    void Update () {
        

        if (GameManager.instance.LocalPlayer == null || !GameManager.instance.LocalPlayer || !GameManager.instance.LocalPlayer.CharacterEntity || GameManager.instance.IsGameOver || ClientChat.instance.IsInputFieldOnFocus)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Period))
        {
            GameManager.instance.LocalPlayer.CmdCheatGiveExp();
        }

        if (EventSystem.current.IsPointerOverGameObject(fingerID))    // is the touch on the GUI
        {
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
            // GUI Action
            //Debug.Log("UI Tapped");
            return;
        }


#if (UNITY_IOS || UNITY_ANDROID)

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (OnTap != null)
                {
                    OnTap(touch);
                }
            }

        }

#else
        if (Input.GetButtonDown(GameKey.Select.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.Select);
            }
        }
        if (Input.GetButtonDown(GameKey.Action.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.Action);
            }
        }

        if (Input.GetButtonDown(GameKey.AbilityQ.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.AbilityQ);
            }
        }
        if (Input.GetButtonDown(GameKey.AbilityW.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.AbilityW);
            }
        }
        if (Input.GetButtonDown(GameKey.AbilityE.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.AbilityE);
            }
        }
        if (Input.GetButtonDown(GameKey.AbilityR.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.AbilityR);
            }
        }
        if (Input.GetButtonDown(GameKey.Stop.ToString()))
        {
            if (OnGetKeyDown != null)
            {
                OnGetKeyDown(GameKey.Stop);
            }
        }
#endif




    }


}
