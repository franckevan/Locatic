# Terraform
## Rôle
Terraform prépare les deux éléments d'infrastructure locale nécessaires avant tout déploiement applicatif :- le namespace Kubernetes `locatic`- le `PersistentVolumeClaim` (PVC) qui portera la base SQLite
Il pilote directement l'API Kubernetes du cluster minikube local via le provider officiel `hashicorp/kubernetes`, en
s'appuyant sur le kubeconfig déjà configuré par `minikube start`. Aucune ressource cloud n'est créée — tout reste local.
## Fichiers (`infra/terraform/`)
| Fichier | Contenu |
| --- | --- |
| `providers.tf` | Déclaration du provider `kubernetes`, pointant vers le kubeconfig local (`~/.kube/config`, contexte
`minikube`) |
| `variables.tf` | Variables : `kube_context`, `namespace`, `sqlite_storage_size` |
| `main.tf` | Ressources : `kubernetes_namespace.locatic`, `kubernetes_persistent_volume_claim.sqlite_data` |
| `outputs.tf` | Outputs : `namespace`, `sqlite_pvc_name` |
| `.terraform.lock.hcl` | Verrouillage des versions de provider (versionné, contrairement à l'état) |
| `terraform.tfvars.example` | Exemple de fichier de variables locales (à copier en `terraform.tfvars`, non versionné) |
## Ressources gérées- `kubernetes_namespace.locatic` — namespace dédié à tous les objets Locatic (application, Nginx, monitoring).- `kubernetes_persistent_volume_claim.sqlite_data` — PVC `locatic-sqlite-pvc`, mode `ReadWriteOnce`, taille configurable
(`1Gi` par défaut).
## Variables
| Variable | Défaut | Rôle |
| --- | --- | --- |
| `kube_context` | `minikube` | Contexte kubectl à utiliser |
| `namespace` | `locatic` | Nom du namespace Kubernetes |
| `sqlite_storage_size` | `1Gi` | Taille du volume persistant SQLite |
## Outputs utiles
| Output | Utilisation |
| --- | --- || `namespace` | Consommé par Ansible (`terraform output -json`) pour cibler le bon namespace lors du déploiement |
| `sqlite_pvc_name` | Consommé par Ansible pour référencer le PVC dans le manifest de Deployment de l'application |
## Gestion de l'état
L'état Terraform (`terraform.tfstate`, `terraform.tfstate.backup`, dossier `.terraform/`) **n'est pas versionné** — ces
chemins sont explicitement exclus dans `.gitignore` (`infra/terraform/.terraform/`, `infra/terraform/*.tfstate*`). L'état
reste local à la machine du développeur, comme l'ensemble du déploiement.
## Initialiser, planifier, appliquer
Depuis `infra/terraform/` :
```bash
terraform init      # télécharge le provider kubernetes
terraform plan       # prévisualise les changements
terraform apply      # applique (namespace + PVC)
terraform output     # récupère les valeurs pour Ansible
```
Pour détruire proprement l'infrastructure locale :
```bash
terraform destroy
```
## Point d'attention
Ce projet a été testé avec deux installations Terraform différentes sur la même machine : une côté Windows (utilisée un
temps) et une côté WSL2 (utilisée pour la suite, car Ansible impose WSL2). **Le provider Kubernetes téléchargé par
`terraform init` est spécifique à la plateforme** (`windows_amd64` vs `linux_amd64`) : si vous changez d'environnement
d'exécution, relancez `terraform init` pour retélécharger le bon binaire de provider. L'état (`terraform.tfstate`), lui,
reste valide d'un environnement à l'autre puisqu'il est stocké sur le disque partagé (`/mnt/d/...` ↔ `D:\...`).