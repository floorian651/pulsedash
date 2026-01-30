FROM mcr.microsoft.com/devcontainers/python:3.12-bookworm

ENV PATH="/usr/local/bin:${PATH}"
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1
ENV LANG=en_US.UTF-8
ENV LC_ALL=en_US.UTF-8

WORKDIR /app
COPY requirements.txt /tmp/requirements.txt
RUN pip install --no-cache-dir -r /tmp/requirements.txt

WORKDIR /workspaces/wavr

#Fix pour enlever le repo yarn qui fait planter le dl des images docker-compose (pitié)
RUN rm -f /etc/apt/sources.list.d/yarn.list

# 1. Installation des dépendances système + Git LFS
RUN apt-get update && apt-get install -y --no-install-recommends \
    make \
    gawk \
    less \
    locales \
    bash-completion \
    git \
    git-lfs \
    build-essential \
    curl \
    procps \
    wget \
    ca-certificates \
    openssh-client \
    apt-transport-https \
    software-properties-common \
    && rm -rf /var/lib/apt/lists/* \
    && update-ca-certificates \
    && git lfs install

# 2. Configuration des locales
RUN echo "en_US.UTF-8 UTF-8" > /etc/locale.gen && \
    locale-gen en_US.UTF-8

# 4. Installation de Starship
RUN curl -sS https://starship.rs/install.sh | sh -s -- -y

# 5. Installation de ble.sh
RUN git clone --recursive --depth 1 --shallow-submodules https://github.com/akinomyoga/ble.sh.git /tmp/ble.sh && \
    make -C /tmp/ble.sh install PREFIX=/usr/local && \
    rm -rf /tmp/ble.sh

# 6. Configuration du shell (ble.sh puis Starship)
RUN echo '# Fix variable USER pour ble.sh' >> /etc/bash.bashrc && \
    echo '[ -z "$USER" ] && export USER=$(id -un)' >> /etc/bash.bashrc && \
    echo '# Activer bash-completion' >> /etc/bash.bashrc && \
    echo '[ -f /usr/share/bash-completion/bash_completion ] && source /usr/share/bash-completion/bash_completion' >> /etc/bash.bashrc && \
    echo '# Activer ble.sh' >> /etc/bash.bashrc && \
    echo 'source /usr/local/share/blesh/ble.sh' >> /etc/bash.bashrc && \
    echo '# Activer Starship' >> /etc/bash.bashrc && \
    echo 'eval "$(starship init bash)"' >> /etc/bash.bashrc && \
    echo "alias ls='ls -la --color=auto'" >> /etc/bash.bashrc

# 7. Configuration Starship
RUN mkdir -p /root/.config
COPY .config/starship.toml /root/.config/starship.toml

# 8. Installation des librairies Python
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# 9. Installation de act (GitHub Actions runner)
RUN curl -s https://raw.githubusercontent.com/nektos/act/master/install.sh | bash -s -- -b /usr/local/bin
