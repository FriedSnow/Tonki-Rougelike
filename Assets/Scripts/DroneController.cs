using UnityEngine;

public class DroneController : MonoBehaviour
{
    public string enemyTag = "Enemy"; // Тег противника
    public float viewRadius = 10f; // Радиус видимости
    public float fieldOfViewAngle = 60f; // Угол обзора дрона (в градусах)
    public float shootingDelayMin = 1f; // Минимальная задержка перед выстрелом
    public float shootingDelayMax = 3f; // Максимальная задержка перед выстрелом
    public int projectileSpeed = 100;
    public Transform firePoint; // Точка, из которой происходит выстрел
    public GameObject bulletPrefab; // Префаб пули

    private Transform target; // Текущая цель
    private float nextShotTime; // Время следующего выстрела

    void Update()
    {
        FindClosestEnemy();
        if (target != null)
        {
            RotateTowardsTarget(target);
            ShootAtTarget();
        }
    }

    void FindClosestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewRadius);

        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag(enemyTag)) // Проверяем тег
            {
                Transform potentialTarget = collider.transform;
                Vector3 directionToTarget = potentialTarget.position - transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                // Проверяем, находится ли цель в поле зрения
                if (IsInFieldOfView(potentialTarget) && distanceToTarget < shortestDistance)
                {
                    closestEnemy = potentialTarget;
                    shortestDistance = distanceToTarget;
                }
            }
        }

        target = closestEnemy;
    }

    bool IsInFieldOfView(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        return angle <= fieldOfViewAngle * 0.5f;
    }

    void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 50f); // Плавный поворот
    }

    void ShootAtTarget()
    {
        if (Time.time >= nextShotTime)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = firePoint.forward * projectileSpeed;
            nextShotTime = Time.time + Random.Range(shootingDelayMin, shootingDelayMax);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Отображение радиуса видимости
        Gizmos.color = Color.green;
        

        // Отображение конуса поля зрения
        Vector3 forward = transform.forward * viewRadius;
        Vector3 right = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.DrawLine(transform.position, transform.position + left);
    }
}