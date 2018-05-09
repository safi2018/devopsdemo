pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '"C:\\Nuget\\nuget.exe" restore I95Dev.Connector.UI.sln'
      }
    }
  }
}