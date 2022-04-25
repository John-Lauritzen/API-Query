using System;
using System.Net;
using System.Collections.Generic;
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
            public int MalID { get; set; }
            public string RomTitle { get; set; }
            public string EngTitle { get; set; }
            public List<string> Genres { get; set; }

            public MangaDetails()
            {
                Genres = new List<string>();
            }

            public void GetDetails()
            {
                Console.WriteLine("MAL ID: " + MalID);
                Console.WriteLine("Romanji Title: " + RomTitle);
                Console.WriteLine("English Title: " + EngTitle);
                Console.Write("Tags: ");
                foreach (var genre in Genres)
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
            manga1.MalID = malID;
            manga1.RomTitle = malDData.SelectToken("title").Value<string>();
            manga1.EngTitle = malDData.SelectToken("alternative_titles").SelectToken("en").Value<string>();
            //Add each genre name to object list
            foreach (var result in malDData.SelectToken("genres"))
            {
                manga1.Genres.Add(result.SelectToken("name").Value<string>());
            }
            //Write contents
            manga1.GetDetails();

        }
    }
}
