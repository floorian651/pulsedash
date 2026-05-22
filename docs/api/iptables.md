# Règles iptables pour l'accès Tailscale à PulseDash

## Vue d'ensemble
Expose les services PostgreSQL et MinIO via le VPN Tailscale grâce à des règles NAT iptables.

> Note : le nom du bridge en production est fixé à `pulsedash-br` et le sous-réseau/IPs sont définis dans [podman-compose.yml](podman-compose.yml). Les adresses ci-dessous sont les ip fixes utilisées pour les règles DNAT Tailscale.

---

## PostgreSQL (Port 5432)

### PREROUTING (Redirection du trafic entrant)
```bash
iptables -t nat -I PREROUTING -i tailscale0 -p tcp --dport 5432 -j DNAT --to 10.89.3.2:5432
```
Redirige le trafic Tailscale entrant sur le port 5432 vers l'IP du conteneur PostgreSQL (10.89.3.2)

### FORWARD (Routage interne)
```bash
iptables -I FORWARD -p tcp -d 10.89.3.2 --dport 5432 -j ACCEPT
```
Autorise le transfert du trafic TCP vers le conteneur PostgreSQL

### POSTROUTING (Chemin de retour)
```bash
iptables -t nat -I POSTROUTING -o tailscale0 -p tcp --sport 5432 -j MASQUERADE
```
Réécrit la source du trafic de retour pour qu'il semble provenir de l'hôte, en repassant par Tailscale

---

## MinIO API S3 (Port 9000)

### PREROUTING
```bash
iptables -t nat -I PREROUTING -i tailscale0 -p tcp --dport 9000 -j DNAT --to 10.89.3.4:9000
```
Redirige le trafic Tailscale sur le port 9000 vers le conteneur MinIO (10.89.3.4)

### FORWARD
```bash
iptables -I FORWARD -p tcp -d 10.89.3.4 --dport 9000 -j ACCEPT
```
Autorise le transfert vers le port de l'API S3 MinIO

### POSTROUTING
```bash
iptables -t nat -I POSTROUTING -o tailscale0 -p tcp --sport 9000 -j MASQUERADE
```
Chemin de retour pour le trafic MinIO S3

---

## MinIO Console (Port 9001)

### PREROUTING
```bash
iptables -t nat -I PREROUTING -i tailscale0 -p tcp --dport 9001 -j DNAT --to 10.89.3.4:9001
```
Redirige le trafic Tailscale sur le port 9001 vers la console MinIO

### FORWARD
```bash
iptables -I FORWARD -p tcp -d 10.89.3.4 --dport 9001 -j ACCEPT
```
Autorise le transfert vers le port de la console MinIO

### POSTROUTING
```bash
iptables -t nat -I POSTROUTING -o tailscale0 -p tcp --sport 9001 -j MASQUERADE
```
Chemin de retour pour le trafic de la console MinIO

---

## Règles UFW (Complémentaires)

```bash
# PostgreSQL
ufw allow 5432/tcp

# API S3 MinIO
ufw allow 9000/tcp

# Console MinIO
ufw allow 9001/tcp
```

---

## Informations réseau des conteneurs

Ces adresses sont les assignations fixes utilisées par le réseau `pulsedash-network`.

| Service    | IP du conteneur | Port       | Réseau             |
|------------|-----------------|------------|--------------------|
| PostgreSQL | 10.89.3.2       | 5432       | pulsedash-network  |
| Redis      | 10.89.3.3       | 6379       | pulsedash-network  |
| MinIO      | 10.89.3.4       | 9000, 9001 | pulsedash-network  |

---

## Persistance

Toutes les règles sont sauvegardées dans :
```bash
/etc/iptables/rules.v4
```

Elles sont restaurées automatiquement au démarrage via le service `netfilter-persistent`.

---

## Tests depuis Tailscale

```bash
# PostgreSQL
nc -zv api-pulsedash 5432
psql -h api-pulsedash -U pulsedash -d pulsedash

# API S3 MinIO
nc -zv api-pulsedash 9000

# Console MinIO
curl http://api-pulsedash:9001
```

---

## Schéma du flux NAT

```
Client Tailscale (100.81.251.100)
         ↓
Interface tailscale0
         ↓
[PREROUTING] DNAT : :5432 → 10.89.1.2:5432
         ↓
[FORWARD] Autorisation du routage vers le conteneur
         ↓
Conteneur PostgreSQL (10.89.3.2:5432)
         ↓
[POSTROUTING] MASQUERADE : retour via tailscale0
         ↓
Le client Tailscale reçoit la réponse
```

---

## Notes de sécurité

**Exposition via Tailscale uniquement** — Aucune exposition directe à Internet  
Accès restreint aux membres du VPN Tailscale  
Trafic chiffré via le tunnel WireGuard de Tailscale
