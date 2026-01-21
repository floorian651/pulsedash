FROM python:3.12-slim-bookworm

ENV PATH="/usr/local/bin:${PATH}"
# Empêche Python d'écrire des fichiers .pyc
ENV PYTHONDONTWRITEBYTECODE=1
# Force le buffer stdout/stderr à être flushé immédiatement
ENV PYTHONUNBUFFERED=1

WORKDIR /workspaces/wavr

# 1. Installation des dépendances système de base + Git LFS + Prérequis .NET
RUN apt-get update && apt-get install -y --no-install-recommends \
    git \
    git-lfs \
    build-essential \
    curl \
    wget \
    ca-certificates \
    openssh-client \
    apt-transport-https \
    software-properties-common \
    && rm -rf /var/lib/apt/lists/* \
    && update-ca-certificates \
    && git lfs install

# 2. Installation de .NET SDK 8.0 (Pour le C# d'Unity)
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

# 3. Installation des librairies Python
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# 4. Install act (GitHub Actions runner)
RUN curl -s https://raw.githubusercontent.com/nektos/act/master/install.sh | bash -s -- -b /usr/local/bin

# 5. Personnalisation du terminal (Starship & Alias)
RUN echo "alias ls='ls -la --color=auto'" >> /root/.bashrc

# Installation de Starship
RUN curl -sS https://starship.rs/install.sh | sh -s -- -y \
  && mkdir -p /root/.config \
  && echo 'eval "$(starship init bash)"' >> /root/.bashrc

# Copie de la config Starship locale si elle existe
COPY .config/starship.toml /root/.config/starship.toml
