using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using jellyfin_ani_sync.Api;
using jellyfin_ani_sync.Configuration;
using jellyfin_ani_sync.Models;
using jellyfin_ani_sync.Models.Mal;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests;

public class Mal {
    private MalApiCalls _malApiCalls;
    private ILoggerFactory _loggerFactory;
    private Mock<IServerApplicationHost> _serverApplicationHost;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private IHttpClientFactory _httpClientFactory;

    private class DelegatingHandlerStub : DelegatingHandler {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) {
            _handlerFunc = handlerFunc;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return _handlerFunc(request, cancellationToken);
        }
    }

    private void MockHttpCalls(HttpStatusCode responseCode, string responseContent) {
        var mockFactory = new Mock<IHttpClientFactory>();
        var clientHandlerStub = new DelegatingHandlerStub((request, _) => {
            request.SetConfiguration(new HttpConfiguration());
            return Task.FromResult(new HttpResponseMessage(responseCode) { RequestMessage = request, Content = new StringContent(responseContent) });
        });
        var client = new HttpClient(clientHandlerStub);
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        _httpClientFactory = mockFactory.Object;
    }

    [SetUp]
    public void Setup() {
        _loggerFactory = new NullLoggerFactory();
        _serverApplicationHost = new Mock<IServerApplicationHost>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    [Test]
    public async Task TestGenericGetUserInformation() {
        MockHttpCalls(HttpStatusCode.OK, JsonSerializer.Serialize(new User {
            Id = 1,
            Name = "name",
            Location = "location",
            JoinedAt = DateTime.UtcNow,
            Picture = "picture"
        }));

        _malApiCalls = new MalApiCalls(_httpClientFactory, _loggerFactory, _serverApplicationHost.Object, _httpContextAccessor.Object, new UserConfig {
            UserApiAuth = new [] {
                new UserApiAuth {
                    AccessToken = "accessToken",
                    Name = ApiName.Mal,
                    RefreshToken = "refreshToken"
                }
            }
        });

        var result = await _malApiCalls.GetUserInformation();
        Assert.IsNotNull(result.Id);
    }

    [Test]
    public async Task TestGenericSearchAnime() {
        var result = await _malApiCalls.SearchAnime("K-ON!!", new[] { "id,title,alternative_titles" });
        var filter = result.FirstOrDefault(anime => anime.Id == 7791);
        Assert.IsNotNull(filter);
    }

    [Test]
    public async Task TestLongQuerySearchAnime() {
        var result = await _malApiCalls.SearchAnime("Kono Subarashii Sekai ni Shukufuku wo! 2: Kono Subarashii Geijutsu ni Shukufuku wo!", new[] { "id" });
        var filter = result.FirstOrDefault(anime => anime.Id == 34626);
        Assert.IsNotNull(filter);
    }

    [Test]
    public async Task TestGenericGetAnime() {
        var result = await _malApiCalls.GetAnime(7791);
        Assert.IsNotNull(result);
    }
    
    [Ignore("Destructive")]
    [Test]
    public async Task TestUpdateAnimeStatus() {
        var makeChange = await _malApiCalls.UpdateAnimeStatus(339, 1, Status.Completed, true);
        // revert the change
        await _malApiCalls.UpdateAnimeStatus(339, 13, Status.Completed, false);
        Assert.IsNotNull(makeChange);
    }
}