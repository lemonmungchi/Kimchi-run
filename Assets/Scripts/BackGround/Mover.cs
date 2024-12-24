using UnityEngine;

public class BuildingMover : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 2f;

    private void Update()
    {
        // �������� �̵�
        transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);

        // ���� x < -15��� ����(Despawn)
        if (transform.position.x < -15f)
        {
            // GameObject �� GameManager�� Despawn ȣ��
            Managers.Game.Despawn(this.gameObject);
        }
    }
}
