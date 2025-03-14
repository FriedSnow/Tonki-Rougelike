using UnityEngine;

public class TankDamage : MonoBehaviour
{
    public static void SpawnHitParticles(GameObject gearPrefab, Transform tankTransform, int numberOfGears = 5)
    {
        for (int i = 0; i < numberOfGears; i++)
        {
            GameObject gear = Instantiate(gearPrefab, tankTransform.position, Quaternion.identity);

            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
            randomDirection.Normalize();

            float randomSpeed = Random.Range(10, 100);

            Rigidbody rb = gear.GetComponent<Rigidbody>();
            rb.AddForce(randomDirection * randomSpeed, ForceMode.VelocityChange);

            gear.transform.Rotate(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f)));

            Destroy(gear, 1f);
        }
    }
    public static void SpawnHitParticles(GameObject particlePrefab, Transform spawnPosiion, int numberOfParticles, float particleForce)
    {
        if (particlePrefab != null)
            for (int i = 0; i < numberOfParticles; i++)
            {
                GameObject damageParticle = Instantiate(particlePrefab, spawnPosiion.position, Quaternion.identity);

                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
                randomDirection.Normalize();

                Rigidbody rb = damageParticle.GetComponent<Rigidbody>();
                rb.AddForce(randomDirection * particleForce * Random.Range(1, 3), ForceMode.VelocityChange);

                damageParticle.transform.Rotate(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f)));

                Destroy(damageParticle, 3f);
            }
    }
}