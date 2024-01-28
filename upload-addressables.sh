#!/bin/bash

# Script to build Unity Addressables and upload to AWS S3

# Check if the correct number of arguments is provided
if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <S3_BUCKET_NAME>"
    exit 1
fi

# Assign arguments to variables
S3_BUCKET_NAME=$1
UNITY_PATH="/Applications/Unity/Hub/Editor/2021.3.7f1/Unity.app"
PROJECT_PATH="/Desktop/SVN/JenkinsWebGlTest"
BUILD_SCRIPT_NAME="BuildWebGl"
ADDRESSABLES_PROFILE="Remote"  

# Get Unity build version
UNITY_BUILD_VERSION=$($UNITY_PATH/Contents/MacOS/Unity -version | grep "Revision" | awk '{print $3}')

# Create a folder with Unity build version in S3 bucket
UNITY_FOLDER_NAME="unity_build_${UNITY_BUILD_VERSION}"
S3_UPLOAD_PATH="s3://$S3_BUCKET_NAME/$UNITY_FOLDER_NAME/Addressables"

# Build Unity Addressables
echo "Building Unity Addressables..."
$UNITY_PATH/Contents/MacOS/Unity -batchmode -projectPath $PROJECT_PATH -executeMethod UnityEditor.AddressableAssets.Settings.AddressablesBuildAll

# Upload Addressables to AWS S3
echo "Uploading Addressables to AWS S3..."
aws s3 sync $PROJECT_PATH/Library/com.unity.addressables/AddressablesBuildResults $S3_UPLOAD_PATH

echo "Upload complete."

