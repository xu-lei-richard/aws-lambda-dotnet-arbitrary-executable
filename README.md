# AWS Lambda Run Arbitrary Executable with .NET Core

AWS Lambda allows running arbitrary executables. This [blog](https://aws.amazon.com/blogs/compute/running-executables-in-aws-lambda/) shows how to do it with Node.js. We needed to do it in .NET Core, and there is no clear instruction on how to implement it. This project shows how to run arbitrary executables with .NET Core.

## Update the S3 Bucket

In the file [build.bat](AwsLambdaRunArbitraryExecutable/build.bat), update the existing bucket name `aws-code-store` to your own S3 bucket.

## Project Goal

[PhantomJS](http://phantomjs.org/) is a popular tool to [capture screen](http://phantomjs.org/screen-capture.html). It is an executable. This project creates an AWS Lambda function with .NET Core to execute PhantomJS to capture a web page, and then saves the image to a S3 bucket.

## Run the Project

1. Assume your AWS credentials have been configured.

2. Run the below batch command under the folder `AwsLambdaRunArbitraryExecutable`. Note: Update its S3 bucket as described above.

```
build.bat
```

Its output is [here](images/build-bat-output.png).

3. Run the below batch command under the folder `AwsLambdaRunArbitraryExecutable` to create the Lambda function through CloudFormation.

```
deploy.bat
```

Its output is [here](images/deploy-bat-output.png).

4. Enter into AWS Console, and the newly created Lambda function should be seen. Test with the below Test Event:

```
{
  "target": "https://dmitrybaranovskiy.github.io/raphael/polar-clock.html",
  "filename": "clock.jpeg"
}
```

Its output is [here](images/lambda-execution-output.png).

5. In the above CloudFormation stack creation, a S3 bucket is created to store the created image file name. Locate the S3 bucket name in the stack's outputs. In that S3 bucket, the file clock.jpeg should present.

## Key Files

[Function.cs](AwsLambdaRunArbitraryExecutable/Function.cs): Entry point of the Lambda function.

[Capturer.cs](AwsLambdaRunArbitraryExecutable/Capturer.cs): The class to setup PhantomJS, and execute PhantomJS to capture a given page, and saves into S3 bucket.

[Logger.cs](AwsLambdaRunArbitraryExecutable/Logger.cs): A very simple logger for AWS Lambda.

[function.yaml](AwsLambdaRunArbitraryExecutable/function.yaml): CloudFormation script to create the Lambda function.

[phantomjs](AwsLambdaRunArbitraryExecutable/phantomjs): PhantomJS executable.

[rasterize.js](AwsLambdaRunArbitraryExecutable/rasterize.js): A PhantomJS sample script to capture screen.

[unix-setup-phantomjs](AwsLambdaRunArbitraryExecutable/unix-setup-phantomjs): PhantomJS setup script in Unix file format.


## Key Points

### PhantomJS executable version: 
phantomjs-1.9.8-linux-x86_64 is used. The underlying [Lambda Execution Environment](https://docs.aws.amazon.com/lambda/latest/dg/current-supported-versions.html) is based on Amazon Linux AMI, and only 64-bit binaries are supported on AWS Lambda.

### Executable configuration
AWS Lambda is based on Linux, and it is different world to Windows programs. Linux file mode is not applicable for Windows. When an axecutable is packaged on Windows machine and uploaded to AWS Lambda, it is just a plain file. AWS Lambda also has a read-only file system. The key of executable configuration is with the file [unix-setup-phantomjs](AwsLambdaRunArbitraryExecutable/unix-setup-phantomjs). It copies the executable to /tmp folder, and then change its file mode to make it executable.
	
Note 1: When the package is uploaded to AWS Lambda, it is stored under /var/task, which is stored in the Lambda Environment Variable LAMBDA_TASK_ROOT.
	
Note 2: The setup script unix-setup-phantomjs must be UNIX format. [.gitattributes](.gitattributes) helps to enforce that in a Windows environment.

### Included files for AWS Lambda Package
The below files must set "Copy to Output Directory" to "Copy Always"
- phantomjs
- rasterize.js
- unix-setup-phantomjs

### Excluded files for AWS Lambda Package
All *.bat / *.yaml files must set "Copy to Output Directory" to "Do not copy".
