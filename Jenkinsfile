pipeline {
    agent any

    environment {
        UNITY_EXECUTABLE = '/Applications/Unity/Hub/Editor/2021.3.7f1/Unity.app/Contents/MacOS/Unity'
        PROJECT_PATH = '/Users/salin/.jenkins/workspace/JenkinsUnityWebGlTest'
        BUILD_SCRIPT_NAME = 'WebGlBuildScript'
        AWS_CLI_EXECUTABLE = 'aws'
        S3_BUCKET_NAME = 'jenkinsbucket-salin'
        UNITY_BUILD_VERSION = "${BUILD_NUMBER}"
    }

    stages {
        stage('Git Pull') {
            steps {
                script {
                    sh "git pull origin main"  // Adjust the branch as needed
                }
            }
        }
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Unity Build') {
            steps {
                script {
                    // Run Unity build script
                    sh "${UNITY_EXECUTABLE} -batchmode -projectPath ${PROJECT_PATH} -executeMethod ${BUILD_SCRIPT_NAME}.Build -quit"
                }
            }
        }

        stage('Upload Addressables to S3') {
            steps {
                script {
                    // Upload Addressables build (including catalog) to S3
                    sh "${AWS_CLI_EXECUTABLE} s3 sync ${PROJECT_PATH}/ServerData/StandaloneOSX/ s3://${S3_BUCKET_NAME}/${UNITY_BUILD_VERSION}/Addressables/"


                }
            }
        }

        stage('Upload WebGL Build to S3') {
            steps {
                script {
                    // Upload WebGL build to S3
                    sh "${AWS_CLI_EXECUTABLE} s3 sync ${PROJECT_PATH}/Builds/ s3://${S3_BUCKET_NAME}/${UNITY_BUILD_VERSION}/WebGL/"

                }
            }
        }

        stage('Generate WebGL Build URL') {
            steps {
                script {
                    // Generate a shareable URL for the WebGL build
                    def webGLBuildURL = "https://${S3_BUCKET_NAME}.s3.amazonaws.com/${UNITY_BUILD_VERSION}/WebGL/index.html"
                    echo "WebGL Build URL: ${webGLBuildURL}"

                    // You can save the URL to a file or use it as needed
                    writeFile file: 'webgl_build_url.txt', text: webGLBuildURL
                }
            }
        }
    }

    post {
        success {
            echo 'Build, upload, and URL generation successful!'
        }
        failure {
            echo 'Build, upload, or URL generation failed. Check the logs for details.'
        }
    }
}
