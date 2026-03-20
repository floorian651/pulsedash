# Créer une BDD en local:
```shell
docker run --name postgres-test -e POSTGRES_PASSWORD=test -e POSTGRES_USER=test -e POSTGRES_DB=testdb -p 5432:5432 -d postgres
```

# Afficher les services qui tournent
```shell
docker ps
```

# Arêter la BDD
```shell
docker stop postgres-test
```

# Supprimer la BDD du docker:
```shell
docker rm postgres-test
```

# Redémarrer la BDD
```shell
docker start postgres-test
```

# Accéder au terminal de la BDD:
```shell
docker exec -it postgres-test psql -U test -d testdb
```
(quitter avec exit)