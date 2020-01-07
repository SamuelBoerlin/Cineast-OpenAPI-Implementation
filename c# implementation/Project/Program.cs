using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cineast_OpenAPI_Implementation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start API");

            Configuration.Default.BasePath = "http://192.168.56.101:4567/";

            Apiv1Api api = new Apiv1Api();

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
                    Console.WriteLine("Object ID: " + similarityContent.Key + ", Similarity Score: " + similarityContent.Value);

                    //TODO Not sure what the "_000001" appended after the returned object ID is, but this API call only works with it removed..
                    var mediaObjectResult = api.ApiV1FindObjectByAttributeValueGet("id", similarityContent.Key.Substring(0, similarityContent.Key.Length - 7));
                    foreach (var mediaObjectContent in mediaObjectResult.Content)
                    {
                        Console.WriteLine("Object Data: " + mediaObjectContent);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
