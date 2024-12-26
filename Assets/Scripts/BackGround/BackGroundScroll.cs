using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroll : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("얼마나 빠르게 지나가게 할지")]
    public List<float> scrollSpeed;

    [Header("References")]
    public List<MeshRenderer> meshRenderer;
    void Start()
    {
        
    }

    
    void Update()
    {
        meshRenderer[0].material.mainTextureOffset += new Vector2(scrollSpeed[0] * Managers.Game.CaculateGameSpeed() / 20 * Time.deltaTime, 0);
        meshRenderer[1].material.mainTextureOffset += new Vector2(scrollSpeed[1] * Managers.Game.CaculateGameSpeed() / 20 * Time.deltaTime, 0);
        meshRenderer[2].material.mainTextureOffset += new Vector2(scrollSpeed[2] * Managers.Game.CaculateGameSpeed() / 20 * Time.deltaTime, 0);
    }
}
