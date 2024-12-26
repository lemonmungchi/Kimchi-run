using UnityEngine;

public class BuildingMover : MonoBehaviour
{
    

    private void Update()
    {
        // �������� �̵�
        transform.Translate(Vector3.left * Managers.Game.CaculateGameSpeed() * Time.deltaTime);

        // ���� x < -15��� ����(Despawn)
        if (transform.position.x < -15f)
        {
            // GameObject �� GameManager�� Despawn ȣ��
            Managers.Game.Despawn(this.gameObject);
        }
    }
}
