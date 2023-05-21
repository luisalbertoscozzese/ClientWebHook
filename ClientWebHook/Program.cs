using System.Net;

Random rnd = new();
string port=rnd.Next(1,5000).ToString();

// Se crea la instancia HttpCliente con un BaseAddress http://localhost:{port} donde port se genera de forma aleatoria al iniciar el cliente.
HttpClient client = new() { BaseAddress= new Uri($"http://localhost:{port}") };
client.Timeout = TimeSpan.FromSeconds(600);

// Se hace una solicitud POST al endpoint del servidor, pasando la URL del cliente como parámetro.
HttpContent content = new StringContent(string.Empty);
client.DefaultRequestHeaders.Add("X-APIKey", "1");
HttpResponseMessage response = await client.PostAsync($"https://localhost:7035/api/WebHook/SubscribeEventUpdate?url={client.BaseAddress}",content);

HttpListener listener = new();

listener.Prefixes.Add(client.BaseAddress.ToString());

// Se inicia la escucha de las solicitudes POST en el endpoint.
listener.Start();

Console.WriteLine($"Cliente escuchando {client.BaseAddress}");

// Se crea un bucle infinito para mantener el programa en ejecución.
while (true)
{
    // Se espera a que se reciba una solicitud POST en el endpoint.
    HttpListenerContext context = listener.GetContext();

    // Se obtiene el cuerpo de la solicitud POST.
    string json;
    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
    {
        json = reader.ReadToEnd();
        Console.WriteLine(json);
    }
    context.Response.Close();
}