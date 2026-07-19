# Helm (bonus — non réalisé)
Le chart Helm est un bonus explicitement optionnel du sujet ; il n'a pas été implémenté dans ce projet par manque de
temps.
À la place, la configurabilité du déploiement (image, tag, replicas, variables d'environnement, exposition, stockage) est
assurée par des **templates Jinja2 rendus par Ansible** (voir [kubernetes.md](kubernetes.md) et [ansible.md](ansible.md)),
ce qui répond à la même exigence fonctionnelle du sujet (« La configuration doit permettre de changer facilement... »)
sans dépendance supplémentaire.
## Piste d'évolution
Si le chart Helm devait être ajouté :- convertir `k8s/base/*.yaml.j2` et `k8s/monitoring/*.yaml.j2` en templates Helm (`templates/*.yaml`), en remplaçant la
syntaxe Jinja2 (`{{ var }}`) par la syntaxe Go template de Helm (`{{ .Values.var }}`)- définir `values.yaml` avec les mêmes clés que `ansible/group_vars/all.yml`- remplacer, dans `ansible/deploy.yml`, les tâches de rendu + `kubectl apply` par une tâche `helm upgrade --install
locatic ./helm/locatic-chart -f values-local.yaml` (ou le module `kubernetes.core.helm`)