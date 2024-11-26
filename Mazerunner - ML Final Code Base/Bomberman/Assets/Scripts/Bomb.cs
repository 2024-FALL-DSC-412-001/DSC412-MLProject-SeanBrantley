using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    PlayerController player;
    [SerializeField] private float bombDelay = 2f;
    private float bombTimer = 0f;

    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private float explosionSpeed = 200f;
    [SerializeField] private float explosionRange = 2f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        bombTimer += Time.deltaTime;
        if (bombTimer >= bombDelay)
        {
            Explode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GetComponent<SphereCollider>().isTrigger = false;
        }
    }


    public void Explode()
    {
        GameObject ExplosionUp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionUp.GetComponent<Explosion>().setExplosion(Vector3.forward, explosionSpeed, explosionRange);
        
        GameObject ExplosionDown = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionDown.GetComponent<Explosion>().setExplosion(Vector3.back, explosionSpeed, explosionRange);
        
        GameObject ExplosionRight = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionRight.GetComponent<Explosion>().setExplosion(Vector3.right, explosionSpeed, explosionRange);
        
        GameObject ExplosionLeft = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionLeft.GetComponent<Explosion>().setExplosion(Vector3.left, explosionSpeed, explosionRange);

        // Tell player to decrease bomb counter by 1

        player.BombExploded();
        Destroy(gameObject);
    }
}
