using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Extensions
{
    private const string VECTOR_PARSE_PERMITTED_SYMBOLS = "-.,";

    public static T GetRandomElement<T>(this IList<T> list) where T : class
    {
        if (list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }
        else
        {
            Debug.LogError("Tried to get random element from empty list");
            return null;
        }
    }

    public static void DestroyGameObjects<T>(this IList<T> gameObjectList) where T : Component
    {
        int objectCount = gameObjectList.Count;
        for (int i = 0; i < objectCount; i++)
        {
            if (gameObjectList[i])
            {
                Object.Destroy(gameObjectList[i].gameObject);
            }
            else
            {
                Debug.LogError($"Tried to destroy null object {i}");
            }
        }
    }

    public static object OneOf(params object[] objects)
    {
        if (objects.Length > 0)
        {
            return objects[Random.Range(0, objects.Length - 1)];
        }
        else
        {
            Debug.LogError($"Not enough objects in params");
            return null;
        }
    }

    public static Vector3 ToVector3(this string data)
    {
        Vector3 newVector = Vector3.zero;
        string cleanData = string.Empty;
        foreach (char c in data)
        {
            if (char.IsDigit(c) || VECTOR_PARSE_PERMITTED_SYMBOLS.Contains(c))
            {
                cleanData += c;
            }
        }
        string[] axis = cleanData.Split(',');
        if (axis.Length < 3)
        {
            Debug.LogError($"Could not parse Vector '{data}'/'{cleanData}', only {data.Length} numbers parsed");
        }
        else
        {
            try
            {
                newVector = new Vector3(float.Parse(axis[0], CultureInfo.InvariantCulture), float.Parse(axis[1], CultureInfo.InvariantCulture), float.Parse(axis[2], CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception when parsing vector {data}/{cleanData}/[{axis[0]},{axis[1]},{axis[2]}]\n{ex}");
            }
        }
        return newVector;
    }
}