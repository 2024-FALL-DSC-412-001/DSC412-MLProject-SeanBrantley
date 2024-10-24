using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    enum PowerUps { IncreaseSpeed, IncreaseMaxSupply, IncreaseRange, IncreaseExplosionSpeed}

    [SerializeField] PowerUps powerUpType;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Object.FindFirstObjectByType<PlayerController>().PlayPowerUpPickUpSound();

            switch (powerUpType)
            {
                case PowerUps.IncreaseMaxSupply:
                    {
                        Object.FindFirstObjectByType<GameManager>().IncreaseBombSupply();
                        Destroy(gameObject);
                        break;
                    }
                    case PowerUps.IncreaseRange:
                    {
                        Object.FindFirstObjectByType<GameManager>().IncreaseBombRange();
                        Destroy(gameObject);
                        break;
                    }
                    case PowerUps.IncreaseSpeed:
                    {
                        Object.FindFirstObjectByType<GameManager>().IncreaseSpeed();
                        Destroy(gameObject);
                        break;
                    }
            }
        }
    }
}
