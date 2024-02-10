using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace ApiTests;

public class IntegrationTest : IDisposable
{
    private HttpClient? _httpClient;

    public IntegrationTest()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => { });

        _httpClient = application.CreateClient();
    }

    protected HttpClient HttpClient
    {
        get
        {
            return _httpClient;
            if (_httpClient == default)
            {
                _httpClient = new HttpClient
                {
                    //task: update your port if necessary
                    BaseAddress = new Uri("https://localhost:7124")
                };
                _httpClient.DefaultRequestHeaders.Add("accept", "text/plain");
            }

            return _httpClient;
        }
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}

