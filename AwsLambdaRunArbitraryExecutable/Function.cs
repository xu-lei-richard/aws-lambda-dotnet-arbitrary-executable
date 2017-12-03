using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace AwsLambdaRunArbitraryExecutable
{
    public class Function
    {
        private readonly Capturer _capturer;

        public Function()
        {
            var s3bucket = Environment.GetEnvironmentVariable("S3bucket");

            _capturer = new Capturer(new Logger(), new AmazonS3Client(), s3bucket);
        }

        public async Task Capture(ILambdaContext context, string target, string filename)
        {
            await _capturer.Capture(target, filename);
        }
    }
}