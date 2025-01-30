using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Material outlineMaterial;

    void Start()
    {
        // Создаем копию объекта для обводки
        GameObject outlineObject = Instantiate(gameObject, transform.position, transform.rotation, transform);
        
        // Увеличиваем его размер для создания эффекта обводки
        outlineObject.transform.localScale *= 1.02f;
        
        // Присваиваем материал с обводкой
        Renderer outlineRenderer = outlineObject.GetComponent<Renderer>();
        outlineRenderer.material = outlineMaterial;
        
        // Отключаем тень на обводке, чтобы она не влияла на основную модель
        outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        outlineRenderer.receiveShadows = false;
    }
}
