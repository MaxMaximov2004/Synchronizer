
using Synchronizer_Core.Vault_Manager;
using System.Collections.Generic;
using System.Text.Json;


/*
 * Баловство с Яндекс - диском
using (HttpClient client = new HttpClient())
{
    client.Timeout = TimeSpan.FromSeconds(10);
    var req = new HttpRequestMessage(HttpMethod.Get, "https://cloud-api.yandex.net/v1/disk/resources/files");
    req.Headers.Add("Authorization", "<Токен>");

    var resp = client.Send(req);

    Console.WriteLine(resp.StatusCode);
    //Console.WriteLine(resp.Content);
    HttpContent content = resp.Content;
    string str_cont  = content.ReadAsStringAsync().Result;
    Console.WriteLine(str_cont);
    if (resp.StatusCode == System.Net.HttpStatusCode.OK)
    {
        using (var file = new StreamWriter("fille_struct.json"))
        {
            file.WriteLine(str_cont);
        }
    }

}*/



/*var dist = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "data");
var dist_file = Path.Combine(dist, $"{Path.GetRandomFileName()}.json");
File.Create(dist_file).Close();

LinkedList<Tuple<string, string>> test_data = new();
test_data.AddLast(Tuple.Create("dist", "source"));
test_data.AddLast(Tuple.Create("dist_1", "source_1"));
test_data.AddLast(Tuple.Create("dist_2", "source_2"));
test_data.AddLast(Tuple.Create("dist_3", "source_3"));



JSON_Manager manage = new JSON_Manager();

manage.Open(dist_file);
manage.Save(test_data);

foreach(var data in manage.Get())
{
    Console.WriteLine($"{data.Item1} {data.Item2}");
}*/



/*
Directory_Manager  manager = new Directory_Manager();

var source = @"C:\Users\Макс\Desktop\source\test_0.txt";
var dist = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "dist", "test_0.txt");

manager.Add_Path(dist, source);
manager.Synchronize();

foreach(String str in manager.Error)
{
    Console.WriteLine(str);
}*/

