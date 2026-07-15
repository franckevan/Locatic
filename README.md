# Locatic — Projet DevOps WEB2

## Objectif

Locatic est une application ASP.NET Core MVC de location de voitures (gestion des marques, modèles, voitures, clients et réservations), réalisée initialement dans le cadre d'un projet de POO. Ce dépôt reprend cette base applicative et y ajoute une chaîne DevOps complète : CI/CD GitHub Actions, conteneurisation Docker, infrastructure locale via Terraform et Ansible, déploiement Kubernetes sur minikube derrière Nginx, et supervision Prometheus/Grafana.

Aucun VPS n'est utilisé : le déploiement final s'exécute entièrement sur la machine locale via minikube.

## Lien avec le projet de POO

L'ensemble du code applicatif (`Locatic.Domain`, `Locatic.Application`, `Locatic.Infrastructure`, `Locatic.Web`) provient du projet de POO Locatic. Les adaptations DevOps ajoutées sur cette base :

- endpoint de santé `/health` (`Locatic.Web/Program.cs`)
- application automatique des migrations EF Core au démarrage (nécessaire pour un volume de stockage vierge, ex. Kubernetes)
- chemin de la base SQLite rendu configurable via la variable d'environnement `ConnectionStrings__DefaultConnection`
- projet de tests automatisés `Locatic.Tests` (xUnit + Moq)

## Prérequis locaux

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [minikube](https://minikube.sigs.k8s.io/) et `kubectl`
- [Terraform](https://developer.hashicorp.com/terraform)
- [Ansible](https://docs.ansible.com/) (via WSL2 sous Windows, non supporté nativement)
- [GitHub CLI](https://cli.github.com/) (optionnel)

## Structure du dépôt

```
.
├── .github/workflows/ci.yml   # Pipeline CI/CD GitHub Actions
├── Locatic/
│   ├── Locatic.sln
│   ├── Dockerfile
│   ├── .dockerignore
│   ├── Locatic.Domain/        # Entités et interfaces
│   ├── Locatic.Application/   # Services et DTOs
│   ├── Locatic.Infrastructure/# Repositories et DbContext
│   ├── Locatic.Web/           # Application MVC
│   └── Locatic.Tests/         # Tests unitaires xUnit + Moq
```
