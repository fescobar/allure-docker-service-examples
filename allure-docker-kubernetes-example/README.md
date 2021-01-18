[![](../resources/allure.png)](http://allure.qatools.ru/)
[![](../resources/docker.png)](https://docs.docker.com/)
[![](../resources/kubernetes.png)](https://kubernetes.io/)

# ALLURE-DOCKER-SERVICE KUBERNETES EXAMPLE
Table of contents
=================
* [USAGE](#USAGE)
    * [Creating a Namespace](#creating-a-namespace)
    * [Creating a Config Map for API](#creating-a-config-map-for-api)
    * [Creating a Secret for API](#creating-a-secret-for-api)
    * [Creating a Config Map for UI](#creating-a-config-map-for-ui)
    * [Creating a Persistent Volume for API](#creating-a-persistent-volume-for-api)
    * [Creating a Persistent Volume Claim for API](#creating-a-persistent-volume-claim-for-api)
    * [Creating a Deployment](#creating-a-deployment)
    * [Creating a Service](#creating-a-service)
        * [Creating a Load Balancer Service](#creating-a-load-balancer-service)
        * [Creating a Node Port Service](#creating-a-node-port-service)
    * [Creating a SSL certificate and key](#creating-a-ssl-certificate-and-key)
    * [Creating a TLS Secret](#creating-a-tls-secret)
    * [Creating an Ingress](#creating-an-ingress)
        * [Creating an Ingress for Load Balancer](#creating-an-ingress-for-load-balancer)
        * [Creating an Ingress for Node Port](#creating-an-ingress-for-node-port)
    * [Using Application Deployed](#using-application-deployed)
    * [Removing all allure objects created](#removing-all-allure-objects-created)

## USAGE

The commands were checked using `Docker Desktop`. Adapt any definition yaml according your needs and infrastructure.

Before starting, we are going to check if we have some objects created in the namespace `allure-docker-service`

```sh
kubectl get all -o wide --namespace allure-docker-service
```

```sh
No resources found.
```

### Creating a Namespace
Check yaml definition here: [allure-namespace.yml](allure-namespace.yml)

- Create a namespace
```sh
kubectl create -f allure-namespace.yml
```
Output:
```sh
namespace/allure-docker-service created
```

- Get namespaces
```sh
kubectl get namespaces
```
Output:
```sh
NAME                    STATUS    AGE
allure-docker-service   Active    4s
```


### Creating a Config Map for API
Check yaml definition here: [allure-config-map.yml](allure-config-map.yml)

This configuration is for the API container `frankescobar/allure-docker-service`.

For this example, we will enable the security `SECURITY_ENABLED` and we will enable `TLS` option to use `https` protocol. The `URL_PREFIX` is used for accessing the API application with the prefix indicated.
```sh
TLS: "1"
SECURITY_ENABLED: "1"
URL_PREFIX: "/allure-api"
```
Note: If you enable the security is necessary to use it together with `TLS` otherwise the credentials could be exposed and your security could be vulnerable.

- Create a config map for API
```sh
kubectl create -f allure-config-map.yml
```
Output:
```sh
configmap/allure-config-map created
```

- Get configmaps
```sh
kubectl get configmaps --namespace allure-docker-service
```
Output:
```sh
NAME                DATA      AGE
allure-config-map   6         88s
```

- Describe configmap created
```sh
kubectl describe configmap allure-config-map --namespace allure-docker-service
```
Output:
```sh
Name:         allure-config-map
Namespace:    allure-docker-service
Labels:       <none>
Annotations:  <none>

Data
====
CHECK_RESULTS_EVERY_SECONDS:
----
NONE
KEEP_HISTORY:
----
0
MAKE_VIEWER_ENDPOINTS_PUBLIC:
----
1
SECURITY_ENABLED:
----
1
TLS:
----
1
URL_PREFIX:
----
/allure-api
Events:  <none>
```

### Creating a Secret for API
Check yaml definition here: [allure-secret.yml](allure-secret.yml)

For setting the `ADMIN` user/password for the API container. First we need to convert those values to `base64`. For example:
```sh
echo -n 'my_username' | base64
echo -n 'my_password' | base64
```
And we will use those encoded values in the yaml:
```sh
SECURITY_USER: bXlfdXNlcm5hbWU=
SECURITY_PASS: bXlfcGFzc3dvcmQ=
```
Also, we are going to define the `VIEWER` user/password
```sh
echo -n 'view_user' | base64
echo -n 'view_pass' | base64
```
and place them in the yaml too
```sh
SECURITY_VIEWER_USER: dmlld191c2Vy
SECURITY_VIEWER_PASS: dmlld19wYXNz
```

- Create a secret
```sh
kubectl create -f allure-secret.yml
```
Output:
```sh
secret/allure-secret created
```

- Get secrets
```sh
kubectl get secrets --namespace allure-docker-service
```
Output:
```sh
NAME                  TYPE                                  DATA      AGE
allure-secret         Opaque                                4         25s
```

### Creating a Config Map for UI
Check yaml definition here: [allure-config-ui-map.yml](allure-config-ui-map.yml)

This configuration is for the UI container `frankescobar/allure-docker-service-ui`.

For this example, the `ALLURE_DOCKER_PUBLIC_API_URL` should be a public url, remember the UI container is a `Single Page Application` and need access to a public API. In case the API use a `PREFIX` you can specify that with the `ALLURE_DOCKER_PUBLIC_API_URL_PREFIX` environment variable.

The `URL_PREFIX` is used for accessing the UI application with the prefix indicated.

```sh
ALLURE_DOCKER_PUBLIC_API_URL: "https://my-domain.com"
ALLURE_DOCKER_PUBLIC_API_URL_PREFIX: "/allure-api"
URL_PREFIX: "/allure-ui"
```

- Create a config map for UI
```sh
kubectl create -f allure-config-ui-map.yml
```
Output:
```sh
configmap/allure-config-ui-map created
```

- Get configmaps
```sh
kubectl get configmaps --namespace allure-docker-service
```
Output:
```sh
NAME                DATA      AGE
allure-config-map      5         12m
allure-config-ui-map   3         11s
```

- Describe configmap created
```sh
kubectl describe configmap allure-config-ui-map --namespace allure-docker-service
```
Output:
```sh
Name:         allure-config-ui-map
Namespace:    allure-docker-service
Labels:       <none>
Annotations:  <none>

Data
====
ALLURE_DOCKER_PUBLIC_API_URL:
----
https://my-domain.com
ALLURE_DOCKER_PUBLIC_API_URL_PREFIX:
----
/allure-api
URL_PREFIX:
----
/allure-ui
Events:  <none>
```

### Creating a Persistent Volume for API
Check yaml definition here: [allure-persistent-volume.yml](allure-persistent-volume.yml)

If you are using a Kubernetes cloud solution, surely you don't need to create a `persistent volume` from your own, due the cloud solutions provision their own persistent volumes types.

We are using `hostPath` in the yaml definition example. This is something that you only should use for development/testing purposes. A `hostPath` PersistentVolume uses a file or directory on the Node to emulate network-attached storage.

For live/production environments you should use a cloud solution like a `Google Compute Engine persistent disk` or an `Amazon Elastic Block Store volume`. Example:
```sh
    awsElasticBlockStore:
        volumeID: <volume-id>
        fsType: ext4
```
You can check other alternatives here: https://kubernetes.io/docs/concepts/storage/persistent-volumes/#types-of-persistent-volumes

- Create a persisent volume
```sh
kubectl create -f allure-persistent-volume.yml
```
Output:
```sh
persistentvolume/allure-persistent-volume created
```

- Get persistent volumes
```sh
kubectl get persistentvolumes
```
Output:
```sh
NAME                       CAPACITY   ACCESS MODES   RECLAIM POLICY   STATUS      CLAIM     STORAGECLASS   REASON    AGE
allure-persistent-volume   3Gi        RWO            Retain           Available                                      40s
```

- Describe persistent volume created
```sh
kubectl describe persistentvolumes allure-persistent-volume
```
Output:
```
Name:            allure-persistent-volume
Labels:          <none>
Annotations:     <none>
Finalizers:      [kubernetes.io/pv-protection]
StorageClass:    
Status:          Available
Claim:           
Reclaim Policy:  Retain
Access Modes:    RWO
VolumeMode:      Filesystem
Capacity:        3Gi
Node Affinity:   <none>
Message:         
Source:
    Type:          HostPath (bare host directory volume)
    Path:          /allure/projects
    HostPathType:  
Events:            <none>
```

References:
- https://kubernetes.io/docs/tasks/configure-pod-container/configure-persistent-volume-storage/#create-a-persistentvolume


### Creating a Persistent Volume Claim for API
Check yaml definition here: [allure-persistent-volume-claim.yml](allure-persistent-volume-claim.yml)

- Create a persistent volume claim
```sh
kubectl create -f allure-persistent-volume-claim.yml
```
Output:
```sh
persistentvolumeclaim/allure-persistent-volume-claim created
```

- Get persistent volume claims
```sh
kubectl get persistentvolumeclaims --namespace allure-docker-service
```
Output:
```sh
NAME                             STATUS    VOLUME                                     CAPACITY   ACCESS MODES   STORAGECLASS   AGE
allure-persistent-volume-claim   Bound     pvc-8569678e-b722-4a23-bcb8-494657f57505   3Gi        RWO            hostpath       33s
```

- Describe persistent volume claim created
```sh
kubectl describe persistentvolumeclaims allure-persistent-volume-claim --namespace allure-docker-service
```
Output:
```sh
Name:          allure-persistent-volume-claim
Namespace:     allure-docker-service
StorageClass:  hostpath
Status:        Bound
Volume:        pvc-d8590772-d0c1-48fa-9301-cfdec7f5c539
Labels:        <none>
Annotations:   pv.kubernetes.io/bind-completed=yes
               pv.kubernetes.io/bound-by-controller=yes
               volume.beta.kubernetes.io/storage-provisioner=docker.io/hostpath
Finalizers:    [kubernetes.io/pvc-protection]
Capacity:      3Gi
Access Modes:  RWO
VolumeMode:    Filesystem
Events:
  Type    Reason                 Age   From                                                                         Message
  ----    ------                 ----  ----                                                                         -------
  Normal  ExternalProvisioning   16s   persistentvolume-controller                                                  waiting for a volume to be created, either by external provisioner "docker.io/hostpath" or manually created by system administrator
  Normal  Provisioning           16s   docker.io/hostpath_storage-provisioner_5c48106f-e3e7-47ef-b82d-166da949db34  External provisioner is provisioning volume for claim "allure-docker-service/allure-persistent-volume-claim"
  Normal  ProvisioningSucceeded  16s   docker.io/hostpath_storage-provisioner_5c48106f-e3e7-47ef-b82d-166da949db34  Successfully provisioned volume pvc-d8590772-d0c1-48fa-9301-cfdec7f5c539
```

### Creating a Deployment
Check yaml definition here: [allure-deployment.yml](allure-deployment.yml)

- Create a deployment
```sh
kubectl create -f allure-deployment.yml
```
Output:
```sh
deployment.apps/allure-deployment created
```

- Get all objects
```
kubectl get all -o wide --namespace allure-docker-service
```
Output:
```
NAME                                     READY     STATUS    RESTARTS   AGE       IP           NODE             NOMINATED NODE   READINESS GATES
pod/allure-deployment-778569987f-rx782   2/2       Running   0          9s        10.1.0.179   docker-desktop   <none>           <none>

NAME                                READY     UP-TO-DATE   AVAILABLE   AGE       CONTAINERS         IMAGES                                                                               SELECTOR
deployment.apps/allure-deployment   1/1       1            1           9s        allure,allure-ui   frankescobar/allure-docker-service,frankescobar/allure-docker-service-ui   type=app

NAME                                           DESIRED   CURRENT   READY     AGE       CONTAINERS         IMAGES                                                                               SELECTOR
replicaset.apps/allure-deployment-778569987f   1         1         1         9s        allure,allure-ui   frankescobar/allure-docker-service,frankescobar/allure-docker-service-ui   pod-template-hash=778569987f,type=app
```

In this case, the pod created has this name assigned `allure-deployment-778569987f-rx782`
```sh
POD_NAME=`allure-deployment-778569987f-rx782`
```
- Check if the API application was deployed successfully in the pod

```sh
kubectl exec -it ${POD_NAME} curl http://localhost:5050 --namespace allure-docker-service
```
Output:
```sh
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Allure Docker Service</title>
  ...
</head>

<body>
...
</body>
</html>%
```

- See logs in real time
```sh
kubectl logs -f ${POD_NAME} allure --namespace allure-docker-service
```
Output:
```sh
Not checking results automatically
ALLURE_VERSION: 2.13.7
Opening existing report
[INFO] /app/allure-docker-api/app.py:143 Enabling TLS=1
[INFO] /app/allure-docker-api/app.py:157 Setting URL_PREFIX=/allure-api
[INFO] /app/allure-docker-api/app.py:172 Setting SECURITY_USER
[INFO] /app/allure-docker-api/app.py:178 Setting SECURITY_PASS
[INFO] /app/allure-docker-api/app.py:184 Setting SECURITY_VIEWER_USER
[INFO] /app/allure-docker-api/app.py:190 Setting SECURITY_VIEWER_PASS
[INFO] /app/allure-docker-api/app.py:199 Enabling Security Login. SECURITY_ENABLED=1

[INFO] /app/allure-docker-api/app.py:154 Enabling Security Login. SECURITY_ENABLED=1
2020-08-31 09:58:13.459:INFO::main: Logging initialized @2219ms to org.eclipse.jetty.util.log.StdErrLog
```


- Check if the UI application was deployed successfully in the pod

```sh
kubectl exec -it ${POD_NAME} curl http://localhost:5252 --namespace allure-docker-service
```
Output:
```sh
<!doctype html><html lang="en"><head><base href="/"/><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><link rel="shortcut icon" href="./favicon.ico"><title>Allure Docker Service UI</title><script src="./env-config.js"></script><link href="./static/css/main.2eaee7e6.chunk.css" rel="stylesheet"></head><body>...</body></html>
```

- See logs in real time
```sh
kubectl logs -f ${POD_NAME} allure-ui --namespace allure-docker-service
```
Output:
```sh
ALLURE_UI_VERSION: 7.0.1
ALLURE_DOCKER_API_URL=https://my-domain.com/allure-api/allure-docker-service
ROUTER_BASE_NAME=/allure-ui/allure-docker-service-ui
```

NOTE: Don't forget to replace the name of your pod in your case


### Creating a Service
You have 2 options for creating a service exposed for the user

#### Creating a Load Balancer Service
Check yaml definition here: [allure-service-load-balancer.yml](allure-service-load-balancer.yml)

For this example, we are going to expose the service in the port `6060`

- Create a load balancer service
```sh
kubectl create -f allure-service-load-balancer.yml
```
Output:
```sh
service/allure-service-load-balancer created
```

- Get services
```sh
kubectl get services --namespace allure-docker-service
```
Output:
```sh
NAME                           TYPE           CLUSTER-IP      EXTERNAL-IP   PORT(S)                         AGE
allure-service-load-balancer   LoadBalancer   10.105.47.254   localhost     6060:31873/TCP,7070:31103/TCP   5s
```

- Describe service created
```sh
kubectl describe service allure-service-load-balancer --namespace allure-docker-service
```
Output:
```sh
Name:                     allure-service-load-balancer
Namespace:                allure-docker-service
Labels:                   <none>
Annotations:              <none>
Selector:                 type=app
Type:                     LoadBalancer
IP:                       10.105.47.254
LoadBalancer Ingress:     localhost
Port:                     allure-api  6060/TCP
TargetPort:               5050/TCP
NodePort:                 allure-api  31873/TCP
Endpoints:                10.1.0.179:5050
Port:                     allure-ui  7070/TCP
TargetPort:               5252/TCP
NodePort:                 allure-ui  31103/TCP
Endpoints:                10.1.0.179:5252
Session Affinity:         None
External Traffic Policy:  Cluster
Events:                   <none>
```

- Verify if service for API is working properly
```sh
curl http://${IP_NODE}:6060 -ik
```
or use `localhost` in case you are in the node where the pod is deployed

```sh
curl http://localhost:6060/ -ik
```
Output:
```sh
HTTP/1.1 200 OK
Access-Control-Allow-Credentials: true
Content-Length: 1684
Content-Type: text/html; charset=utf-8
Date: Mon, 31 Aug 2020 10:09:53 GMT
Server: waitress

<!-- HTML for static distribution bundle build -->
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Allure Docker Service</title>
  ...
</head>

<body>
...
</body>
</html>%
```

- Verify if service for UI is working properly
```sh
curl http://${IP_NODE}:7070 -ik
```
or use `localhost` in case you are in the node where the pod is deployed

```sh
curl http://localhost:7070/ -ik
```
Output:
```sh
HTTP/1.1 200 OK
X-Powered-By: Express
Accept-Ranges: bytes
Cache-Control: public, max-age=0
Last-Modified: Mon, 31 Aug 2020 07:21:02 GMT
ETag: W/"807-17443640330"
Content-Type: text/html; charset=UTF-8
Content-Length: 2055
Date: Mon, 31 Aug 2020 10:12:18 GMT
Connection: keep-alive

<!doctype html><html lang="en"><head><base href="/"/><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><link rel="shortcut icon" href="./favicon.ico"><title>Allure Docker Service UI</title><script src="./env-config.js"></script><link href="./static/css/main.2eaee7e6.chunk.css" rel="stylesheet"></head><body>...</body></html>%
```

#### Creating a Node Port Service
Check yaml definition here: [allure-service-node-port.yml](allure-service-node-port.yml)

For this example, we are going to expose the service in the port `30008`

- Create a node port service
```sh
kubectl create -f allure-service-node-port.yml
```
Output:
```sh
service/allure-service-node-port created
```

- Get services
```sh
kubectl get services --namespace allure-docker-service
```
Output:
```sh
NAME                       TYPE       CLUSTER-IP     EXTERNAL-IP   PORT(S)                         AGE
allure-service-node-port   NodePort   10.104.1.236   <none>        2020:30008/TCP,3030:30009/TCP   9s
```

- Describe service created
```sh
kubectl describe service allure-service-node-port --namespace allure-docker-service
```
Output:
```sh
Name:                     allure-service-node-port
Namespace:                allure-docker-service
Labels:                   <none>
Annotations:              <none>
Selector:                 type=app
Type:                     NodePort
IP:                       10.104.1.236
LoadBalancer Ingress:     localhost
Port:                     allure-api  2020/TCP
TargetPort:               5050/TCP
NodePort:                 allure-api  30008/TCP
Endpoints:                10.1.0.180:5050
Port:                     allure-ui  3030/TCP
TargetPort:               5252/TCP
NodePort:                 allure-ui  30009/TCP
Endpoints:                10.1.0.180:5252
Session Affinity:         None
External Traffic Policy:  Cluster
Events:                   <none>
```

- Verify if service for API is working properly
```sh
curl http://${IP_NODE}:30008 -ik
```
or use `localhost` in case you are in the node where the pod is deployed
```sh
curl http://localhost:30008 -ik
```
Output:
```sh
HTTP/1.1 200 OK
Access-Control-Allow-Credentials: true
Content-Length: 1683
Content-Type: text/html; charset=utf-8
Date: Mon, 31 Aug 2020 10:37:51 GMT
Server: waitress

<!-- HTML for static distribution bundle build -->
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Allure Docker Service</title>
  ...
</head>

<body>
...
</body>
</html>%
```

- Verify if service for UI is working properly
```sh
curl http://${IP_NODE}:30009 -ik
```
or use `localhost` in case you are in the node where the pod is deployed
```sh
curl http://localhost:30009 -ik
```
Output:
```sh
HTTP/1.1 200 OK
X-Powered-By: Express
Accept-Ranges: bytes
Cache-Control: public, max-age=0
Last-Modified: Mon, 31 Aug 2020 07:21:02 GMT
ETag: W/"807-17443640330"
Content-Type: text/html; charset=UTF-8
Content-Length: 2055
Date: Mon, 31 Aug 2020 10:39:00 GMT
Connection: keep-alive

<!doctype html><html lang="en"><head><base href="/"/><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><link rel="shortcut icon" href="./favicon.ico"><title>Allure Docker Service UI</title><script src="./env-config.js"></script><link href="./static/css/main.2eaee7e6.chunk.css" rel="stylesheet"></head><body>...</body></html>%
```

### Creating a SSL certificate and key
If you have an existing certificate/key ready to use you can skip this section.

- Create a certificate/key
For this example, we are going to create a certificate/key for domain `my-domain.com`
```sh
openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout tls.key -out tls.crt -subj "/CN=my-domain.com"
```
Note: If you use `Git Bash` use double slashes at the beginning `-subj "//CN=my-domain.com"`

Output
```sh
Generating a 4096 bit RSA private key
......++
......................................................................................................................................................++
writing new private key to 'tls.key'
-----
```

### Creating a TLS Secret
- Create a TLS secret
Create TLS secret using certificate and key files generated previously
```sh
kubectl create secret tls my-domain-com-tls --cert=tls.crt --key=tls.key --namespace=allure-docker-service
```
Output:
```sh
secret/my-domain-com-tls created
```
- Get secrets
```sh
kubectl get secrets --namespace allure-docker-service
```
Output:
```sh
NAME                            TYPE                                  DATA      AGE
my-domain-com-tls           kubernetes.io/tls                          2        64s
```

### Creating an Ingress
- As pre-requisite you need to have an Ingress Controller to be able to use ingress. 
For example, you can install `ingress-nginx` --> https://kubernetes.github.io/ingress-nginx/deploy/
If you are testing in a `windows` run the yaml for Mac https://kubernetes.github.io/ingress-nginx/deploy/#docker-for-mac


- If you are using kubernetes locally, as you don't have a DNS you can edit the file `/etc/hosts`
```sh
sudo vi /etc/hosts
```
Note: If you use `Windows` the path is this one `C:\Windows\System32\drivers\etc\hosts`

then add the next line at the end
```sh
127.0.0.1   my-domain.com
```
On that way, when you request `my-domain.com` you will be re-directed to `localhost`

- Remove `TLS` section in the ingress yaml definition in case you don't want to use `https` protocol.

Reference:
- https://kubernetes.io/docs/concepts/services-networking/ingress/

#### Creating an Ingress for Load Balancer
Check yaml definition here: [allure-ingress-service-load-balancer.yml](allure-ingress-service-load-balancer.yml)

- Create an ingress
```sh
kubectl create -f allure-ingress-service-load-balancer.yml
```
Output:
```sh
ingress.networking.k8s.io/allure-ingress-service-load-balancer created
```
- Get ingress
```sh
kubectl get ingress --namespace allure-docker-service
```
Output:
```sh
NAME                                   HOSTS           ADDRESS   PORTS     AGE
allure-ingress-service-load-balancer   my-domain.com             80, 443   18s
```

- Check if ingress for API is working
```sh
curl https://my-domain.com/allure-api -ik
```
Output:
```sh
HTTP/2 200
server: nginx/1.19.0
date: Mon, 31 Aug 2020 10:22:50 GMT
content-type: text/html; charset=utf-8
content-length: 1683
vary: Accept-Encoding
access-control-allow-credentials: true
strict-transport-security: max-age=15724800; includeSubDomains

<!-- HTML for static distribution bundle build -->
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Allure Docker Service</title>
  ...
</head>

<body>
...
</body>
</html>%
```

- Check if ingress for UI is working
```sh
curl https://my-domain.com/allure-ui/allure-docker-service-ui -ik
```
Output:
```sh
HTTP/2 200
server: nginx/1.19.0
date: Mon, 31 Aug 2020 10:24:07 GMT
content-type: text/html; charset=UTF-8
content-length: 2055
vary: Accept-Encoding
x-powered-by: Express
accept-ranges: bytes
cache-control: public, max-age=0
last-modified: Mon, 31 Aug 2020 07:21:02 GMT
etag: W/"807-17443640330"
strict-transport-security: max-age=15724800; includeSubDomains

<!doctype html><html lang="en"><head><base href="/"/><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><link rel="shortcut icon" href="./favicon.ico"><title>Allure Docker Service UI</title><script src="./env-config.js"></script><link href="./static/css/main.2eaee7e6.chunk.css" rel="stylesheet"></head><body>...</body></html>%
```

#### Creating an Ingress for Node Port
Check yaml definition here: [allure-ingress-service-node-port.yml](allure-ingress-service-node-port.yml)

- Create an ingress
```sh
kubectl create -f allure-ingress-service-node-port.yml
```
Output:
```sh
ingress.networking.k8s.io/allure-ingress-service-node-port created
```
- Get ingress
```sh
kubectl get ingress --namespace allure-docker-service
```
Output:
```sh
NAME                               HOSTS           ADDRESS     PORTS     AGE
allure-ingress-service-node-port   my-domain.com   localhost   80, 443   8s
```

- Check if ingress for API is working
```sh
curl https://my-domain.com/allure-api -ik
```
Output:
```sh
HTTP/2 200
server: nginx/1.19.0
date: Mon, 31 Aug 2020 10:46:07 GMT
content-type: text/html; charset=utf-8
content-length: 1683
vary: Accept-Encoding
access-control-allow-credentials: true
strict-transport-security: max-age=15724800; includeSubDomains

<!-- HTML for static distribution bundle build -->
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Allure Docker Service</title>
  ...
</head>

<body>
...
</body>
</html>%
```

- Check if ingress for UI is working
```sh
curl https://my-domain.com/allure-ui/allure-docker-service-ui -ik
```
Output:
```sh
HTTP/2 200
server: nginx/1.19.0
date: Mon, 31 Aug 2020 10:46:57 GMT
content-type: text/html; charset=UTF-8
content-length: 2055
vary: Accept-Encoding
x-powered-by: Express
accept-ranges: bytes
cache-control: public, max-age=0
last-modified: Mon, 31 Aug 2020 07:21:02 GMT
etag: W/"807-17443640330"
strict-transport-security: max-age=15724800; includeSubDomains

<!doctype html><html lang="en"><head><base href="/"/><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><link rel="shortcut icon" href="./favicon.ico"><title>Allure Docker Service UI</title><script src="./env-config.js"></script><link href="./static/css/main.2eaee7e6.chunk.css" rel="stylesheet"></head><body>...</body></html>%
```

### Using Application Deployed
You can start using `Allure Docker Service` in Kubernetes like this:
```sh
curl https://my-domain.com/allure-api/version -ik
```
Output:
```sh
HTTP/2 200
server: nginx/1.19.0
date: Mon, 31 Aug 2020 10:24:53 GMT
content-type: application/json
content-length: 86
access-control-allow-credentials: true
strict-transport-security: max-age=15724800; includeSubDomains

{"data":{"version":"2.13.7"},"meta_data":{"message":"Version successfully obtained"}}
```
Check the Swagger Documentation in a browser wit the url: https://my-domain.com/allure-api

Check scripts https://github.com/fescobar/allure-docker-service/tree/beta#send-results-through-api where the `allure server url` would be `https://my-domain.com` and the prefix `/allure-api`


Also, you can start using `Allure Docker Service UI` opening a browser this url: https://my-domain.com/allure-ui/allure-docker-service-ui


### Removing all allure objects created
- Execute bash script [delete-allure-k8s-objects.sh](delete-allure-k8s-objects.sh)
```sh
./delete-allure-k8s-objects.sh
```
