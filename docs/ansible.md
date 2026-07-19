# Ansible
## Rôle
Ansible orchestre le déploiement applicatif complet sur minikube, en s'appuyant sur l'infrastructure préparée par
Terraform. Il est exécuté **depuis la machine locale**, jamais depuis GitHub Actions.
Ansible ne tournant pas nativement sur Windows, il doit être exécuté depuis WSL2 (voir
[deploiement-local.md](deploiement-local.md) pour les prérequis).
## Fichiers (`ansible/`)
| Fichier | Rôle |
| --- | --- |
| `inventory.ini` | Inventaire minimal : un hôte `localhost` en connexion locale |
| `group_vars/all.yml` | Variables par défaut (namespace, image, replicas, configuration Nginx/monitoring...) |
| `group_vars/local.yml` | *(non versionné)* Surcharge locale, notamment `grafana_admin_password` |
| `deploy.yml` | Playbook principal |
## Étapes orchestrées par `deploy.yml`
1. **Chargement des variables locales** (`group_vars/local.yml`, si présent) — permet de surcharger des valeurs sensibles
sans les committer.
2. **Vérification des prérequis** : `kubectl` disponible, `minikube status` retourne bien `Running`.
3. **Récupération des outputs Terraform** (`terraform output -json` dans `infra/terraform/`), chargés comme variables
Ansible (`namespace`, `sqlite_pvc_name`).
4. **Rendu des manifests Kubernetes** : les templates Jinja2 de `k8s/base/` et `k8s/monitoring/` sont rendus avec les
variables (image, tag, replicas, namespace, etc.) vers `k8s/rendered/` (dossier généré, non versionné).
5. **Application des manifests** avec `kubectl apply -f`, dans l'ordre : application + Nginx d'abord, puis attente que les
rollouts soient prêts (`kubectl rollout status`), puis la stack de monitoring (kube-state-metrics, Prometheus, Grafana),
avec les mêmes attentes de rollout.
6. **Affichage des URLs d'accès** (application via Nginx, Prometheus, Grafana), calculées à partir de `minikube ip` et des

NodePort effectivement attribués.
## Dépendance aux outputs Terraform
Le playbook ne peut pas s'exécuter avant que `terraform apply` ait été lancé : il lit `namespace` et `sqlite_pvc_name`
directement depuis l'état Terraform via `terraform output -json`. Si Terraform n'a pas encore été appliqué, la tâche «
Récupérer les outputs Terraform » échoue explicitement plutôt que de déployer dans le vide.
## Variables principales (`group_vars/all.yml`)
| Variable | Rôle |
| --- | --- |
| `app_image_name`, `app_image_tag` | Image et tag à déployer |
| `app_image_pull_policy` | `Always` par défaut (voir note ci-dessous) |
| `app_replicas`, `nginx_replicas` | Nombre de réplicas |
| `app_environment` | `ASPNETCORE_ENVIRONMENT` injecté dans le ConfigMap de l'application |
| `sqlite_db_path` | Chemin du fichier SQLite dans le conteneur (`/data/locatic.db`) |
| `nginx_service_type` | Type d'exposition du Service Nginx (`NodePort` par défaut) |
| `grafana_admin_password` | Mot de passe Grafana (valeur par défaut de développement ; à surcharger via
`group_vars/local.yml`) |
## Exécuter le playbook
```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
```
Simuler sans appliquer (dans la mesure où les modules utilisés le permettent) :
```bash
ansible-playbook -i inventory.ini deploy.yml --check
```
## Note : tag `:latest` et `imagePullPolicy`
Par défaut, `app_image_pull_policy` vaut `Always`. C'est volontaire : avec un tag mutable comme `:latest`, `IfNotPresent`
peut faire utiliser à Kubernetes une image obsolète déjà présente en cache local, sans jamais vérifier si le contenu a
changé sur le registre — ce qui a été observé concrètement pendant le développement de ce projet (voir
[exploitation.md](exploitation.md)). Pour une itération locale rapide avec `minikube image load` (sans repasser par GHCR),
on peut surcharger ponctuellement :
```bash
ansible-playbook -i inventory.ini deploy.yml -e app_image_pull_policy=Never
NodePort effectivement attribués.
## Dépendance aux outputs Terraform
Le playbook ne peut pas s'exécuter avant que `terraform apply` ait été lancé : il lit `namespace` et `sqlite_pvc_name`
directement depuis l'état Terraform via `terraform output -json`. Si Terraform n'a pas encore été appliqué, la tâche «
Récupérer les outputs Terraform » échoue explicitement plutôt que de déployer dans le vide.
## Variables principales (`group_vars/all.yml`)
| Variable | Rôle |
| --- | --- |
| `app_image_name`, `app_image_tag` | Image et tag à déployer |
| `app_image_pull_policy` | `Always` par défaut (voir note ci-dessous) |
| `app_replicas`, `nginx_replicas` | Nombre de réplicas |
| `app_environment` | `ASPNETCORE_ENVIRONMENT` injecté dans le ConfigMap de l'application |
| `sqlite_db_path` | Chemin du fichier SQLite dans le conteneur (`/data/locatic.db`) |
| `nginx_service_type` | Type d'exposition du Service Nginx (`NodePort` par défaut) |
| `grafana_admin_password` | Mot de passe Grafana (valeur par défaut de développement ; à surcharger via
`group_vars/local.yml`) |
## Exécuter le playbook
```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
```
Simuler sans appliquer (dans la mesure où les modules utilisés le permettent) :
bash
ansible-playbook -i inventory.ini deploy.yml -e app_image_pull_policy=Never