REM Deploy the packaged CloudFormation script to create the Lambda Function
aws cloudformation deploy --stack-name AwsLambdaRunArbitraryExecutableStack --template-file function-packaged.yaml --capabilities CAPABILITY_IAM
