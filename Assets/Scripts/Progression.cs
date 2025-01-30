using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progression : MonoBehaviour
{
    private static List<int> progression;

    private void Start()
    {
        string progressString = PlayerPrefs.GetString("ProgressString");
        if (string.IsNullOrEmpty(progressString) || progressString.Split('-').Length != 10)
        {
            // Initialize progression with default values
            progression = new List<int>(new int[10]);
            SetProgressString();
        }
        else
        {
            // Load progression from PlayerPrefs
            string[] parts = progressString.Split('-');
            progression = new List<int>();
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int val))
                {
                    progression.Add(val);
                }
                else
                {
                    // Handle corrupted data by setting default value
                    progression.Add(0);
                }
            }
        }
        Debug.Log("Progression loaded: " + string.Join("-", progression));
    }

    public static int GetProgression(int position)
    {
        if (position >= 0 && position < progression.Count)
        {
            return progression[position];
        }
        else
        {
            Debug.LogError("Position out of range: " + position);
            return -1;
        }
    }

    public static void SetProgression(int position, int value)
    {
        if (position >= 0 && position < progression.Count)
        {
            progression[position] = value;
            SetProgressString();
        }
        else
        {
            Debug.LogError("Position out of range: " + position);
        }
    }
    public static void IncrementProgression(int position)
    {
        if (position >= 0 && position < progression.Count)
        {
            progression[position] += 1;
            SetProgressString();
        }
        else
        {
            Debug.LogError("Position out of range: " + position);
        }
    }

    private static void SetProgressString()
    {
        string progressString = string.Join("-", progression.ConvertAll(p => p.ToString("D2")));
        PlayerPrefs.SetString("ProgressString", progressString);
    }
}