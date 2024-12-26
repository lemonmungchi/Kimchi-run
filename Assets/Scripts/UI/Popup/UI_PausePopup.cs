using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup
{
    enum Images
    {
        Background,
    }

    enum Buttons
    {
        BacktoMainMenuBtn,
        ExitBtn,
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        
        GetImage((int)Images.Background).gameObject.AddUIEvent(ClosePopupUI);
        GetButton((int)Buttons.BacktoMainMenuBtn).gameObject.AddUIEvent(ClosePopupUI);
        GetButton((int)Buttons.ExitBtn).gameObject.AddUIEvent(ExitGame);

        Time.timeScale = 0.0f;     // 일시정지
    }

    public override void ClosePopupUI(PointerEventData action)
    {
        Time.timeScale = 1.0f;
        
        base.ClosePopupUI(action);
    }
    
    void BacktoMainMenu(PointerEventData action)
    {
        // TODO 데이터 저장
        //Managers.Data.SaveData();
        Managers.Scene.ChangeScene(Define.Scene.LobbyScene);
        Time.timeScale = 1.0f;
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
