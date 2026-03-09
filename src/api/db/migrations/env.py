import sys
from logging.config import fileConfig
from os.path import abspath, dirname

from sqlalchemy import engine_from_config, pool
from alembic import context

# 1. Ajout du dossier racine au PYTHONPATH pour qu'Alembic trouve le module 'api'
# On remonte de 3 niveaux depuis api/db/migrations/ pour atteindre le dossier parent de 'api'
sys.path.insert(0, abspath(dirname(dirname(dirname(dirname(__file__))))))

# 2. Import de tes paramètres et de tes modèles
from api.core.config import get_settings
from api.db.models import (
    Base,
)  # L'import de Base ici contient TOUS tes modèles (User, Job, Music, etc.)

# Cet objet permet de configurer le logger d'Alembic
config = context.config
if config.config_file_name is not None:
    fileConfig(config.config_file_name)

# 3. Récupération de l'URL de la DB via ta fonction get_settings()
settings = get_settings()
database_url = (
    settings.DATABASE_URL
)  # Assure-toi que cette variable existe dans ton config.py

# 4. Définition de la metadata pour l'autogenerate (le "cerveau" de la migration)
target_metadata = Base.metadata


def run_migrations_offline() -> None:
    """Exécute les migrations en mode 'offline'."""
    url = database_url
    context.configure(
        url=url,
        target_metadata=target_metadata,
        literal_binds=True,
        dialect_opts={"paramstyle": "named"},
    )

    with context.begin_transaction():
        context.run_migrations()


def run_migrations_online() -> None:
    """Exécute les migrations en mode 'online' (connexion réelle à la DB)."""

    configuration = config.get_section(config.config_ini_section) or {}
    configuration["sqlalchemy.url"] = database_url

    connectable = engine_from_config(
        configuration,
        prefix="sqlalchemy.",
        poolclass=pool.NullPool,
    )

    with connectable.connect() as connection:
        context.configure(connection=connection, target_metadata=target_metadata)

        with context.begin_transaction():
            context.run_migrations()


if context.is_offline_mode():
    run_migrations_offline()
else:
    run_migrations_online()
