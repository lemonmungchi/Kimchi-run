using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    enum Buttons
    {
        NewGameBtn,
        //ExitGameBtn,
    }

    private PlayerInputActions playerInputActions; // Input Actions 인스턴스

    public override void Init()
    {


        base.Init();


        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.NewGameBtn).gameObject.AddUIEvent(NewGame);
        //GetButton((int)Buttons.ExitGameBtn).gameObject.AddUIEvent(ExitGame);

        // Input Actions 초기화
        playerInputActions = new PlayerInputActions();
        playerInputActions.PlayerAction.Jump.performed += OnSpacebarPressed; // 스페이스바 등록
        playerInputActions.Enable();

    }

    private void OnSpacebarPressed(InputAction.CallbackContext context)
    {
        NewGame(null); // 스페이스바 입력 시 NewGame 실행
    }

    //void LoadGame(PointerEventData data)
    //{
    //    Managers.Scene.ChangeScene(Define.Scene.GameScene);
    //    Managers.Game.thisGameis = Define.ThisGameis.LoadedGame;
    //}

    void NewGame(PointerEventData data)
    {
        Managers.Scene.ChangeScene(Define.Scene.GameScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
    }

    public void ExitGame(PointerEventData data)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    private void OnDestroy()
    {
        // performed 이벤트에서 메서드 해제
        playerInputActions.PlayerAction.Jump.performed -= OnSpacebarPressed;
        playerInputActions.Disable();
    }

}