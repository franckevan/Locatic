output "namespace" {
  description = "Namespace Kubernetes préparé pour Locatic."
  value       = kubernetes_namespace_v1.locatic.metadata[0].name
}

output "sqlite_pvc_name" {
  description = "Nom du PersistentVolumeClaim SQLite, à consommer par Ansible/Helm."
  value       = kubernetes_persistent_volume_claim_v1.sqlite.metadata[0].name
}

output "kube_context" {
  description = "Contexte kubectl utilisé, transmis tel quel à Ansible."
  value       = var.kube_context
}

output "ansible_vars" {
  description = "Bloc de variables prêt à être consommé par le playbook Ansible."
  value = {
    namespace       = kubernetes_namespace_v1.locatic.metadata[0].name
    kube_context    = var.kube_context
    sqlite_pvc_name = kubernetes_persistent_volume_claim_v1.sqlite.metadata[0].name
    app_name        = var.app_name
  }
}
