# --- Configuration ---
COMPOSE = docker compose

# --- Commandes ---
.PHONY: aide logs ps clean \
	shell-backend shell-celery db-shell redis-cli \
	format lint test

.DEFAULT_GOAL := aide

aide: ## Affiche cette liste d'aide
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | \
	awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-22s\033[0m %s\n", $$1, $$2}'

ps: ## Affiche l'état des conteneurs
	$(COMPOSE) ps

logs: ## Affiche les logs (ex: make logs redis)
	$(COMPOSE) logs -f $(filter-out $@,$(MAKECMDGOALS))

# --- Shells ---
shell-celery: ## Ouvre un shell dans Celery (wip)
	$(COMPOSE) exec -it celery /bin/bash

db-shell: ## Ouvre un shell PostgreSQL (ok)
	$(COMPOSE) exec -it db psql -U $$POSTGRES_USER -d $$POSTGRES_DB

redis-cli: ## Ouvre un shell Redis CLI (ok)
	$(COMPOSE) exec -it redis redis-cli

# --- Qualité du code ---
format: ## Formate le code (black + isort) (ok)
	black src
	isort src

lint: ## Analyse statique (ruff) (installer ruff)
	ruff check src

# Permet les arguments après certaines commandes (logs, etc.)
%:
	@:
