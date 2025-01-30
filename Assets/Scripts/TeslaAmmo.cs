using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class TeslaAmmo : MonoBehaviour
{
    public float radius = 5f;
    public int damage = 10;
    public float strikeInterval = 1f;
    public Color lightningColor = Color.white;
    public Material lightningMaterial; // Добавьте материал
    public GameObject hitSoundPrefab;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.material = lightningMaterial; // Установите материал
        StartCoroutine(StrikeEnemies());
        Destroy(gameObject, 10f);
    }

    private IEnumerator StrikeEnemies()
    {
        while (true)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, radius)
                                        .Where(c => c.CompareTag("Enemy"))
                                        .ToArray();

            if (enemies.Length > 0)
            {
                Collider closestEnemy = enemies.OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                                               .FirstOrDefault();

                if (closestEnemy != null)
                {
                    StartCoroutine(ShowLightning(closestEnemy.transform.position));
                    DamageManager.DealDamage(closestEnemy, damage);
                    GameObject sound = Instantiate(hitSoundPrefab, transform.position, transform.rotation);
                    Destroy(sound, 5f);
                }
            }

            yield return new WaitForSeconds(strikeInterval + Random.Range(-.1f, .1f));
        }
    }

    private IEnumerator ShowLightning(Vector3 targetPosition)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);
        lineRenderer.startColor = lightningColor;
        lineRenderer.endColor = lightningColor;

        yield return new WaitForSeconds(0.1f);

        lineRenderer.enabled = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject, 1);
        if (!other.collider.CompareTag("Player"))
            DamageManager.DealDamage(other, damage);
    }
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawWireSphere(transform.position, radius);
    // }
}