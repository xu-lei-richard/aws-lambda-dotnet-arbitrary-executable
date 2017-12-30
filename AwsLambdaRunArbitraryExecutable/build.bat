REM 1. Use dotnet CLI to package the project into a zip file
dotnet lambda package publish\AwsLambdaRunArbitraryExecutable.zip -c Release -f netcoreapp1.1

REM ** Update the below bucket "aws-code-store" to your own S3 bucket
REM 2. Package the CloudFormation script
aws cloudformation package --template-file function.yaml --s3-bucket aws-code-store --s3-prefix AwsLambdaRunArbitraryExecutable --output-template-file function-packaged.yaml
