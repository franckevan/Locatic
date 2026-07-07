variable "kubeconfig_path" {
  type    = string
  default = "~/.kube/config"
}

variable "kube_context" {
  type        = string
  description = "Contexte kubectl à utiliser (minikube par défaut)."
  default     = "minikube"
}

variable "namespace" {
  type        = string
  description = "Namespace Kubernetes dans lequel Locatic est déployé."
  default     = "locatic"
}

variable "app_name" {
  type    = string
  default = "locatic"
}

variable "sqlite_storage" {
  type        = string
  description = "Taille du volume persistant utilisé pour le fichier SQLite."
  default     = "1Gi"
}

variable "sqlite_storage_class" {
  type        = string
  description = "StorageClass à utiliser pour le PVC SQLite (laisser vide pour la classe par défaut du cluster)."
  default     = ""
}
