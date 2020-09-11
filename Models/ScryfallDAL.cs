using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagicTheGatheringFinal.Models
{
    public class ScryfallDAL
    {
        public HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.scryfall.com");
            //URI - uniform resource identifier
            return client;
        }

        public async Task<Cardobject> GetCard(string input)
        {
            var client = GetClient(); //calls the method that gives the API the general information needed to 
            //receive data from the API 
            var inputQuery = MakeQuery(input);
            var response = await client.GetAsync($"/named?fuzzy={inputQuery}"); //uses the client (HTTPClient) to receive 
            //data from the API based off of a certain endpoint.
            Cardobject card = await response.Content.ReadAsAsync<Cardobject>();
            //install-package Microsoft.AspNet.WebAPI.Client
            //response has a property called Content and Content has a method that reads the JSON and plugs it into a specified
            //obect. If the JSON does not fit within the object we get an Internal Deserialization error
            return card;
        }

        public string MakeQuery(string input)
        {
            //splits query into words separated by +, as this is the format for the api queries.
            string[] inputs = input.Split(" ");
            string query = "";
            foreach(string word in inputs)
            {
                query += word + "+";
            }

            //final interation of for loop leaves + on end of string, so returning a substring without the last +
            query = query.Substring(0, query.Length - 1);
            return query;
        }

    }
}
