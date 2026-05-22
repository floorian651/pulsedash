# Jobs

Consultation du statut d'un job de génération de niveau via HTTP (alternative au [WebSocket](../websocket.md)).

**Préfixe :** `/api/v1/jobs`

---

## GET /jobs/{job_id}

Retourne l'état courant d'un job de génération. Si le job est terminé avec succès et que `result_path` est renseigné, une URL de téléchargement présignée du niveau est incluse dans la réponse.

**`GET /api/v1/jobs/{job_id}`** 🔒 *Authentification requise*

!!! warning "Isolation par utilisateur"
    Un utilisateur ne peut consulter que **ses propres jobs**. Toute tentative d'accès au job d'un autre utilisateur retourne `403 Forbidden`.

### Path Parameters

| Paramètre | Type | Description |
|---|---|---|
| `job_id` | string (UUID) | Identifiant unique du job |

### Réponse

| Champ | Type | Description |
|---|---|---|
| `job_id` | string | UUID du job |
| `state` | string | `pending`, `running`, `completed`, `failed` |
| `progress` | integer | Progression de 0 à 100 |
| `result_url` | string ou null | URL présignée du niveau (si `completed`) |
| `error` | string ou null | Message d'erreur (si `failed`) |

=== "Requête"
    ```bash
    curl "https://pulsedashapi.floabd.app/api/v1/jobs/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
      -H "Authorization: Bearer <token>"
    ```

=== "200 OK — En attente"
    ```json
    {
      "job_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "state": "pending",
      "progress": 0,
      "result_url": null,
      "error": null
    }
    ```

=== "200 OK — En cours"
    ```json
    {
      "job_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "state": "running",
      "progress": 65,
      "result_url": null,
      "error": null
    }
    ```

=== "200 OK — Terminé"
    ```json
    {
      "job_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "state": "completed",
      "progress": 100,
      "result_url": "https://storage.exemple.com/levels/...",
      "error": null
    }
    ```

=== "200 OK — Échoué"
    ```json
    {
      "job_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "state": "failed",
      "progress": 40,
      "result_url": null,
      "error": "Librosa failed to analyse audio: unsupported format"
    }
    ```

=== "401 Unauthorized"
    ```json
    { "detail": "Token invalide" }
    ```

=== "403 Forbidden"
    ```json
    { "detail": "Accès refusé" }
    ```

=== "404 Not Found"
    ```json
    { "detail": "Job non trouvé" }
    ```

---

## Stratégie de polling

Si le WebSocket n'est pas disponible dans l'environnement client, un polling HTTP avec backoff exponentiel est recommandé :

```python
import time, requests

def poll_job(job_id: str, token: str, max_attempts: int = 30):
    headers = {"Authorization": f"Bearer {token}"}
    delay = 2.0
    for _ in range(max_attempts):
        r = requests.get(
            f"https://pulsedashapi.floabd.app/api/v1/jobs/{job_id}",
            headers=headers,
        )
        data = r.json()
        state = data["state"]
        print(f"[{state}] progress={data['progress']}%")
        if state in ("completed", "failed"):
            return data
        time.sleep(delay)
        delay = min(delay * 1.5, 10.0)  # backoff exponentiel, max 10s
    raise TimeoutError("Job did not complete in time")
```
