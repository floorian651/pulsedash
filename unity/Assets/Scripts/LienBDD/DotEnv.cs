using System;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DotEnv
{
    string GetVariable(string variableName)
    {
        string value = Environment.GetEnvironmentVariable(variableName);

        if (string.IsNullOrEmpty(value))
        {
            Debug.LogWarning($"Variable '{variableName}' vide ou inexistante.");
        }

        return value;
    }

    public static string GetURL()
    {
        DotEnv dotEnv = new DotEnv();
        return dotEnv.GetVariable("API_URL");
    }
}