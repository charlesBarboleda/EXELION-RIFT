using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukePowerUp : PowerUp, IPickable
{
    bool _isAttracted;
    public bool isAttracted { get => _isAttracted; set => _isAttracted = value; }
    float _maxSpeed;
    public float maxSpeed { get => _maxSpeed; set => _maxSpeed = value; }
    void FixedUpdate()
    {

        if (isAttracted)
        {
            MoveTowardsPlayer();
        }
    }

    public void OnPickUp()
    {
        Effect();
        gameObject.SetActive(false);
    }


    public void MoveTowardsPlayer()
    {
        Vector2 playerPosition = PlayerManager.GetInstance().transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, _maxSpeed * Time.deltaTime);
    }

    protected override void Effect()
    {
        CameraFollowBehaviour.Instance.ShakePlayerCamera(20f, 2f, 2f);
        GameManager.Instance.DestroyAllShips();
        UIManager.Instance.MidScreenWarningText("Nuke power up!", 1f);
    }

    public override void DeactivateEffect()
    {
        Debug.Log("Nuke PowerUp Deactivated");
    }


}
