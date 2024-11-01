var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowLocalhost5173",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowLocalhost5173");

app.MapGet("/graph", async () =>
{
    using var client = new HttpClient();
    var requestUri = $"{app.Configuration["fuseki"]}query";

    var query = GetQuery();
    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("query", query)
    });
    var response = await client.PostAsync(requestUri, content);
    var responseAsString = await response.Content.ReadAsStringAsync();

    var d3String = GraphService.ConvertRdfToD3Json(responseAsString);
    return d3String;
});

app.MapGet("/validateStars", async () =>
{
    using var client = new HttpClient();
    var requestUri = $"{app.Configuration["fuseki"]}query";

    var query = GetStarsQuery();
    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("query", query)
    });
    var response = await client.PostAsync(requestUri, content);
    var responseAsString = await response.Content.ReadAsStringAsync();

    var failedStars = GraphService.ValidateStars(responseAsString);
    return failedStars;
});

app.Run();

static string GetQuery()
{
    return $"CONSTRUCT {{?s ?p ?o}} WHERE {{?s ?p ?o}}";
} 

static string GetStarsQuery()
{
    return @$"
    PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
    PREFIX star: <http://example.org/star-ontology/>

     CONSTRUCT {{ ?star ?p ?o }} WHERE {{ ?star rdf:type star:Star . ?star ?p ?o }}";
}