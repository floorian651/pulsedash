# Stockage de la musique
## Démarrer les serveurs
```shell
make infra
```

## Interface graphique
Aller sur [localhost:9001](http://localhost:9001) (ou regarder dans l'onglet "Ports" lequel est occupé par le server minio)

Se connecter avec les identifiant MINIO_ROOT_USER et MINIO_ROOT_PASSWORD écrit dans le fichier [.env](../.env)

Sélectionner le bucket "music"

## Interface en ligne de commandes

### Avec mc (non installé dans le container)
Récupérer la musique "seven" du serveur MinIO au répertoire courant
```shell
mc cp local/music/seven.mp3 .
```

Uploader la musique "Darude" du répertoire courant vers le serveur MinIO
```shell
mc cp Darude.mp3 local/music
```

### Avec des requêtes HTTP (nécessite une API, et que le bucket soit publique)
Récupérer la musique "seven" du serveur MinIO au répertoire courant
```shell
curl http://localhost:9001/music/seven.mp3 -o chanson.mp3
```

Uploader la musique "Darude" du répertoire courant vers le serveur MinIO
```shell
curl -X PUT -T Darude.mp3 http://localhost:9001/music/
```