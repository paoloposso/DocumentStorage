using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DocumentStorage.Api.Model;
using DocumentStorage.User;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace DocumentStorage.Api.Test;

public class IntegrationTest
{
    private readonly IConfiguration _configuration;

    public IntegrationTest()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../.."))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Env.Load();

        _client = new HttpClient();

        _baseAddress = _configuration.GetValue<string>("apiUrl")!;

        _email = Environment.GetEnvironmentVariable("TEST_USER_EMAIL");
        _password = Environment.GetEnvironmentVariable("TEST_USER_PASSWORD");
    }

    private HttpClient _client;
    private readonly string _baseAddress;
    private readonly string? _email;
    private readonly string? _password;

    [Fact]
    public async void ShouldAuthenticate()
    {
        var token = await AuthenticateAdmin();

        Assert.True(token.Length > 0);
    }

    [Fact]
    public async void ShouldFailAuthentication()
    {
        HttpResponseMessage response = await _client.PostAsync($"{_baseAddress}authentication", 
            new StringContent(JsonSerializer.Serialize(new AuthenticationRequest {
                Email = _email,
                Password = _password + "asdadas"
            }), Encoding.UTF8, "application/json"));

        var authResponse = 
            JsonSerializer.Deserialize<AuthenticationResponse>(await response.Content.ReadAsStringAsync());

        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(authResponse);
        Assert.True(authResponse!.Token.Length > 0);
    }

    [Fact]
    public async void ShouldListGroups()
    {
        var token = await AuthenticateAdmin();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}usergroup");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await _client.SendAsync(request);

        var groups = JsonSerializer.Deserialize<IEnumerable<UserGroup>>(
            await response.Content.ReadAsStringAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(groups);
        Assert.True(groups.Count() > 0);
    }

    private async Task<string> AuthenticateAdmin() 
    {
        HttpResponseMessage response = await _client.PostAsync($"{_baseAddress}authentication", 
            new StringContent(JsonSerializer.Serialize(new AuthenticationRequest {
                Email = _email,
                Password = _password
            }), Encoding.UTF8, "application/json"));

        var authResponse = 
            JsonSerializer.Deserialize<AuthenticationResponse>(await response.Content.ReadAsStringAsync());

        return authResponse!.Token;
    }
}