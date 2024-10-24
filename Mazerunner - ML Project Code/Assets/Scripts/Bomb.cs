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
    private int explodeRange = 1;

    [SerializeField] private AudioClip bombExplodeSound;

    private bool hasExploded = false;

    [SerializeField] private GameObject bombModel;

    [System.Obsolete]
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        explodeRange = Object.FindFirstObjectByType<GameManager>().GetExplodeRange();
    }

    void Update()
    {
        bombTimer += Time.deltaTime;
        if (bombTimer >= bombDelay && !hasExploded)
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
        ExplosionUp.GetComponent<Explosion>().setExplosion(Vector3.forward, explosionSpeed, explodeRange);
        
        GameObject ExplosionDown = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionDown.GetComponent<Explosion>().setExplosion(Vector3.back, explosionSpeed, explodeRange);
        
        GameObject ExplosionRight = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionRight.GetComponent<Explosion>().setExplosion(Vector3.right, explosionSpeed, explodeRange);
       
        GameObject ExplosionLeft = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ExplosionLeft.GetComponent<Explosion>().setExplosion(Vector3.left, explosionSpeed, explodeRange);

        // Tell player to decrease bomb counter by 1
        player.BombExploded();

        GetComponent<AudioSource>().PlayOneShot(bombExplodeSound, 0.8f);
        
        // turn off the collider on the bomb and turn off the bomb model
        Destroy(GetComponent<Collider>());
        bombModel.SetActive(false);

        hasExploded = true;

        Destroy(gameObject, 1f);
    }
}
