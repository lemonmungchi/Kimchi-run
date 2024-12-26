using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        Managers.Scene.ChangeScene(Define.Scene.LobbyScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
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
