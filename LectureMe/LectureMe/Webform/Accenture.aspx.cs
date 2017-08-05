using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace LectureMe.Webform
{
    public partial class Accenture : System.Web.UI.Page
    {
        const string zip = "";
        const string uriBase = " http://smartvision.aiam-dh.com:8080/api/v1.0/tasks/run/" + zip;
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUpload.FileName);
                    FileUpload.SaveAs(Server.MapPath("../Picture/") + filename);

                    byte[] imageArray = System.IO.File.ReadAllBytes(@"C:\Users\ssabd\Documents\Visual Studio 2017\Projects\LectureMe\LectureMe\Picture\" + filename);
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    MakeAnalysisRequest(@"C:\Users\ssabd\Documents\Visual Studio 2017\Projects\LectureMe\LectureMe\Picture\" + filename, base64ImageRepresentation);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Gets the analysis of the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async void MakeAnalysisRequest(string imageFilePath, string base64ImageRepresentation)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("demo1", "hackathon7493");

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "scanned=true&image=" + base64ImageRepresentation;

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PutAsync(uri, content);

                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                String result = JsonPrettyPrint(contentString);

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