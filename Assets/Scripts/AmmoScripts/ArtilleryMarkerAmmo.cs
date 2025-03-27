using System;
using UnityEngine;

class ArtilleryMarkerAmmo : MonoBehaviour
{
    public GameObject projectile; // Prefab of the projectile
    public int projectileAmount = 5; // Number of projectiles
    public float circleRadius = 5f; // Radius of the circle where projectiles will be positioned
    public float projectileSpeed = 10f; // Speed of projectiles downward
    TankController player;
    Rigidbody rb;

    private bool hasSpawned = false; // Flag to prevent multiple spawns

    private void Start()
    {
        player = FindObjectOfType<TankController>();
        projectile = player.secondaryBulletPrefab;
        rb = GetComponent<Rigidbody>();
        projectileSpeed = player.projectileSpeed / 2;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Spawn projectiles only if not already spawned
        if (!hasSpawned)
        {
            SpawnProjectiles(transform.position);
            hasSpawned = true; // Set the flag to true
            Destroy(gameObject); // Destroy the game object after spawning
        }
    }

    private void SpawnProjectiles(Vector3 spawnPosition)
    {
        // Direction downward
        Vector3 downwardDirection = Vector3.down;

        // Spawn central projectile
        Vector3 centralSpawnPosition = spawnPosition + new Vector3(0, 40, 0); // Offset upward on Y
        Quaternion downwardRotation = Quaternion.LookRotation(downwardDirection); // Rotation downward
        GameObject centralProjectile = Instantiate(projectile, centralSpawnPosition, downwardRotation);
        Rigidbody centralRb = centralProjectile.GetComponent<Rigidbody>();

        // Set speed for the central projectile
        if (centralRb != null)
        {
            centralRb.velocity = downwardDirection * projectileSpeed; // Speed downward
        }
        Destroy(centralProjectile, 10f); // Destroy after 10 seconds

        // Calculate the angle between projectiles
        float angleStep = 360f / projectileAmount;

        // Spawn other projectiles around the circle
        for (int i = 0; i < projectileAmount; i++)
        {
            // Calculate the position of the projectile on the circle
            float angle = i * angleStep * Mathf.Deg2Rad; // Convert degrees to radians
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;

            Vector3 spawnOffset = new Vector3(x, 50 + UnityEngine.Random.Range(0, 20), z); // Offset on the circle (and upward on Y)
            Vector3 circularSpawnPosition = spawnPosition + spawnOffset;

            // Create projectile with downward orientation
            GameObject circularProjectile = Instantiate(projectile, circularSpawnPosition, downwardRotation);
            circularProjectile.layer = 12;
            Rigidbody circularRb = circularProjectile.GetComponent<Rigidbody>();

            // Set speed for the projectile
            if (circularRb != null)
            {
                circularRb.velocity = downwardDirection * projectileSpeed; // Speed downward
            }

            // Destroy the projectile after 10 seconds
            Destroy(circularProjectile, 10f);
        }
    }
}