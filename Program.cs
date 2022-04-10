using System;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace API_Query
{
    class Program
    {
        static void Main(string[] args)
        {
            string searchTitle;
            Console.WriteLine("Enter the name of the Manga to search for:");
            searchTitle = Console.ReadLine();
            //Create initial query URL based on input
            string malURL = "https://api.myanimelist.net/v2/manga?q=" + searchTitle;
            WebRequest malQuery;
            malQuery = WebRequest.Create(malURL);
            //Attach API Key for access
            malQuery.Headers["X-MAL-CLIENT-ID"] = "8e63b628fd74b0bab02e703f52e79743";
            //Match system proxy
            malQuery.Proxy = WebRequest.GetSystemWebProxy();
            //Create stream to hold query result
            Stream malQStream;
            malQStream = malQuery.GetResponse().GetResponseStream();
            StreamReader malQReader = new(malQStream);
            //Parse JSON data
            var malQData = JObject.Parse(malQReader.ReadToEnd());
            //Get first title returned
            string malTitle = malQData.SelectToken("data").First.First.First.SelectToken("title").Value<string>();
            //Get ID for first title returned
            int malID = malQData.SelectToken("data").First.First.First.SelectToken("id").Value<int>();
            Console.WriteLine("First result retured is: " + malTitle);
            //Create details query URL based on previous ID
            string malDetailsURL = "https://api.myanimelist.net/v2/manga/" + malID + "?fields=id,title,genres";
            WebRequest malDetails;
            malDetails = WebRequest.Create(malDetailsURL);
            //Attach API Key for access
            malDetails.Headers["X-MAL-CLIENT-ID"] = "8e63b628fd74b0bab02e703f52e79743";
            //Match system proxy
            malDetails.Proxy = WebRequest.GetSystemWebProxy();
            //Create stream to hold details result
            Stream malDStream;
            malDStream = malDetails.GetResponse().GetResponseStream();
            StreamReader malDReader = new(malDStream);
            //Parse JSON data
            var malDData = JObject.Parse(malDReader.ReadToEnd());
            Console.WriteLine(malDData);
        }
    }
}
