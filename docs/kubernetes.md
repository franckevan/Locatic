# Kubernetes
## Organisation des manifests
Les manifests sont des **templates Jinja2** (`.yaml.j2`), rendus par Ansible avant application (voir
[ansible.md](ansible.md)). C'est ce mécanisme qui permet de changer facilement l'image, le tag, les replicas, etc. sans
dupliquer de fichiers YAML, sans passer par Helm (bonus non réalisé — voir [helm.md](helm.md)).
```
k8s/
├── base/                              # application + Nginx
│   ├── app-configmap.yaml.j2
│   ├── app-deployment.yaml.j2
│   ├── app-service.yaml.j2
│   ├── nginx-configmap.yaml.j2
│   ├── nginx-deployment.yaml.j2
│   └── nginx-service.yaml.j2
├── monitoring/                        # Prometheus, Grafana, kube-state-metrics
│   ├── kube-state-metrics-rbac.yaml.j2
│   ├── kube-state-metrics-deployment.yaml.j2                                                                          │   ├── kube-state-metrics-service.yaml.j2
│   ├── prometheus-configmap.yaml.j2
│   ├── prometheus-deployment.yaml.j2
│   ├── prometheus-service.yaml.j2
│   ├── grafana-secret.yaml.j2
│   ├── grafana-datasource-configmap.yaml.j2
│   ├── grafana-dashboard-provisioning-configmap.yaml.j2
│   ├── grafana-dashboard-configmap.yaml.j2
│   ├── grafana-deployment.yaml.j2
│   └── grafana-service.yaml.j2
└── rendered/                          # généré par Ansible, non versionné
```
Le namespace `locatic` et le `PersistentVolumeClaim` ne sont **pas** définis ici : ils sont gérés par Terraform (voir
[terraform.md](terraform.md)), pour éviter que deux outils ne se disputent la même ressource.
## Ressources de l'application (`k8s/base/`)- **`app-configmap`** : variables d'environnement non sensibles (`ASPNETCORE_ENVIRONMENT`, chemin de connexion SQLite).- **`app-deployment`** :
  - image et tag configurables (`app_image_name:app_image_tag`)
 - `securityContext.fsGroup: 1000` — nécessaire pour que le conteneur non-root (`appuser`, uid 1000) puisse écrire sur le
volume SQLite monté (voir [exploitation.md](exploitation.md) pour l'incident qui a motivé ce réglage)
  - `readinessProbe` / `livenessProbe` sur `GET /health`
  - `resources.requests/limits` définis (CPU/mémoire)
  - volume `sqlite-data` monté sur `/data`, référencé via `{{ sqlite_pvc_name }}` (nom du PVC créé par Terraform)- **`app-service`** : `ClusterIP` — **volontairement non exposé** à l'extérieur du cluster. C'est l'application de la
contrainte « l'application ne doit pas être exposée directement ».
## Ressources Nginx (`k8s/base/`)- **`nginx-configmap`** : configuration du reverse proxy (`proxy_pass` vers `locatic-app:8080`), plus une location
`/nginx_status` (stub_status Nginx) accessible uniquement en local au sein du pod (pour le sidecar de métriques).- **`nginx-deployment`** : deux conteneurs dans le même pod —
  - `nginx` (image `nginx:1.27-alpine`), reverse proxy
 - `nginx-exporter` (image `nginx/nginx-prometheus-exporter`), sidecar qui lit `stub_status` en local et expose des
métriques Prometheus sur le port 9113- **`nginx-service`** : `NodePort`, avec deux ports nommés (`http` 80, `metrics` 9113) — c'est **le seul point d'entrée
utilisateur** du système.
## Ressources de monitoring (`k8s/monitoring/`)
Voir [monitoring.md](monitoring.md) pour le détail.
## Configuration facilement modifiable
Toutes les valeurs suivantes se changent dans `ansible/group_vars/all.yml` (ou en surcharge `-e` à l'exécution d'Ansible),
sans toucher aux fichiers YAML :
| Élément | Variable |
| --- | --- |
| Nom de l'image | `app_image_name` |
| Tag de l'image | `app_image_tag` |
| Nombre de replicas (app / Nginx) | `app_replicas` / `nginx_replicas` |
| Variables d'environnement de l'app | `app_environment`, `sqlite_db_path` |
| Type d'exposition du service Nginx | `nginx_service_type` |
| Chemin de stockage SQLite | `sqlite_db_path` |
| Activation du monitoring | les templates `k8s/monitoring/*` sont toujours appliqués par le playbook actuel ; les rendre
optionnels reviendrait à conditionner leur rendu/application par une variable booléenne (non fait par manque de temps,
amélioration possible) |
## Vérifier le déploiement
```bash
kubectl get all -n locatic
kubectl get pvc -n locatic
kubectl describe deployment locatic-app -n locatic
kubectl logs -n locatic -l app=locatic-app