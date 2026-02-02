# --- Configuration ---
COMPOSE = docker compose
BACKEND_SERVICE = backend
DEV_SERVICE = dev

# --- Commandes ---
.PHONY: aide infra infra-stop infra-reset backend celery logs ps clean shell-dev shell-backend

# Cible par défaut
.DEFAULT_GOAL := aide

aide: ## Affiche cette liste d'aide
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | \
	awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-18s\033[0m %s\n", $$1, $$2}'

# --- Infrastructure ---
infra: ## Démarre PostgreSQL, Redis et MinIO
	$(COMPOSE) up -d db redis minio

infra-stop: ## Arrête l'infrastructure (sans supprimer les volumes)
	$(COMPOSE) down

infra-reset: ## Arrête l'infrastructure et supprime les volumes (perte de données)
	$(COMPOSE) down -v

# --- Backend ---
backend: ## Démarre le backend
	$(COMPOSE) up -d $(BACKEND_SERVICE)

celery: ## Démarre le worker Celery
	$(COMPOSE) up -d celery

# --- Outils ---
logs: ## Affiche les logs (ex: make logs backend)
	$(COMPOSE) logs -f $(filter-out $@,$(MAKECMDGOALS))

ps: ## Affiche l'état des conteneurs
	$(COMPOSE) ps

clean: ## Arrête tout, supprime volumes et orphelins
	$(COMPOSE) down -v --remove-orphans

shell-dev: ## Ouvre un shell dans le DevContainer
	$(COMPOSE) exec -it $(DEV_SERVICE) /bin/bash

shell-backend: ## Ouvre un shell dans le backend
	$(COMPOSE) exec -it $(BACKEND_SERVICE) /bin/bash

# Permet les arguments après certaines commandes (logs, etc.)
%:
	@:
