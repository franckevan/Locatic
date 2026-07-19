Documentation - architecture.md
docs/architecture.md
# Architecture
## Vue d'ensemble
Locatic part d'une application ASP.NET Core MVC de location de voitures (projet de POO) et y ajoute une chaîne DevOps
complète, entièrement locale (aucun VPS, aucun serveur distant).
```
Développeur
   │  git push / Pull Request
   ▼
GitHub (dépôt central)
   │  déclenche
   ▼
GitHub Actions (CI)
   │  build, tests, lint, scan Trivy
   │  publication de l'image (uniquement sur main)
   ▼
GitHub Container Registry (ghcr.io)
   │
   │  (le pipeline GitHub s'arrête ici)
   │
   ▼
Machine locale du développeur
   │
   ├─ Terraform ──► namespace Kubernetes + PersistentVolumeClaim SQLite
   │
   └─ Ansible ──► rend les manifests Kubernetes (Jinja2) et les applique
                     │
                     ▼
              Cluster minikube
              ┌─────────────────────────────────────────────┐
              │  Nginx (reverse proxy, point d'entrée)       │
              │     │                                        │
              │     ▼                                        │
              │  Application Locatic (ClusterIP, non exposée)│
              │     │                                        │
              │     ▼                                        │
              │  Volume persistant SQLite (PVC)               │
              │                                               │
              │  Prometheus ──scrape──► app, Nginx,          │
              │                          kube-state-metrics   │
              │  Grafana ──lit──► Prometheus                 │
              └─────────────────────────────────────────────┘
```
## Rôle de chaque composant
### GitHub Actions
Exécute les contrôles qualité (build, tests, lint), construit l'image Docker, la scanne (Trivy) et la publie sur `ghcr.io`
— uniquement quand le code est mergé sur `main`. Le pipeline **ne déploie jamais** sur minikube : cette étape doit se
faire depuis la machine locale, minikube n'étant pas accessible depuis les runners GitHub.
### Terraform
Prépare l'infrastructure locale minimale requise avant tout déploiement applicatif : le namespace Kubernetes `locatic` et
le `PersistentVolumeClaim` qui portera la base SQLite. Terraform pilote directement l'API Kubernetes de minikube via le
provider `hashicorp/kubernetes`, en utilisant le kubeconfig local. Voir [terraform.md](terraform.md).
### Ansible
Orchestre la suite du déploiement depuis la machine locale : vérifie les prérequis (minikube démarré, kubectl disponible),
récupère les outputs Terraform, rend les manifests Kubernetes (templates Jinja2) avec les bonnes variables, puis les
applique avec `kubectl apply`. Voir [ansible.md](ansible.md).## Déploiement Kubernetes
Deux Deployments principaux :- **Nginx** : reverse proxy, seul point d'entrée utilisateur (Service `NodePort`).- **Application Locatic** : exposée uniquement en interne (Service `ClusterIP`), jamais accessible directement de
l'extérieur.
L'application monte le PersistentVolumeClaim créé par Terraform sur `/data`, où réside `locatic.db`. Voir
[kubernetes.md](kubernetes.md).
### Nginx (reverse proxy)
Toutes les requêtes utilisateur passent par Nginx, qui relaie vers le service interne de l'application (`proxy_pass`).
L'application n'a pas de Service exposé à l'extérieur du cluster — c'est une contrainte explicite du projet.
### Volume SQLite
L'application utilise SQLite comme unique moyen de persistance (pas de base de données externe). Le chemin de la base est
configurable par variable d'environnement (`ConnectionStrings__DefaultConnection`), ce qui permet de le pointer vers
`/data/locatic.db`, monté sur le PVC. Les migrations Entity Framework Core sont appliquées automatiquement au démarrage de
l'application, ce qui garantit un schéma correct même sur un volume vierge (par exemple lors d'un premier déploiement
Kubernetes).
### Monitoring (Prometheus + Grafana)- **Prometheus** scrape les métriques de l'application (`/metrics`, via `prometheus-net.AspNetCore`), de Nginx (via un
conteneur sidecar `nginx-prometheus-exporter` lisant le `stub_status` interne), et de l'état du cluster
(`kube-state-metrics`, pour les pods et le PVC).- **Grafana** est provisionné automatiquement avec la datasource Prometheus et un dashboard prêt à l'emploi ("Locatic 
Vue d'ensemble") au démarrage, sans configuration manuelle.
Voir [monitoring.md](monitoring.md).
## Pourquoi cette séparation Terraform / Ansible ?
Terraform gère l'état déclaratif de l'infrastructure de base (namespace, stockage) et son propre fichier d'état (non
versionné). Ansible orchestre les étapes procédurales du déploiement applicatif (vérifications, templating, application
des manifests) en s'appuyant sur les outputs Terraform. Cette séparation reflète la consigne du projet : Terraform
prépare, Ansible déploie.