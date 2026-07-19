# Déploiement local
Ce document décrit l'ordre exact des actions locales pour passer d'une image publiée sur `ghcr.io` à une application
déployée sur minikube, derrière Nginx, avec monitoring.
## Prérequis
| Outil | Où | Vérification |
| --- | --- | --- |
| Docker Desktop | Windows | `docker version` |
| minikube | Windows **et** WSL2 (voir note ci-dessous) | `minikube version` |
| kubectl | Windows et WSL2 | `kubectl version --client` |
| Terraform | Windows et WSL2 | `terraform version` |
| Ansible | **WSL2 uniquement** (pas de support natif Windows) | `ansible --version` |
| Git / GitHub CLI | Windows | `git --version` |
### Pourquoi WSL2 ?
Ansible ne s'installe pas nativement sur Windows. L'ensemble de la chaîne Terraform → Ansible → kubectl a donc été
exécutée depuis WSL2 (Ubuntu) pour rester cohérente d'un bout à l'autre, plutôt que de jongler entre deux environnements.
**Point d'attention machine-spécifique** : sur cette machine, le `docker` de WSL2 et celui de Windows ne pointent **pas**
vers le même daemon (l'intégration WSL de Docker Desktop n'était pas complètement effective pour la distributionutilisée). Conséquence concrète : une image construite avec `docker build` côté Windows n'est pas visible par `minikube
image load` exécuté depuis WSL. **Toujours builder l'image Docker depuis le même environnement que celui utilisé pour
piloter minikube** (WSL2 dans ce projet) lors d'une itération locale. Vérifiez avec :
```bash
# côté Windows
docker images | grep locatic
# côté WSL
wsl -e bash -lc "docker images | grep locatic"
```
Si les IDs d'image diffèrent, vous n'êtes pas sur le même daemon.
## Étape par étape
### 1. Démarrer minikube
```bash
minikube start --driver=docker
minikube status
```
### 2. Préparer l'infrastructure avec Terraform
```bash
cd infra/terraform
terraform init
terraform apply
terraform output
cd ../..
```
Crée le namespace `locatic` et le PersistentVolumeClaim SQLite.
### 3. Déployer avec Ansible
```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
cd ..
```
Ce playbook :- vérifie que minikube tourne,- lit les outputs Terraform,- rend et applique les manifests de l'application, de Nginx, puis du monitoring (Prometheus, Grafana, kube-state-metrics),- affiche les URLs d'accès en fin d'exécution.
### 4. Vérifier
```bash
kubectl get all -n locatic
kubectl get pvc -n locatic
```
Tous les pods doivent être `Running` avec leurs conteneurs `Ready` (ex. `1/1`, `2/2` pour Nginx qui a un sidecar).
### 5. Accéder à l'application
```bash
kubectl get svc nginx -n locatic# Déploiement local
Ce document décrit l'ordre exact des actions locales pour passer d'une image publiée sur `ghcr.io` à une application
déployée sur minikube, derrière Nginx, avec monitoring.
## Prérequis
| Outil | Où | Vérification |
| --- | --- | --- |
| Docker Desktop | Windows | `docker version` |
| minikube | Windows **et** WSL2 (voir note ci-dessous) | `minikube version` |
| kubectl | Windows et WSL2 | `kubectl version --client` |
| Terraform | Windows et WSL2 | `terraform version` |
| Ansible | **WSL2 uniquement** (pas de support natif Windows) | `ansible --version` |
| Git / GitHub CLI | Windows | `git --version` |
### Pourquoi WSL2 ?
Ansible ne s'installe pas nativement sur Windows. L'ensemble de la chaîne Terraform → Ansible → kubectl a donc été
exécutée depuis WSL2 (Ubuntu) pour rester cohérente d'un bout à l'autre, plutôt que de jongler entre deux environnements.
**Point d'attention machine-spécifique** : sur cette machine, le `docker` de WSL2 et celui de Windows ne pointent **pas**
vers le même daemon (l'intégration WSL de Docker Desktop n'était pas complètement effective pour la distributionutilisée). Conséquence concrète : une image construite avec `docker build` côté Windows n'est pas visible par `minikube
image load` exécuté depuis WSL. **Toujours builder l'image Docker depuis le même environnement que celui utilisé pour
piloter minikube** (WSL2 dans ce projet) lors d'une itération locale. Vérifiez avec :
```bash
# côté Windows
docker images | grep locatic
# côté WSL
wsl -e bash -lc "docker images | grep locatic"
```
Si les IDs d'image diffèrent, vous n'êtes pas sur le même daemon.
## Étape par étape
### 1. Démarrer minikube
```bash
minikube start --driver=docker
minikube status
```
### 2. Préparer l'infrastructure avec Terraform
```bash
cd infra/terraform
terraform init
terraform apply
terraform output
cd ../..
```
Crée le namespace `locatic` et le PersistentVolumeClaim SQLite.
### 3. Déployer avec Ansible
```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
cd ..
```
Ce playbook :- vérifie que minikube tourne,- lit les outputs Terraform,- rend et applique les manifests de l'application, de Nginx, puis du monitoring (Prometheus, Grafana, kube-state-metrics),- affiche les URLs d'accès en fin d'exécution.
### 4. Vérifier
```bash
kubectl get all -n locatic
kubectl get pvc -n locatic
```
Tous les pods doivent être `Running` avec leurs conteneurs `Ready` (ex. `1/1`, `2/2` pour Nginx qui a un sidecar).
### 5. Accéder à l'application
```bash
kubectl get svc nginx -n locatic
```
Puis ouvrir `http://<minikube ip>:<nodeport http>` dans un navigateur. `minikube ip` donne l'adresse.
## Mettre à jour après un nouveau commit sur `main`
1. La CI publie automatiquement la nouvelle image sur `ghcr.io` (tag `latest` + tag SHA du commit).
2. Localement : relancer simplement `ansible-playbook -i inventory.ini deploy.yml` depuis `ansible/`. Avec
`app_image_pull_policy: Always` (valeur par défaut), Kubernetes retire systématiquement la dernière image `:latest` duregistre au redémarrage du pod.
## Itérer localement sans passer par GHCR (développement)
Utile pour tester une modification avant de la committer :
```bash
docker build -t ghcr.io/franckevan/locatic:latest .        # depuis le même environnement que minikube (voir note
ci-dessus)
minikube image load ghcr.io/franckevan/locatic:latest
kubectl rollout restart deployment/locatic-app -n locatic
```
Si l'image ne semble pas se mettre à jour malgré tout (cache `minikube image load` parfois tenace), forcer un remplacement
complet :
```bash
kubectl scale deployment/locatic-app -n locatic --replicas=0
minikube image rm ghcr.io/franckevan/locatic:latest
minikube image load ghcr.io/franckevan/locatic:latest
kubectl scale deployment/locatic-app -n locatic --replicas=1
```
Voir [exploitation.md](exploitation.md) pour le détail de cet incident rencontré pendant le développement.```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
cd ..
```
Ce playbook :- vérifie que minikube tourne,- lit les outputs Terraform,- rend et applique les manifests de l'application, de Nginx, puis du monitoring (Prometheus, Grafana, kube-state-metrics),- affiche les URLs d'accès en fin d'exécution.
### 4. Vérifier
```bash
kubectl get all -n locatic
kubectl get pvc -n locatic
```
Tous les pods doivent être `Running` avec leurs conteneurs `Ready` (ex. `1/1`, `2/2` pour Nginx qui a un sidecar).
### 5. Accéder à l'application
```bash
kubectl get svc nginx -n locatic
```
Puis ouvrir `http://<minikube ip>:<nodeport http>` dans un navigateur. `minikube ip` donne l'adresse.
## Mettre à jour après un nouveau commit sur `main`
1. La CI publie automatiquement la nouvelle image sur `ghcr.io` (tag `latest` + tag SHA du commit).
2. Localement : relancer simplement `ansible-playbook -i inventory.ini deploy.yml` depuis `ansible/`. Avec
`app_image_pull_policy: Always` (valeur par défaut), Kubernetes retire systématiquement la dernière image `:latest` duregistre au redémarrage du pod.
## Itérer localement sans passer par GHCR (développement)
Utile pour tester une modification avant de la committer :
```bash
docker build -t ghcr.io/franckevan/locatic:latest .        # depuis le même environnement que minikube (voir note
ci-dessus)
minikube image load ghcr.io/franckevan/locatic:latest
kubectl rollout restart deployment/locatic-app -n locatic
```
Si l'image ne semble pas se mettre à jour malgré tout (cache `minikube image load` parfois tenace), forcer un remplacement
complet :

Voir [exploitation.md](exploitation.md) pour le détail de cet incident rencontré pendant le développement.```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
cd ..
```
Ce playbook :- vérifie que minikube tourne,- lit les outputs Terraform,- rend et applique les manifests de l'application, de Nginx, puis du monitoring (Prometheus, Grafana, kube-state-metrics),- affiche les URLs d'accès en fin d'exécution.
### 4. Vérifier
```bash
kubectl get all -n locatic
kubectl get pvc -n locatic
```
Tous les pods doivent être `Running` avec leurs conteneurs `Ready` (ex. `1/1`, `2/2` pour Nginx qui a un sidecar).
### 5. Accéder à l'application
```bash
kubectl get svc nginx -n locatic
```
Puis ouvrir `http://<minikube ip>:<nodeport http>` dans un navigateur. `minikube ip` donne l'adresse.
## Mettre à jour après un nouveau commit sur `main`
1. La CI publie automatiquement la nouvelle image sur `ghcr.io` (tag `latest` + tag SHA du commit).
2. Localement : relancer simplement `ansible-playbook -i inventory.ini deploy.yml` depuis `ansible/`. Avec
`app_image_pull_policy: Always` (valeur par défaut), Kubernetes retire systématiquement la dernière image `:latest` duregistre au redémarrage du pod.
## Itérer localement sans passer par GHCR (développement)
Utile pour tester une modification avant de la committer :
```bash
docker build -t ghcr.io/franckevan/locatic:latest .        # depuis le même environnement que minikube (voir note
ci-dessus)
minikube image load ghcr.io/franckevan/locatic:latest
kubectl rollout restart deployment/locatic-app -n locatic
```
Si l'image ne semble pas se mettre à jour malgré tout (cache `minikube image load` parfois tenace), forcer un remplacement
complet :
```bash
kubectl scale deployment/locatic-app -n locatic --replicas=0
minikube image rm ghcr.io/franckevan/locatic:latest
minikube image load ghcr.io/franckevan/locatic:latest
kubectl scale deployment/locatic-app -n locatic --replicas=1
```
Voir [exploitation.md](exploitation.md) pour le détail de cet incident rencontré pendant le développement.```bash
cd ansible
ansible-playbook -i inventory.ini deploy.yml
cd ..
```
Ce playbook :- vérifie que minikube tourne,- lit les outputs Terraform,- rend et applique les manifests de l'application, de Nginx, puis du monitoring (Prometheus, Grafana, kube-state-metrics),- affiche les URLs d'accès en fin d'exécution.
### 4. Vérifier
```bash
kubectl get all -n locatic
kubectl get pvc -n locatic
```
Tous les pods doivent être `Running` avec leurs conteneurs `Ready` (ex. `1/1`, `2/2` pour Nginx qui a un sidecar).
### 5. Accéder à l'application
```bash
kubectl get svc nginx -n locatic
```
Puis ouvrir `http://<minikube ip>:<nodeport http>` dans un navigateur. `minikube ip` donne l'adresse.
## Mettre à jour après un nouveau commit sur `main`
1. La CI publie automatiquement la nouvelle image sur `ghcr.io` (tag `latest` + tag SHA du commit).
2. Localement : relancer simplement `ansible-playbook -i inventory.ini deploy.yml` depuis `ansible/`. Avec
`app_image_pull_policy: Always` (valeur par défaut), Kubernetes retire systématiquement la dernière image `:latest` duregistre au redémarrage du pod.
## Itérer localement sans passer par GHCR (développement)
Utile pour tester une modification avant de la committer :

Si l'image ne semble pas se mettre à jour malgré tout (cache `minikube image load` parfois tenace), forcer un remplacement
complet :

Voir [exploitation.md](exploitation.md) pour le détail de cet incident rencontré pendant le développement.