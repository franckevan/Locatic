variable "kube_context" {
  description = "contexte kubectl a utiliser"
  type        = string
  default     = "minikube"
}

variable "namespace" {
  description = "namespace kubernetes pour l'appli"
  type        = string
  default     = "locatic"
}

variable "sqlite_storage_size" {
  description = "taille du volume pour sqlite"
  type        = string
  default     = "1Gi"
}
