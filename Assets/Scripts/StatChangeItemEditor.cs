// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(StatChangeItem))]
// public class StatChangeItemEditor : Editor
// {
//     SerializedProperty canBeMaxed;
//     SerializedProperty maxAmount;

//     private void OnEnable()
//     {
//         // Получаем ссылки на свойства
//         canBeMaxed = serializedObject.FindProperty("canBeMaxed");
//         maxAmount = serializedObject.FindProperty("maxAmount");
//     }

//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();

//         // Рисуем стандартные поля
//         DrawDefaultInspector();

//         // Если canBeMaxed == true, показываем maxAmount
//         if (canBeMaxed.boolValue)
//         {
//             EditorGUILayout.PropertyField(maxAmount, new GUIContent("Max Amount"));
//         }

//         serializedObject.ApplyModifiedProperties();
//     }
// }