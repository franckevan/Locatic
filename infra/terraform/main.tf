terraform {
  required_version = ">= 1.5"
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.35"
    }
  }
}

provider "kubernetes" {
  config_path    = pathexpand(var.kubeconfig_path)
  config_context = var.kube_context
}

# Terraform ne prépare que l'infrastructure locale : namespace + stockage.
# Le déploiement des ressources applicatives (Nginx, app, monitoring) est
# ensuite orchestré par Ansible, qui consomme les outputs ci-dessous.

resource "kubernetes_namespace_v1" "locatic" {
  metadata {
    name = var.namespace
    labels = {
      project    = "locatic"
      managed-by = "terraform"
    }
  }
}

resource "kubernetes_persistent_volume_claim_v1" "sqlite" {
  metadata {
    name      = "${var.app_name}-sqlite-pvc"
    namespace = kubernetes_namespace_v1.locatic.metadata[0].name
    labels = {
      app       = var.app_name
      component = "storage"
    }
  }

  spec {
    access_modes = ["ReadWriteOnce"]

    resources {
      requests = {
        storage = var.sqlite_storage
      }
    }

    storage_class_name = var.sqlite_storage_class != "" ? var.sqlite_storage_class : null
  }

  wait_until_bound = false
}
