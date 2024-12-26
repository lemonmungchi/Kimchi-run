using UnityEngine;

public class BuildingMover : MonoBehaviour
{
    

    private void Update()
    {
        // 왼쪽으로 이동
        transform.Translate(Vector3.left * Managers.Game.CaculateGameSpeed() * Time.deltaTime);

        // 만약 x < -15라면 제거(Despawn)
        if (transform.position.x < -15f)
        {
            // GameObject → GameManager의 Despawn 호출
            Managers.Game.Despawn(this.gameObject);
        }
    }
}
