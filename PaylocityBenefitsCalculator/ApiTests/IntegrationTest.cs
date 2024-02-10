using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace ApiTests;

public class IntegrationTest : IDisposable
{
    private readonly HttpClient _httpClient;

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
        }
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}

