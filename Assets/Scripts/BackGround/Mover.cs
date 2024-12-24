using UnityEngine;

public class BuildingMover : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 2f;

    private void Update()
    {
        // 왼쪽으로 이동
        transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);

        // 만약 x < -15라면 제거(Despawn)
        if (transform.position.x < -15f)
        {
            // GameObject → GameManager의 Despawn 호출
            Managers.Game.Despawn(this.gameObject);
        }
    }
}
