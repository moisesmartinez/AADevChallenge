using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace AADevChallenge
{
    class Program
    {
        const string APIUrl = "http://internal-devchallenge-2-dev.apphb.com";
        const string getValuesURL = APIUrl + "/values/{0}"; //0 = Guid
        const string getEncodeValueURL = APIUrl + "/encoded/{0}/{1}"; //0 = Guid, 1 = Algorithm Name
        const string postEncodedValueURL = APIUrl + "/values/{0}/{1}"; //0 = Guid, 1 = Algorithm Name

        static void Main(string[] args)
        {
            bool isChallengeCompleted = false;
            List<string> statusMessages = new List<string>();

            //Do the following 20 times
            for (int i = 0; i < 20; i++)
            {
                //Generate Guid
                string guid = Guid.NewGuid().ToString();

                //1.a: Get values
                string jsonWords = RequestValues(guid);
                GetValuesResponse values = JsonConvert.DeserializeObject<GetValuesResponse>(jsonWords);

                //1.b Run algorithm
                string myEncodedValue = DoAlgorithm(values.algorithm, values.words, values.startingFibonacciNumber);

                //1.c Post the calculated value into the API
                string jsonPostResponse = PostEncodedValueToAPI(guid, myEncodedValue, values.algorithm);
                PostEncodedValueResponse postResponse = JsonConvert.DeserializeObject<PostEncodedValueResponse>(jsonPostResponse);

                statusMessages.Add(postResponse.status);

                if (postResponse.status.ToUpper() == "CRASHANDBURN")
                {
                    Console.WriteLine("I lost...");
                    Console.ReadLine();
                    return;
                }

                if (postResponse.status.ToUpper() == "WINNER")
                {
                    isChallengeCompleted = true;
                }
            }

            if (isChallengeCompleted)
            {
                Console.WriteLine("Winner!");
            }
            else
            {
                Console.WriteLine("Apparently, I did not completed the challenge...");
            }
            Console.ReadLine();
            return;

        }

        //Gets the list of words, fibonacci number and algorithm name from the API.
        private static string RequestValues(string guid)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(getValuesURL, guid));
            request.Method = "GET";
            request.Accept = "application/json";
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        //Gets the enconded string for a given guid and algorithm name (for testing purposes)
        private static string GetEncodedValues(string guid, string algorithm)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(getEncodeValueURL, guid, algorithm));
            request.Method = "GET";
            request.Accept = "application/json";
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        //POST json data to value
        private static string PostEncodedValueToAPI(string guid, string encodedValue, string algorithmName)
        {
            PostEncodedValueRequest postValues = new PostEncodedValueRequest();
            postValues.emailAddress = "moises.martinezhn@gmail.com";
            postValues.encodedValue = encodedValue;
            postValues.name = "Moises David Martinez Padilla";
            postValues.repoUrl = "https://github.com/moisesmartinez/AADevChallenge";
            postValues.webhookUrl = "http://aawebhookhandler.apphb.com/WebhookHandler";

            //Create json string
            string jsonContent = JsonConvert.SerializeObject(postValues);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(postEncodedValueURL, guid, algorithmName));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                // Log exception and throw as for GET example above
                throw;
            }
        }




        #region Algorithms

        private static string DoAlgorithm(string algorithmName, string[] words, double startingFibonacciNumber)
        {
            string base64 = "";

            switch (algorithmName.ToUpper())
            {
                case "IRONMAN":
                    base64 = IronMan(words);
                    break;
                case "THEINCREDIBLEHULK":
                    base64 = TheIncredibleHulk(words);
                    break;
                case "THOR":
                    base64 = Thor(words, startingFibonacciNumber);
                    break;
                case "CAPTAINAMERICA":
                    base64 = CaptainAmerica(words, startingFibonacciNumber);
                    break;
            }

            return base64;
        }

        private static string IronMan(string[] values)
        {
            string base64 = "";

            //Put values in alphabetic order
            values = UtilityClass.AlphabeticSort(values);

            //Shift vowels to the right
            values = UtilityClass.ShiftVowels(values);

            //Concatenate values into one string, using ASCII
            string concatenatedString = UtilityClass.ConcatenateWords(values);

            //Encode
            base64 = UtilityClass.StringToBase64String(concatenatedString);
            return base64;
        }


        private static string TheIncredibleHulk(string[] values)
        {
            string base64 = "";

            //Shift vowels to the right
            values = UtilityClass.ShiftVowels(values);

            //Sort in reverse
            values = UtilityClass.ReverseAlphabeticSort(values);

            //Concatenate using asteriks
            string concatenatedString = UtilityClass.ConcatenateWordsWithAsterik(values);

            //Encode
            base64 = UtilityClass.StringToBase64String(concatenatedString);
            return base64;
        }


        private static string Thor(string[] values, double startingFibonacciNumber)
        {
            string base64 = "";
            
            //Split english words
            values = UtilityClass.SplitEnglishWords(values);

            //Put values in alphabetic order
            values = UtilityClass.AlphabeticSort(values);

            //Alternate consonants
            values = UtilityClass.AlternateConsonants(values);

            //Replace vowels with Fibonacci
            values = UtilityClass.ReplaceVowelsFibonacci(values, startingFibonacciNumber);

            //Concatenate using asterisks
            string concatenatedString = UtilityClass.ConcatenateWordsWithAsterik(values);

            //Encode
            base64 = UtilityClass.StringToBase64String(concatenatedString);
            return base64;
        }

        private static string CaptainAmerica(string[] values, double startingFibonacciNumber)
        {
            string base64 = "";

            //Shift vowels to the right
            values = UtilityClass.ShiftVowels(values);

            //Sort in reverse
            values = UtilityClass.ReverseAlphabeticSort(values);

            //Replace vowels with Fibonacci
            values = UtilityClass.ReplaceVowelsFibonacci(values, startingFibonacciNumber);

            //Concatenate values into one string, using ASCII
            string concatenatedString = UtilityClass.ConcatenateWords(values);

            //Encode
            base64 = UtilityClass.StringToBase64String(concatenatedString);
            return base64;
        }

        #endregion

    }

    internal class GetValuesResponse
    {
        public string[] words { get; set; }
        public double startingFibonacciNumber { get; set; }
        public string algorithm { get; set; }
    }

    internal class GetEncodedResponse
    {
        public string encoded { get; set; }
    }

    internal class PostEncodedValueRequest
    {
        public string encodedValue { get; set; }
        public string emailAddress { get; set; }
        public string name { get; set; }
        public string webhookUrl { get; set; }
        public string repoUrl { get; set; }
    }

    internal class PostEncodedValueResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}
