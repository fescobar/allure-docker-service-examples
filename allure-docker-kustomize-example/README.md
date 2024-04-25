# ALLURE-DOCKER-SERVICE KUSTOMIZE EXAMPLE

## USAGE
These files would typically be in a `/base` directory that is patched for a specific environment.  

For example in a directory structure like this where `base` is the dirctory containing this readme.:

```bash
├── base
│   ├── api
│   ├── kustomization.yaml
│   ├── namespace.yaml
│   └── ui
└── environments
    ├── dev
    │   ├── config
    │   ├── kustomization.yaml
    │   └── patches
    └── prod
        ├── config
        ├── kustomization.yaml
        └── patches
```

Then in an environments kustomization.yaml:  

`/environments/dev/kustomization.yaml`:
```yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

resources:
  - ../../base
  # your other env specific resources here

patches:
  - path: patches/api-secret.yaml
  # your other patches here

images:
  - name: allure-api-image
    newName: frankescobar/allure-docker-service
    newTag: latest
  - name: allure-ui-image
    newName: frankescobar/allure-docker-service-ui
    newTag: latest
```