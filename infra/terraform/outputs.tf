output "namespace" {
  description = "Namespace Kubernetes créé pour Locatic"
  value       = kubernetes_namespace.locatic.metadata[0].name
}

output "sqlite_pvc_name" {
  description = "Nom du PersistentVolumeClaim utilisé par SQLite"
  value       = kubernetes_persistent_volume_claim.sqlite_data.metadata[0].name
}
