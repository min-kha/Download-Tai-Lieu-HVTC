using HtmlAgilityPack;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net;


//var client = new HttpClient();
//var request = new HttpRequestMessage(HttpMethod.Post, "https://catalog.hvtc.edu.vn/pds");
//request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
//request.Headers.Add("Accept-Language", "en,en-US;q=0.9,vi;q=0.8");
//request.Headers.Add("Cache-Control", "max-age=0");
//request.Headers.Add("Connection", "keep-alive");
//request.Headers.Add("Cookie", "ALEPH_SESSION_ID=VLP8DVUEFJSVX76NQTC33P3CAQ6RRVGN4271R86M7DI66JN22S; PDS_HANDLE=27102023044650151381419063983760");
//request.Headers.Add("DNT", "1");
//request.Headers.Add("Origin", "https://catalog.hvtc.edu.vn");
//request.Headers.Add("Referer", "https://catalog.hvtc.edu.vn/pds?func=load-login&calling_system=primo&institute=AOF01&lang=und&url=https://libsearch.hvtc.edu.vn:443/primo_library/libweb/pdsLogin?targetURL=https%3A%2F%2Flibsearch.hvtc.edu.vn%2Fprimo-explore%2Fsearch%3Fvid%3Daof%26from-new-ui%3D1%26authenticationProfile%3DAOF");
//request.Headers.Add("Sec-Fetch-Dest", "document");
//request.Headers.Add("Sec-Fetch-Mode", "navigate");
//request.Headers.Add("Sec-Fetch-Site", "same-origin");
//request.Headers.Add("Sec-Fetch-User", "?1");
//request.Headers.Add("Upgrade-Insecure-Requests", "1");
//request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2088.61");
//request.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"118\", \"Microsoft Edge\";v=\"118\", \"Not=A?Brand\";v=\"99\"");
//request.Headers.Add("sec-ch-ua-mobile", "?0");
//request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
//var collection = new List<KeyValuePair<string, string>>();
//collection.Add(new("func", "login"));
//collection.Add(new("calling_system", "primo"));
//collection.Add(new("term1", "short"));
//collection.Add(new("lang", "vie"));
//collection.Add(new("selfreg", ""));
//collection.Add(new("bor_id", "20CL73402010432"));
//collection.Add(new("bor_verification", "matkhau"));
//collection.Add(new("institute", "AOF50"));
//collection.Add(new("url", "https://libsearch.hvtc.edu.vn:443/primo_library/libweb/pdsLogin?targetURL=https%3A%2F%2Flibsearch%2Ehvtc%2Eedu%2Evn%2Fprimo-explore%2Fsearch%3Fvid%3Daof&from-new-ui=1&authenticationProfile=AOF"));
//var content = new FormUrlEncodedContent(collection);
//request.Content = content;
//await client.SendAsync(request);

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {

            //var linkGoto = await LoginPost();
            //await GetSession((linkGoto,));

            Console.WriteLine("Download image Hoai \n\n");

            // Nhập folder
            string appFolder = Directory.GetCurrentDirectory();
            var destFolder = appFolder;
            Console.Write("\nFolder: " + destFolder + "\n");
            //var destFolder = Console.ReadLine() ?? "";


            Console.Write("Link base: ");
            string baseLink = Console.ReadLine() ?? "";



            // Nhập số trang đầu
            Console.Write("From page number: ");
            var fromPageStr = Console.ReadLine() ?? "";
            int fromPage = int.Parse(fromPageStr);

            // Nhập số trang cuối
            Console.Write("To page number: ");
            var toPageStr = Console.ReadLine() ?? "";
            int toPage = int.Parse(toPageStr);

            // Nhập max download
            Console.Write("Max download (20-50):");
            var maxDownStr = Console.ReadLine() ?? "";
            int maxParallelDownloads = int.Parse(maxDownStr);

            // Nhập cookie
            Console.Write("Cookies: ");
            var cookies = Console.ReadLine() ?? "";

            Console.WriteLine("Start.........\n\n");


            //==============================================
            List<Task> tasks = new List<Task>();

            Semaphore semaphore = new Semaphore(maxParallelDownloads, maxParallelDownloads);

            for (int i = fromPage; i <= toPage; i++)
            {
                semaphore.WaitOne();

                Task task = DownloadImage(baseLink, destFolder, cookies, i);
                task.ContinueWith(t => semaphore.Release());

                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("\nDownloaded image\n");

            CompileToPDF(destFolder);

        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);

        }
        Console.ReadLine();


        //===========================================================================================
        static async Task DownloadImage(string baseLink, string destFolder, string cookies, int nameOfImage)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseLink}&pages={nameOfImage}");
                #region Header
                request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                request.Headers.Add("Accept-Language", "en,en-US;q=0.9,vi;q=0.8");
                request.Headers.Add("Cache-Control", "max-age=0");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("Cookie", cookies);
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Sec-Fetch-Dest", "document");
                request.Headers.Add("Sec-Fetch-Mode", "navigate");
                request.Headers.Add("Sec-Fetch-Site", "none");
                request.Headers.Add("Sec-Fetch-User", "?1");
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2088.61");
                request.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"118\", \"Microsoft Edge\";v=\"118\", \"Not=A?Brand\";v=\"99\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\""); 
                #endregion
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string content2 = await response.Content.ReadAsStringAsync();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(content2); // Load the HTML content

                var imageElement = doc.DocumentNode.SelectNodes("//img[contains(@src, 'base64')]").FirstOrDefault();

                if (imageElement != null)
                {
                    string src = imageElement.Attributes["src"].Value; // Get the 'src' attribute

                    byte[] imageBytes = Convert.FromBase64String(src.Split(',')[1]);

                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var returnImage = System.Drawing.Image.FromStream(ms);
                        returnImage.Save($"{destFolder}/{nameOfImage:0000}.png", ImageFormat.Png);
                        Console.WriteLine($"Downloaded: {destFolder}/{nameOfImage}.png"); // Print the 'src' attribute
                    }
                }
                else
                {
                    Console.WriteLine("Image not found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(nameOfImage + " ----> Download image error: " + e.Message);
            }
        }
    }

    private static void CompileToPDF(string destFolder)
    {
        // Tạo tài liệu PDF với kích thước tương ứng
        Document doc = new();

        PdfWriter.GetInstance(doc, new FileStream(destFolder + "/Result.pdf", FileMode.Create));
        doc.Open();

        // Duyệt qua các file ảnh trong thư mục

        // Lấy danh sách tất cả các file .png trong thư mục
        string[] imageFiles = Directory.GetFiles(destFolder, "*.png");

        // Sắp xếp danh sách theo tên file
        Array.Sort(imageFiles);

        foreach (string imagePath in imageFiles)
        {
            // Tạo đối tượng ảnh từ file
            Image img = Image.GetInstance(imagePath);

            doc.SetPageSize(new Rectangle(img.Width, img.Height));

            img.SetAbsolutePosition(0, 0);

            // Thêm ảnh vào tài liệu
            doc.NewPage();
            doc.Add(img);
        }

        // Đóng và lưu tài liệu PDF
        doc.Close();
        Console.WriteLine("\nCompile PDF success!");
    }

    private static async Task GetSession((string, string) linkCookie)
    {
        var cookies = new CookieContainer();
        var handler = new HttpClientHandler();
        handler.CookieContainer = cookies;
        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://catalog.hvtc.edu.vn" + linkCookie.Item2);
        request.Headers.Add("Cookie", "PDS_HANDLE=3010202313544050232009622020628501");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Uri uri = new Uri("https://catalog.hvtc.edu.vn");
        IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
        foreach (Cookie cookie in responseCookies)
            Console.WriteLine(cookie.Name + ": " + cookie.Value);
        return;
    }

    private static async Task<(string, string)> LoginPost()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://catalog.hvtc.edu.vn/pds");
        request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        request.Headers.Add("Accept-Language", "en,en-US;q=0.9,vi;q=0.8");
        request.Headers.Add("Cache-Control", "max-age=0");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Cookie", "ALEPH_SESSION_ID=VLP8DVUEFJSVX76NQTC33P3CAQ6RRVGN4271R86M7DI66JN22S; PDS_HANDLE=27102023044650151381419063983760");
        request.Headers.Add("DNT", "1");
        request.Headers.Add("Origin", "https://catalog.hvtc.edu.vn");
        request.Headers.Add("Referer", "https://catalog.hvtc.edu.vn/pds?func=load-login&calling_system=primo&institute=AOF01&lang=und&url=https://libsearch.hvtc.edu.vn:443/primo_library/libweb/pdsLogin?targetURL=https%3A%2F%2Flibsearch.hvtc.edu.vn%2Fprimo-explore%2Fsearch%3Fvid%3Daof%26from-new-ui%3D1%26authenticationProfile%3DAOF");
        request.Headers.Add("Sec-Fetch-Dest", "document");
        request.Headers.Add("Sec-Fetch-Mode", "navigate");
        request.Headers.Add("Sec-Fetch-Site", "same-origin");
        request.Headers.Add("Sec-Fetch-User", "?1");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2088.61");
        request.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"118\", \"Microsoft Edge\";v=\"118\", \"Not=A?Brand\";v=\"99\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("func", "login"));
        collection.Add(new("calling_system", "primo"));
        collection.Add(new("term1", "short"));
        collection.Add(new("lang", "vie"));
        collection.Add(new("selfreg", ""));
        collection.Add(new("bor_id", "20CL73402010432"));
        collection.Add(new("bor_verification", "22012002"));
        collection.Add(new("institute", "AOF50"));
        collection.Add(new("url", "https://libsearch.hvtc.edu.vn:443/primo_library/libweb/pdsLogin?targetURL=https%3A%2F%2Flibsearch%2Ehvtc%2Eedu%2Evn%2Fprimo-explore%2Fsearch%3Fvid%3Daof&from-new-ui=1&authenticationProfile=AOF"));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string htmlContent = await response.Content.ReadAsStringAsync();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent); // Load the HTML content

        var gotoElement = doc.DocumentNode.SelectNodes("//a[contains(@href, '/goto/')]").FirstOrDefault();
        return (gotoElement.Attributes["href"].Value, "");
    }
}



//byte[] imageBytes = Convert.FromBase64String(src.Split(',')[1]);
//using (MemoryStream ms = new MemoryStream(imageBytes))
//{
//    iTextSharp.text.Image imagesrc = iTextSharp.text.Image.GetInstance(ms);
//    using (FileStream fs = new FileStream("htttp2.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
//    {
//        using (Document docaa = new Document())
//        {
//            using (PdfWriter writer = PdfWriter.GetInstance(docaa, fs))
//            {
//                docaa.Open();
//                docaa.Add(imagesrc);
//                docaa.Close();
//            }
//        }
//    }
//}