# PYTHON PYTEST DEMO PROJECT USING ALLURE

## INSTALLATION
- Download Python 3
https://www.python.org/downloads/

- Check Python version
```sh
python3 --version
```
- Go to project
- Install dependencies

```sh
pip3 install -r requirements.txt
 ```

## USAGE
Execute Allure Docker Service from this directory
```sh
docker-compose up -d allure allure-ui
```

- Verify if Allure API is working. Go to -> http://localhost:5050/allure-docker-service/latest-report

- Verify if Allure UI is working. Go to -> http://localhost:5252/allure-docker-service-ui/

Each time you run tests, the Allure report will be updated.
Execute tests:
```sh
pytest tests/*.py --alluredir=allure-results
```

See documentation here:
- https://github.com/fescobar/allure-docker-service
- https://github.com/fescobar/allure-docker-service-ui

## DEVELOPER USAGE
Freeze dependencies when you update some of them
```sh
pip freeze > requirements.txt
```
https://blog.miguelgrinberg.com/post/the-package-dependency-blues
