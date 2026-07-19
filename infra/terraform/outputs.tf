output "namespace" {
  description = "namespace cree"
  value       = kubernetes_namespace.locatic.metadata[0].name
}

output "sqlite_pvc_name" {
  description = "nom du PVC sqlite, utilise par ansible"
  value       = kubernetes_persistent_volume_claim.sqlite_data.metadata[0].name
}
