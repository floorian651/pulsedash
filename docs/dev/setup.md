# Configuration de l'environnement Wawe

Guide complet pour mettre en place votre environnement de développement.

## Prérequis

Avant de commencer, assurez-vous que vous disposez de :

- **Git** (pour le contrôle de version)
- **Docker Desktop** (pour les conteneurs)
- **VS Code** (recommandé pour l'expérience Dev Container optimale)
- **Extension Dev Containers** pour VS Code

## Étape 1 : Installation de Git

Git est essentiel pour cloner et gérer le projet.

### Windows

1. Téléchargez l'installateur depuis [git-scm.com](https://git-scm.com/download/win)
2. Exécutez l'installateur et suivez les instructions par défaut
3. Ouvrez Git Bash et vérifiez l'installation :

```bash
git --version
```

### Linux (Ubuntu/Debian)

```bash
sudo apt update
sudo apt install git
git --version
```

### macOS

```bash
# Via Homebrew
brew install git
```

## Configuration SSH (optionnel mais recommandé)

Pour cloner via SSH sans entrer votre mot de passe à chaque fois :

### Générer une clé SSH

```bash
ssh-keygen -t ed25519 -C "votre.email@exemple.com"
# Appuyez sur Entrée pour accepter le chemin par défaut
# Optionnel : entrez une passphrase
```

### Ajouter la clé à GitHub

1. Copiez votre clé publique :

```bash
# Linux/macOS
cat ~/.ssh/id_ed25519.pub

# Windows (Git Bash)
cat ~/.ssh/id_ed25519.pub
```

2. Allez sur [GitHub Settings > SSH Keys](https://github.com/settings/keys)
3. Cliquez sur "New SSH key"
4. Collez votre clé publique et sauvegardez

### Tester la connexion

```bash
ssh -T git@github.com
# Vous devez voir : "Hi username! You've successfully authenticated..."
```

## Étape 2 : Installation de Docker Desktop

Docker est essentiel pour exécuter l'environnement de développement en conteneur.

### Windows

1. Téléchargez [Docker Desktop pour Windows](https://www.docker.com/products/docker-desktop)
2. Exécutez l'installateur
3. Redémarrez votre ordinateur après l'installation
4. Ouvrez PowerShell ou Command Prompt et vérifiez :

```powershell
docker --version
docker run hello-world
```

### Linux (Ubuntu/Debian)

```bash
# Installer Docker
sudo apt update
sudo apt install -y docker.io

# Installer Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Vérifier
docker --version
docker-compose --version

# Ajouter votre utilisateur au groupe docker (optionnel, pour éviter sudo)
sudo usermod -aG docker $USER
# Reconnectez-vous pour que les changements prennent effet
```

### macOS

1. Téléchargez [Docker Desktop pour Mac](https://www.docker.com/products/docker-desktop)
2. Ouvrez le fichier `.dmg` et glissez Docker dans le dossier Applications
3. Lancez Docker depuis Applications
4. Vérifiez l'installation :

```bash
docker --version
docker run hello-world
```

## Étape 3 : Installation de VS Code et Dev Containers

### Installer VS Code

Téléchargez depuis [code.visualstudio.com](https://code.visualstudio.com/)

### Installer l'extension Dev Containers

1. Ouvrez VS Code
2. Appuyez sur `Ctrl+Shift+X` (ou `Cmd+Shift+X` sur macOS) pour ouvrir les extensions
3. Cherchez **"Dev Containers"**
4. Cliquez sur installer (édition officielle par Microsoft)

Ou installez directement :
```bash
code --install-extension ms-vscode-remote.remote-containers
```

## Étape 4 : Cloner le projet

### Via HTTPS

```bash
git clone https://github.com/floorian651/wavr.git
cd wavr
```

### Via SSH

```bash
git clone git@github.com:floorian651/wavr.git
cd wavr
```

## Étape 5 : Ouvrir dans le Dev Container

### Avec VS Code (recommandé)

1. Ouvrez le dossier `wavr` dans VS Code
2. Une notification devrait apparaître : "Folder contains a Dev Container configuration"
3. Cliquez sur **"Reopen in Container"**
4. Attendez que Docker construise l'image (cela peut prendre quelques minutes la première fois)

Ou utilisez la palette de commandes :
- Appuyez sur `Ctrl+Shift+P` (ou `Cmd+Shift+P` sur macOS)
- Tapez "Remote-Containers: Reopen in Container"
- Appuyez sur Entrée

### En ligne de commande

```bash
# Une fois dans le dossier du projet, utiliser make par exemple :
make infra ..
make aide
```

## Vérification de l'installation

Une fois que le Dev Container est prêt, vérifiez que tout fonctionne :

```bash
# Vérifier Python
python --version

# Vérifier pip
pip --version

# Vérifier .NET
dotnet --version

# Tester l'installation des dépendances
pip list | grep -E "librosa|numpy|fastapi"
```

## Dépannage

### "Cannot connect to Docker daemon"

**Solution** : Assurez-vous que Docker Desktop est en cours d'exécution

### "Permission denied while trying to connect to Docker daemon" (Linux)

**Solution** : Ajoutez votre utilisateur au groupe docker
```bash
sudo usermod -aG docker $USER
# Reconnectez-vous
```

### Espace disque insuffisant

**Solution** : Libérez de l'espace
```bash
docker system prune -a
# Soyez prudent, cela supprime les images inutilisées
```

## Besoin d'aide ?

Consultez la documentation complète du projet sur [wavr.io](https://floorian651.github.io/wavr/)
