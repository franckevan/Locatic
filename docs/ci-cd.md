# CI/CD
## Règles de branche
La branche `main` est protégée (GitHub Rulesets) :- Push direct interdit — tout changement passe par une Pull Request.- Deux checks obligatoires avant merge : `Build & Test` et `Docker Build & Security Scan`.- Historique lisible : chaque changement correspond à une PR avec un message de commit explicite.
## Pull Requests
Chaque fonctionnalité ou correction est développée sur une branche dédiée (ex. `feat/lot-b-infra-k8s-ansible`), poussée
sur GitHub, puis proposée via une Pull Request vers `main`. La PR ne peut être mergée que si les deux checks CI passent.
C'est le mécanisme qui garantit qu'aucun code non testé/non scanné n'atteint `main`.
## Jobs du pipeline (`.github/workflows/ci.yml`)
Le workflow se déclenche sur chaque Pull Request et sur chaque push vers `main`.
### 1. `build-and-test`- Checkout du code- Installation du SDK .NET 9- `dotnet restore`- `dotnet format --verify-no-changes` (lint / contrôle de style)- `dotnet build - `dotnet test` (projet `Locatic.Tests`, xUnit + Moq)- Publication des résultats de tests en artifact
### 2. `docker-build-scan` (dépend de `build-and-test`)- Build de l'image Docker à partir du `Dockerfile` à la racine- Scan de vulnérabilités avec Trivy (`aquasecurity/trivy-action`, **épinglé par SHA de commit** et non par tag — voir note
sécurité ci-dessous)- Échoue si des vulnérabilités CRITICAL/HIGH **avec correctif disponible** sont détectées (`ignore-unfixed: true`)
### 3. `publish-image` (dépend de `docker-build-scan`)- Ne s'exécute **que** sur push vers `main` (jamais sur une Pull Request — la condition `if: github.ref ==
'refs/heads/main' && github.event_name == 'push'` le garantit)- Connexion à `ghcr.io` avec le `GITHUB_TOKEN` intégré (aucun secret à créer manuellement)- Build et push de l'image, taguée `latest` et avec le SHA du commit
## Note sécurité : épinglage de l'action Trivy
En mars 2026, `aquasecurity/trivy-action` a subi une attaque de la chaîne d'approvisionnement (75 tags sur 76 détournés
pendant ~12h pour voler des secrets CI/CD). Par précaution, l'action est référencée par son SHA de commit complet plutôt
que par un tag mutable (`@ed142f...` aurait été un tag piégeable ; on utilise le SHA vérifié via `git ls-remote`). C'est
une pratique recommandée pour toute action tierce sensible.
## Limites du pipeline GitHub
Conformément à la consigne du projet, le pipeline GitHub **s'arrête après la publication de l'image**. Il ne doit pas et
ne peut pas :- exécuter `terraform apply`- lancer le playbook Ansible- déployer sur minikube
Ces trois opérations ciblent la machine locale du développeur (minikube n'est pas accessible depuis les runners GitHub) et
sont décrites dans [deploiement-local.md](deploiement-local.md).
## Gestion des secrets- Le seul secret utilisé est `GITHUB_TOKEN`, généré automatiquement par GitHub Actions pour chaque run (permissions
`packages: write` scopées au job `publish-image`).- Aucun token personnel, mot de passe ou fichier `.env` n'est stocké dans le dépôt.- Les secrets nécessaires au déploiement local (mot de passe Grafana) sont gérés côté Ansible, voir
[monitoring.md](monitoring.md) et [ansible.md](ansible.md).