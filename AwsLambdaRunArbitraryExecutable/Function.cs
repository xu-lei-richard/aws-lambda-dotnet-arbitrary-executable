using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace AwsLambdaRunArbitraryExecutable
{
    /// <summary>
    ///     Entry point for AWS Lambda Function
    /// </summary>
    public class Function
    {
        private readonly Capturer _capturer;

        public Function()
        {
            // configure the S3 bucket to store generated image file.
            // This bucket is created through CloudFormation, and then configures as environment variable
            var s3bucket = Environment.GetEnvironmentVariable("S3bucket");

            _capturer = new Capturer(new Logger(), new AmazonS3Client(), s3bucket);
        }

        /// <summary>
        ///     Lambda Function Handler
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Capture(JObject input, ILambdaContext context)
        {
            var target = input["target"].ToString();
            var filename = input["filename"].ToString();

            await _capturer.Capture(target, filename);
        }
    }
}