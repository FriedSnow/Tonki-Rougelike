using UnityEngine;

class BreakableObject : MonoBehaviour
{
    public int hp = 20;
    public GameObject[] walls;
    public GameObject[] dropPool;
    public GameObject destroyParticlesPrefab;
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) Die();
    }
    private void Die()
    {
        if (destroyParticlesPrefab != null)
        {
            GameObject destroyParticles = Instantiate(destroyParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(destroyParticles, 3f);
        }
        
        int rnd = Random.Range(0, 100);
        if (rnd <= 15) Instantiate(dropPool[Random.Range(0, dropPool.Length)], transform.position, Quaternion.identity);

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
            renderer.enabled = false;

        foreach (GameObject wall in walls)
            wall.SetActive(true);
        Destroy(gameObject, 3f);
    }
}