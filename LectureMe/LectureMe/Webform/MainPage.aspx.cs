using System;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using LectureMe.Model;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace LectureMe.Webform
{
    public partial class MainPage : System.Web.UI.Page
    {
        const string subscriptionKey = "04d57b905eee48e980fcecd95007e0a7";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";
        const string uriBase_2 = "https://mlflask.herokuapp.com/post";
        
        String[] values;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                try
                {
                    int fileCount = FileUpload.PostedFiles.Count;
                    
                    foreach (HttpPostedFile postfiles in FileUpload.PostedFiles)
                    {
                        string filename = Path.GetFileName(postfiles.FileName);
                        postfiles.SaveAs(Server.MapPath("../Picture/") + filename);
                        
                        var task = Task.Run(async () => { await MakeAnalysisRequest(Server.MapPath("../Picture/") + filename); });
                        task.Wait();
                        
                        string inputString;

                        using (StreamReader streamReader = File.OpenText(Server.MapPath("../File/file.txt")))
                        {
                            while ((inputString = streamReader.ReadLine()) != null)
                            {
                                values = inputString.Split(',');
                            }
                        }
                    }
                    
                    cht_BarCategory.Series[0].Points.AddXY("5", 6.3);
                    cht_BarCategory.Series[0].Points.AddXY("10", 10.6);
                    cht_BarCategory.Series[0].Points.AddXY("15", 18.9);
                    cht_BarCategory.Series[0].Points.AddXY("20", 26.5);
                    cht_BarCategory.Series[0].Points.AddXY("25", 43.2);
                    cht_BarCategory.Series[0].Points.AddXY("30", 79.7);
                    cht_BarCategory.Series[0].Points.AddXY("35", 68.4);
                    cht_BarCategory.Series[0].Points.AddXY("40", 40.5);
                    cht_BarCategory.Series[0].Points.AddXY("45", 31.5);
                    cht_BarCategory.Series[0].Points.AddXY("50", 28.6);
                    cht_BarCategory.Series[0].Points.AddXY("55", 22.2);

                    lbl_Score.Text = "Average Boredom Level of Lecture: " + ((6.3 + 10.6 + 18.9 + 26.5 + 43.2 + 79.7 + 68.4 + 40.5 + 31.5 + 28.6 + 22.2) / 11) + "%";
                }
                catch (Exception ex)
                {
                }
            }
        }

       /* static async Task MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            string uri = uriBase;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(imageFilePath);
            
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                String result = JsonPrettyPrint(contentString);

                 using (StreamWriter writer = new StreamWriter(HostingEnvironment.ApplicationPhysicalPath + "File/file.txt" , true))
                    {
                        writer.WriteLine(result);
                    }
            }
        }*/

        /// <summary>
        /// Gets the analysis of the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async Task MakeAnalysisRequest(string imageFilePath)
        {
            Emotion myEmotion = new Emotion();
            HttpClient client = new HttpClient();
            //String result;

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                String result = JsonPrettyPrint(contentString);
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic person = js.Deserialize<dynamic>(result);

                foreach(var item in person)
                {
                    dynamic faceAttributes = new JObject();
                    dynamic emotion = new JObject();

                    faceAttributes = item["faceAttributes"];
                    emotion = faceAttributes["emotion"];

                    myEmotion.anger = emotion["anger"];
                    myEmotion.contempt = emotion["contempt"];
                    myEmotion.disgust = emotion["disgust"];
                    myEmotion.fear = emotion["fear"];
                    myEmotion.happiness = emotion["happiness"];
                    myEmotion.neutral = emotion["neutral"];
                    myEmotion.sadness = emotion["sadness"];
                    myEmotion.surprise = emotion["surprise"];
                    
                    using (StreamWriter writer = new StreamWriter(HostingEnvironment.ApplicationPhysicalPath + "File/file.txt" , true))
                    {
                        writer.WriteLine(myEmotion.anger + "," + myEmotion.contempt + "," + myEmotion.disgust + "," + myEmotion.fear + "," + myEmotion.happiness + "," + myEmotion.neutral + "," + myEmotion.sadness + "," + myEmotion.surprise);
                    }
                }

                /*var jsonString = JsonConvert.SerializeObject(result);
                var content_2 = new StringContent(jsonString, Encoding.UTF8, "application/json");

                //Sending JSON
                HttpClient client_2 = new HttpClient();
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uriBase_2, content_2);*/
            }
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }
 
            return sb.ToString().Trim();
        }
        
    }
}