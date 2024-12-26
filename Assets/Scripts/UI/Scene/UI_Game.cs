using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Game : UI_Scene
{
    [SerializeField] private Sprite OnHeart;
    [SerializeField] private Sprite OffHeart;

    enum Images
    {
        Heart1,
        OffHeart1,
        Heart2,
        OffHeart2,
        Heart3,
        OffHeart3,
    }

    enum Buttons
    {
        EscapeButton,
    }

    enum Text
    {
        Score,
    }
    
    public override void Init()
    {
        base.Init(); // 상위 클래스의 초기화 메서드 호출

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Text));

        GetButton((int)Buttons.EscapeButton).gameObject.AddUIEvent(PauseOrResume);

        // Player의 OnHealthChanged 이벤트 구독
        var player = Managers.Game.GetPlayer().GetComponent<Player>();
        player.OnHealthChanged += UpdateHearts;

        Managers.Game.OnScoreChanged += UpdateScore;
        Managers.Game.OnHighScoreChanged += UpdateHighScore;

        for (int i = 0; i < 3; i++)
        {
            // Heart와 OffHeart의 GameObject를 활성화/비활성화
            GetImage((int)Images.Heart1 + i * 2).gameObject.SetActive(i < player.CurrentLives);   // 활성화된 하트
            GetImage((int)Images.OffHeart1 + i * 2).gameObject.SetActive(i >= player.CurrentLives); // 비활성화된 하트
        }

    }

    void PauseOrResume(PointerEventData eventData)
    {
        // 1. 뭐든지 열려있으면 다 닫기
        // 2. 아무것도 없으면 열기

        if (Managers.UI.GetStackSize() > 0)
            Managers.UI.CloseAllPopupUI();
        else
            Managers.UI.ShowPopupUI<UI_PausePopup>();
    }

    /// <summary>
    /// 플레이어 목숨에 따라 하트 UI 업데이트
    /// </summary>
    /// <param name="currentLives">현재 목숨 수</param>
    private void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < 3; i++)
        {
            // Heart와 OffHeart의 GameObject를 활성화/비활성화
            GetImage((int)Images.Heart1 + i * 2).gameObject.SetActive(i < currentLives);   // 활성화된 하트
            GetImage((int)Images.OffHeart1 + i * 2).gameObject.SetActive(i >= currentLives); // 비활성화된 하트
        }
    }

    private void UpdateScore(int score)
    {
        GetText((int)Text.Score).text = "Score : " + score;
    }

    private void UpdateHighScore(int score)
    {
        GetText((int)Text.Score).text = "HighScore : " + score;
    }

}


