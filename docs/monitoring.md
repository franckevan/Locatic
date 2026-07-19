# Monitoring
## Services monitorés
| Service | Comment il est monitoré |
| --- | --- |
| Application Locatic | Endpoint `/metrics` (format Prometheus), exposé via `prometheus-net.AspNetCore`. Métriques HTTP
(`http_request_duration_seconds_*`, par méthode/code/endpoint) et métriques runtime .NET (mémoire, GC, threads) |
| Nginx | Sidecar `nginx-prometheus-exporter` dans le même pod, qui lit le `stub_status` interne de Nginx (accessible
uniquement en local) et expose des métriques (`nginx_http_requests_total`, connexions actives...) sur le port 9113 |
| Pods / Deployments / PVC (état Kubernetes) | `kube-state-metrics`, déployé avec un `ServiceAccount` + `ClusterRole` +
`ClusterRoleBinding` en lecture seule (pods, services, PVC, PV, deployments, replicasets) |
| Prometheus lui-même | Scrapé en `localhost:9090`, comme n'importe quelle autre cible |
Chaque service a donc au moins un indicateur (`up{job="..."}`) visible dans Grafana, plus des métriques applicatives plus
fines pour l'application et Nginx.
## Architecture du monitoring
```
Prometheus (scrape toutes les 15s)
   ├─ locatic-app:8080/metrics
   ├─ nginx:9113/metrics          (sidecar nginx-prometheus-exporter)
   ├─ kube-state-metrics:8080/metrics
   └─ localhost:9090 (lui-même)
Grafana
   └─ datasource Prometheus (provisionnée automatiquement)
   └─ dashboard "Locatic - Vue d'ensemble" (provisionné automatiquement)
```
Prometheus et Grafana sont déployés dans le **même namespace** (`locatic`) que l'application, pour rester dans le
périmètre géré par Terraform/Ansible sans complexité RBAC inter-namespace supplémentaire.
## Accès à Prometheus
```bash
kubectl get svc prometheus -n locatic
```
Récupère le NodePort attribué, puis :
```http://$(minikube ip):<nodeport>
```
Cible utile : `/targets` pour voir l'état de santé de chaque scrape (`up` / `down`), `/alerts` pour l'état des règles
d'alerte.
## Accès à Grafana
Même principe pour le service `grafana`. Identifiants : `admin` / valeur de `grafana_admin_password` (par défaut
`changeme` en local — voir [ansible.md](ansible.md) pour la surcharge non versionnée via `group_vars/local.yml`).
## Lecture du dashboard "Locatic - Vue d'ensemble"
Le dashboard est composé de deux rangées :
**Rangée 1 — état instantané (stat panels, vert = up/bound, rouge = down) :**- Application Locatic (`up{job="locatic-app"}`)- Nginx (`up{job="nginx"}`)- Prometheus (`up{job="prometheus"}`)- Volume SQLite / PVC Bound (`kube_persistentvolumeclaim_status_phase{phase="Bound"}`)- Pods en cours d'exécution (`sum(kube_pod_status_phase{phase="Running"})`)
**Rangée 2 — activité (graphiques) :**- Débit de requêtes de l'application (`rate(http_request_duration_seconds_count[5m])`)- Débit de requêtes Nginx (`rate(nginx_http_requests_total[5m])`)
En un coup d'œil, ce dashboard permet de répondre à : Nginx répond-il ? L'application répond-elle ? Le volume SQLite
est-il correctement monté ? Combien de pods tournent ? Y a-t-il du trafic ?
## Alertes
Trois règles d'alerte simples sont chargées dans Prometheus (`k8s/monitoring/prometheus-configmap.yaml.j2`, clé
`rules.yml`) :
| Alerte | Condition | Sévérité |
| --- | --- | --- |
| `AppDown` | `up{job="locatic-app"} == 0` pendant 1 minute | critical |
| `NginxDown` | `up{job="nginx"} == 0` pendant 1 minute | critical |
| `SqlitePvcNotBound` | `kube_persistentvolumeclaim_status_phase{phase="Bound"} == 0` pendant 2 minutes | warning |
Ces alertes sont visibles dans l'interface Prometheus (`/alerts`). Aucun Alertmanager n'a été déployé
(routage/notification des alertes vers Slack, email, etc.) — c'est une limite connue assumée pour rester dans le périmètre
temporel du projet ; voir [exploitation.md](exploitation.md).
## Limite connue : ordre d'application
Le namespace `locatic` étant partagé par l'application et le monitoring, un premier déploiement complet peut nécessiter un
court délai avant que toutes les cibles Prometheus passent à `up` (le temps que chaque pod devienne `Ready`). Le playbook
Ansible attend explicitement chaque `rollout status` avant de passer à la suite, ce qui limite ce risque.# Monitoring
## Services monitorés
| Service | Comment il est monitoré |
| --- | --- |
| Application Locatic | Endpoint `/metrics` (format Prometheus), exposé via `prometheus-net.AspNetCore`. Métriques HTTP
(`http_request_duration_seconds_*`, par méthode/code/endpoint) et métriques runtime .NET (mémoire, GC, threads) |
| Nginx | Sidecar `nginx-prometheus-exporter` dans le même pod, qui lit le `stub_status` interne de Nginx (accessible
uniquement en local) et expose des métriques (`nginx_http_requests_total`, connexions actives...) sur le port 9113 |
| Pods / Deployments / PVC (état Kubernetes) | `kube-state-metrics`, déployé avec un `ServiceAccount` + `ClusterRole` +
`ClusterRoleBinding` en lecture seule (pods, services, PVC, PV, deployments, replicasets) |
| Prometheus lui-même | Scrapé en `localhost:9090`, comme n'importe quelle autre cible |
Chaque service a donc au moins un indicateur (`up{job="..."}`) visible dans Grafana, plus des métriques applicatives plus
fines pour l'application et Nginx.
## Architecture du monitoring
```
Prometheus (scrape toutes les 15s)
   ├─ locatic-app:8080/metrics
   ├─ nginx:9113/metrics          (sidecar nginx-prometheus-exporter)
   ├─ kube-state-metrics:8080/metrics
   └─ localhost:9090 (lui-même)
Grafana
   └─ datasource Prometheus (provisionnée automatiquement)
   └─ dashboard "Locatic - Vue d'ensemble" (provisionné automatiquement)
```
Prometheus et Grafana sont déployés dans le **même namespace** (`locatic`) que l'application, pour rester dans le
périmètre géré par Terraform/Ansible sans complexité RBAC inter-namespace supplémentaire.
## Accès à Prometheus

Cible utile : `/targets` pour voir l'état de santé de chaque scrape (`up` / `down`), `/alerts` pour l'état des règles
d'alerte.
## Accès à Grafana
Même principe pour le service `grafana`. Identifiants : `admin` / valeur de `grafana_admin_password` (par défaut
`changeme` en local — voir [ansible.md](ansible.md) pour la surcharge non versionnée via `group_vars/local.yml`).
## Lecture du dashboard "Locatic - Vue d'ensemble"
Le dashboard est composé de deux rangées :
**Rangée 1 — état instantané (stat panels, vert = up/bound, rouge = down) :**- Application Locatic (`up{job="locatic-app"}`)- Nginx (`up{job="nginx"}`)- Prometheus (`up{job="prometheus"}`)- Volume SQLite / PVC Bound (`kube_persistentvolumeclaim_status_phase{phase="Bound"}`)- Pods en cours d'exécution (`sum(kube_pod_status_phase{phase="Running"})`)
**Rangée 2 — activité (graphiques) :**- Débit de requêtes de l'application (`rate(http_request_duration_seconds_count[5m])`)- Débit de requêtes Nginx (`rate(nginx_http_requests_total[5m])`)
En un coup d'œil, ce dashboard permet de répondre à : Nginx répond-il ? L'application répond-elle ? Le volume SQLite
est-il correctement monté ? Combien de pods tournent ? Y a-t-il du trafic ?
## Alertes
Trois règles d'alerte simples sont chargées dans Prometheus (`k8s/monitoring/prometheus-configmap.yaml.j2`, clé
`rules.yml`) :
| Alerte | Condition | Sévérité |
| --- | --- | --- |
| `AppDown` | `up{job="locatic-app"} == 0` pendant 1 minute | critical |
| `NginxDown` | `up{job="nginx"} == 0` pendant 1 minute | critical |
| `SqlitePvcNotBound` | `kube_persistentvolumeclaim_status_phase{phase="Bound"} == 0` pendant 2 minutes | warning |
Ces alertes sont visibles dans l'interface Prometheus (`/alerts`). Aucun Alertmanager n'a été déployé
(routage/notification des alertes vers Slack, email, etc.) — c'est une limite connue assumée pour rester dans le périmètre
temporel du projet ; voir [exploitation.md](exploitation.md).
## Limite connue : ordre d'application
Le namespace `locatic` étant partagé par l'application et le monitoring, un premier déploiement complet peut nécessiter un
court délai avant que toutes les cibles Prometheus passent à `up` (le temps que chaque pod devienne `Ready`). Le playbook
Ansible attend explicitement chaque `rollout status` avant de passer à la suite, ce qui limite ce risque.