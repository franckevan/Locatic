variable "kube_context" {
  description = "Contexte kubectl à utiliser (cluster minikube local)"
  type        = string
  default     = "minikube"
}

variable "namespace" {
  description = "Namespace Kubernetes de l'application Locatic"
  type        = string
  default     = "locatic"
}

variable "sqlite_storage_size" {
  description = "Taille du volume persistant pour la base SQLite"
  type        = string
  default     = "1Gi"
}
