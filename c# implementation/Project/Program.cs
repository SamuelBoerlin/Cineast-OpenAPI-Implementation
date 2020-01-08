using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cineast_OpenAPI_Implementation
{
    class Program
    {


        static void Main(string[] args)
        {
            Task.Run(async () => await Run()).GetAwaiter().GetResult();
        }

        private static async Task Run()
        {
            Console.WriteLine("Start API");

            var baseApiUrl = "http://192.168.56.101:4567/";
            var baseFileUrl = "http://192.168.56.101:8000/";

            Configuration.Default.BasePath = baseApiUrl;

            var downloader = new CineastObjectDownloader
            {
                HostBaseUrl = baseFileUrl,
                HostObjectsPath = "data/3d/:p",
                HostThumbnailsPath = "thumbnails/:o/:s:x",
                UseCineastServer = false //Cineast's thumbnail resolver is currently broken
            };

            var api = new Apiv1Api();

            var categories = new List<string>
            {
                "sphericalharmonicshigh"
            };

            var testModelJson = "";
            using (FileStream fs = File.OpenRead("../../cube.obj"))
            {
                testModelJson = ObjToJsonConverter.Convert(fs);
            }

            var testModelData = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(testModelJson));

            var terms = new List<QueryTerm>
            {
                new QueryTerm(categories, QueryTerm.TypeEnum.MODEL3D, "data:application/3d-json," + testModelData)
            };

            var components = new List<QueryComponent>
            {
                new QueryComponent(terms)
            };

            var conf = new QueryConfig();

            Console.WriteLine("Start Similarity Query Request");

            //Execute a similarity query
            var response = api.ApiV1FindSegmentsSimilarPost(new SimilarityQuery(components, conf));

            Console.WriteLine("Finished Similarity Query Request");

            Console.WriteLine("Results:");
            Console.WriteLine();

            foreach (var similarityResult in response.Results)
            {
                foreach (var similarityContent in similarityResult.Content)
                {
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Segment ID: " + similarityContent.Key + ", Similarity Score: " + similarityContent.Value);

                    var segmentsResult = api.ApiV1FindSegmentsByIdPost(new IdList(new List<string>() { similarityContent.Key }));

                    foreach (var segmentContent in segmentsResult.Content)
                    {
                        Console.WriteLine("Object ID: " + segmentContent.ObjectId);

                        var mediaObjectResult = api.ApiV1FindObjectByAttributeValueGet("id", segmentContent.ObjectId);
                        foreach (var mediaObjectContent in mediaObjectResult.Content)
                        {
                            Console.WriteLine("Object Data: " + mediaObjectContent);

                            using (var stream = await downloader.RequestObjectAsync(api, mediaObjectContent, segmentContent))
                            using (var reader = new StreamReader(stream))
                            {

                                string text = reader.ReadToEnd();

                                Console.WriteLine("Downloaded Object: ");

                                var lines = text.Split('\n');
                                int maxLines = Math.Min(lines.Length, 8);
                                for (int i = 0; i < maxLines; i++)
                                {
                                    Console.WriteLine(lines[i]);
                                }
                                Console.WriteLine("...");
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
