using UnityEngine;

public class LaserRFItem : StandardItem
{
    public GameObject itemPrefab;
    public Vector3 spawnPosition = new Vector3(.5f, 5 / 2, 4);

    protected override void ApplyEffect()
    {
        GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);

        Transform turretTransform = player.transform.Find("Turret");

        if (turretTransform != null)
        {
            item.transform.SetParent(turretTransform);
        }
        else
        {
            Debug.LogError("Turret not found in player object!");
        }

        item.transform.localPosition = spawnPosition;
        item.transform.localRotation = Quaternion.identity;
    }
}
