using UnityEngine;
using System;
using DotNetEnv;

public class DotEnv
{
    public string GetVariable(string variableName)
    {
        try
        {
            // Charger les variables d'environnement à partir du fichier .env
            Env.Load();

            // Récupérer la valeur de la variable d'environnement
            string variableValue = Env.GetString(variableName);

            if (string.IsNullOrEmpty(variableValue))
            {
                Debug.LogWarning($"La variable d'environnement '{variableName}' n'est pas définie ou est vide.");
                return null;
            }

            return variableValue;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors de la récupération de la variable d'environnement '{variableName}': {ex.Message}");
            return null;
        }
    }
}
