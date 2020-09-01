# NODEJS TYPESCRIPT MOCHA DEMO PROJECT USING ALLURE

## INSTALLATION
- Download Node JS
https://nodejs.org/en/download/

- Check NPM version
```sh
npm -version
```

- Check NodeJS version
```sh
node -v
```

- Go to project
- Install dependencies

```sh
 npm install
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
 npm run-script allure-test
 ```

  NOTE FOR WINDOWS:
 - There are some scripts using `Unix` commands like `rm`. We recommend you to install `Git Bash for Windows` https://git-scm.com/downloads, on that console you can run any unix command. Otherwise, adapt the script with commands for Windows.

See documentation here:
- https://github.com/fescobar/allure-docker-service
- https://github.com/fescobar/allure-docker-service-ui
