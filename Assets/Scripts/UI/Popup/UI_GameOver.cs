using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameOver : UI_Popup
{
    
    enum Buttons
    {
        ReTryBtn,
    }

  

    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));


        GetButton((int)Buttons.ReTryBtn).gameObject.AddUIEvent(NewGame);


        //Time.timeScale = 0.0f;
    }

    void NewGame(PointerEventData data)
    {
        ClearDontDestroyOnLoad();
        Managers.Scene.ChangeScene(Define.Scene.LobbyScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
    }

    private void ClearDontDestroyOnLoad()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.scene.name == "DontDestroyOnLoad")
            {
                // 특정 객체를 제외하고 삭제
                if (go.name == "@Pool_root" || go.name == "AudioManager_AudioSource") // Managers는 유지할 경우
                {
                    GameObject.Destroy(go);
                }
            }
        }
    }


    public void ExitGame(PointerEventData action)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
        Time.timeScale = 1.0f;
    }

}
