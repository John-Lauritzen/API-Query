using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace API_Query
{
    class Program
    {
        static StreamReader MALConnection(string URL){
            //Create Query
            WebRequest malQuery;
            malQuery = WebRequest.Create(URL);
            //Attach API Key for access
            malQuery.Headers["X-MAL-CLIENT-ID"] = "8e63b628fd74b0bab02e703f52e79743";
            //Match system proxy
            malQuery.Proxy = WebRequest.GetSystemWebProxy();
            //Create stream to hold query result
            Stream malStream;
            malStream = malQuery.GetResponse().GetResponseStream();
            //Create Reader
            StreamReader malReader;
            malReader = new(malStream);
            return malReader;
        }

        class MangaDetails
        {
            public int malID { get; set; }
            public string romTitle { get; set; }
            public string engTitle { get; set; }
            public List<string> genres { get; set; }

            public void GetDetails()
            {
                Console.WriteLine("MAL ID: " + malID);
                Console.WriteLine("Romanji Title: " + romTitle);
                Console.WriteLine("English Title: " + engTitle);
                Console.Write("Tags: ");
                foreach (var genre in genres)
                {
                    Console.Write(genre + " ");
                }
                Console.WriteLine();
            }
        }
        
        static void Main(string[] args)
        {
            string searchTitle;
            Console.WriteLine("Enter the name of the Manga to search for:");
            searchTitle = Console.ReadLine();
            //Create initial query URL based on input
            string malURL = "https://api.myanimelist.net/v2/manga?q=" + searchTitle;
            //Run Query
            StreamReader malQReader = MALConnection(malURL);
            //Parse JSON data
            var malQData = JObject.Parse(malQReader.ReadToEnd());
            //Get first title returned
            string malTitle = malQData.SelectToken("data").First.First.First.SelectToken("title").Value<string>();
            //Get ID for first title returned
            int malID = malQData.SelectToken("data").First.First.First.SelectToken("id").Value<int>();
            Console.WriteLine("First result retured is: " + malTitle);
            //Create details query URL based on previous ID
            string malDetailsURL = "https://api.myanimelist.net/v2/manga/" + malID + "?fields=id,title,alternative_titles,genres";
            //Run Query
            StreamReader malDReader = MALConnection(malDetailsURL);
            //Parse JSON data
            var malDData = JObject.Parse(malDReader.ReadToEnd());
            //Create new manga object
            MangaDetails manga1 = new MangaDetails();
            //Assign values
            manga1.malID = malID;
            manga1.romTitle = malDData.SelectToken("title").Value<string>();
            manga1.engTitle = malDData.SelectToken("alternative_titles").SelectToken("en").Value<string>();
            //Create temp list for breaking out genres
            var tempGenres = new List<string>();
            //Add each genre name to temp list
            foreach (var result in malDData.SelectToken("genres"))
            {
                tempGenres.Add(result.SelectToken("name").Value<string>());
            }
            //Assign manga object with the completed list
            manga1.genres = tempGenres;
            //Write contents
            manga1.GetDetails();

        }
    }
}
