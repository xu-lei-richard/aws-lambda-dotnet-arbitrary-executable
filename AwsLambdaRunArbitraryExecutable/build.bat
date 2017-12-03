dotnet lambda package publish\AwsLambdaRunArbitraryExecutable.zip -c Release -f netcoreapp1.1

aws cloudformation package --template-file function.yaml --s3-bucket aws-code-store --s3-prefix AwsLambdaRunArbitraryExecutable --output-template-file function-packaged.yaml

aws cloudformation deploy --stack-name AwsLambdaRunArbitraryExecutableStack --template-file function-packaged.yaml --capabilities CAPABILITY_IAM
