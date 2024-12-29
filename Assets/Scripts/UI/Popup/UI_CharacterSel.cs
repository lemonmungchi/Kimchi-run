using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CharacterSel : UI_Popup
{
    enum Buttons
    {
        Character1,
        Character2,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Character1).gameObject.AddUIEvent(PlayerA);
        GetButton((int)Buttons.Character2).gameObject.AddUIEvent(PlayerB);
    }

    void PlayerA(PointerEventData data)
    {
        Managers.Pool.Init();
        Managers.Game.Init();
        Managers.Audio.Init();
        Managers.Game.StartPlayer();
        Managers.Scene.ChangeScene(Define.Scene.GameScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
    }

    void PlayerB(PointerEventData data)
    {
        Managers.Pool.Init();
        Managers.Game.Init();
        Managers.Audio.Init();
        Managers.Game.StartPlayerB();
        Managers.Scene.ChangeScene(Define.Scene.GameScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
    }
}
