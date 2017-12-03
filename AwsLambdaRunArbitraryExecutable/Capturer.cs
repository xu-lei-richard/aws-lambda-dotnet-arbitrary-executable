using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;

namespace AwsLambdaRunArbitraryExecutable
{
    public class Capturer
    {
        private const string phantomjsPath = "/tmp/phantomjs";
        private readonly ILogger _logger;
        private readonly string _s3Bucket;
        private readonly IAmazonS3 _s3Client;
        private readonly string capturejsPath;

        public Capturer(ILogger logger, IAmazonS3 s3Client, string s3bucket)
        {
            _logger = logger;
            _s3Client = s3Client;
            _s3Bucket = s3bucket;

            capturejsPath = new FileInfo("rasterize.js").FullName;

            // setup PhantomJS
            RunProcess("/bin/bash", "unix-setup-phantomjs",
                out var exitCode, out var output, out var err);

            _logger?.LogInfo($"Setup PhantomJS. ExitCode {exitCode}. Output: {output}");

            if (!string.IsNullOrWhiteSpace(err))
                _logger?.LogError($"Setup PhantomJS with error: {err}");

            // check phantomjs has been setup properly
            var phantomjs = new FileInfo(phantomjsPath);

            if (!phantomjs.Exists) _logger?.LogError($"Couldn't locate {phantomjsPath}");
        }

        public async Task Capture(string target, string filename)
        {
            try
            {
                // check input target
                if (string.IsNullOrWhiteSpace(target)) return;

                if (string.IsNullOrWhiteSpace(filename)) filename = Guid.NewGuid() + ".jpeg";

                var filePath = "/tmp/" + filename; // Lambda has a read-only file system, but can write to tmp folder

                RunProcess(phantomjsPath, $"{capturejsPath} {target} {filePath}",
                    out var exitCode, out var output, out var err);

                var fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists)
                {
                    _logger?.LogError(
                        $"PhantomJS failed to generate. ExitCode {exitCode}. Output: {output}. Err: {err}");
                    return;
                }

                // store in s3
                await _s3Client.UploadObjectFromFilePathAsync(_s3Bucket, filename, fileInfo.FullName, null);

                // remove it to save space, in case of many times of container reuse.
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                _logger?.LogError("Exception to generate", ex);
            }
        }

        private static void RunProcess(string processFileName, string arguments,
            out int exitCode, out string output, out string err)
        {
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = processFileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                proc.Start();

                output = proc.StandardOutput.ReadToEnd();
                err = proc.StandardError.ReadToEnd();

                proc.WaitForExit();

                exitCode = proc.ExitCode;
            }
        }
    }
}