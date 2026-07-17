resource "kubernetes_namespace" "locatic" {
  metadata {
    name = var.namespace
    labels = {
      "app.kubernetes.io/part-of" = "locatic"
    }
  }
}

resource "kubernetes_persistent_volume_claim" "sqlite_data" {
  metadata {
    name      = "locatic-sqlite-pvc"
    namespace = kubernetes_namespace.locatic.metadata[0].name
  }

  spec {
    access_modes = ["ReadWriteOnce"]
    resources {
      requests = {
        storage = var.sqlite_storage_size
      }
    }
  }
}
