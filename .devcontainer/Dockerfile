FROM python:3.12-slim
ENV PATH="/usr/local/bin:${PATH}"
WORKDIR /workspaces/wavr

# Installations système utiles
RUN apt-get update && apt-get install -y \
    git \
    build-essential \
    curl \
    ca-certificates \
    eza \
    && rm -rf /var/lib/apt/lists/* \
    && update-ca-certificates

# Install Git LFS
RUN apt-get update && apt-get install -y git-lfs && \
    git lfs install


#Installation des librairies Python
COPY requirements.txt .
RUN pip install -r requirements.txt

# Install act (GitHub Actions runner)
RUN curl -s https://raw.githubusercontent.com/nektos/act/master/install.sh | bash


# Personalisation du terminal (prompt,alias etc.)
RUN echo "alias ls='eza -la'" >> /root/.bashrc

#Starship
RUN curl -sS https://starship.rs/install.sh | sh -s -- -y \
  && mkdir -p /root/.config \
  && starship preset no-nerd-font -o /root/.config/starship.toml

RUN echo 'eval "$(starship init bash)"' >> /root/.bashrc

RUN mkdir -p /root/.config
COPY .config/starship.toml /root/.config/starship.toml

