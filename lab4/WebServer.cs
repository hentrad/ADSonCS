using System.Net;
using System.Text;
using System.Text.Json;

namespace HashLab;

public class WebServer
{
    private record Req(string? Key, string? Info);

    private readonly OpenHashTable _table = new();
    private readonly object _lock = new();
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task RunAsync(string prefix)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(prefix);
        listener.Start();
        while (true)
        {
            var ctx = await listener.GetContextAsync();
            _ = Task.Run(() => Handle(ctx));
        }
    }

    private void Handle(HttpListenerContext ctx)
    {
        try
        {
            string path = ctx.Request.Url!.AbsolutePath;
            if (path == "/")
            {
                Send(ctx, Encoding.UTF8.GetBytes(Ui.Html), "text/html; charset=utf-8");
                return;
            }
            Req? req = null;
            if (ctx.Request.HttpMethod == "POST")
            {
                using var sr = new StreamReader(ctx.Request.InputStream, Encoding.UTF8);
                string body = sr.ReadToEnd();
                if (body.Length > 0) req = JsonSerializer.Deserialize<Req>(body, Json);
            }
            object result;
            lock (_lock)
            {
                result = path switch
                {
                    "/api/add" => _table.Add(req?.Key ?? "", req?.Info ?? ""),
                    "/api/find" => _table.Find(req?.Key ?? ""),
                    "/api/clear" => ClearAndState(),
                    "/api/state" => State(),
                    _ => new { error = "not found" }
                };
            }
            Send(ctx, JsonSerializer.SerializeToUtf8Bytes(result, Json), "application/json; charset=utf-8");
        }
        catch (Exception ex)
        {
            try { Send(ctx, Encoding.UTF8.GetBytes(ex.Message), "text/plain; charset=utf-8", 500); } catch { }
        }
    }

    private object ClearAndState() { _table.Clear(); return State(); }

    private object State()
    {
        var cells = new List<object>();
        for (int i = 1; i <= OpenHashTable.N; i++)
            cells.Add(new { i, key = _table.KeyAt(i), info = _table.InfoAt(i) });
        return new { n = OpenHashTable.N, m = OpenHashTable.M, count = _table.Count, cells, collisions = _table.Collisions };
    }

    private static void Send(HttpListenerContext ctx, byte[] data, string type, int status = 200)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = type;
        ctx.Response.ContentLength64 = data.Length;
        ctx.Response.OutputStream.Write(data, 0, data.Length);
        ctx.Response.Close();
    }
}
