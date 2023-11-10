using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using jellyfin_ani_sync.Api.Shikimori;
using jellyfin_ani_sync.Models.Shikimori;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests; 

public class Shikimori {
    private ShikimoriApiCalls _shikimoriApiCalls;

    [SetUp]
    public void Setup() {
        var mockFactory = new Mock<IHttpClientFactory>();
        var client = new HttpClient();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        IHttpClientFactory factory = mockFactory.Object;

        var mockLoggerFactory = new NullLoggerFactory();
        var mockServerApplicationHost = new Mock<IServerApplicationHost>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _shikimoriApiCalls = new ShikimoriApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, new Dictionary<string, string> {{"User-Agent", Secrets.shikimoriAppName}}, GetUserConfig.ManuallyGetUserConfig());
    }

    [Test]
    public async Task TestGetUser() {
        var result = await _shikimoriApiCalls.GetUserInformation();
        
        Assert.IsNotNull(result);
    }
    
    [Test]
    public async Task TestGenericSearch() {
        var result = await _shikimoriApiCalls.SearchAnime("Bakemonogatari");
        
        Assert.IsNotNull(result?[0].Id);
    }

    [Test]
    public async Task TestGetAnime() {
        var result = await _shikimoriApiCalls.GetAnime(5081);
        
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestGetRelatedAnime() {
        var result = await _shikimoriApiCalls.GetRelatedAnime(5081);
        
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestUpdateAnime() {
        var result = await _shikimoriApiCalls.UpdateAnime(5081, ShikimoriUpdate.UpdateStatus.watching, 3, 1);
        
        Assert.IsTrue(result);
    }

    [Test]
    public async Task TestMultipleAnimeUpdates() {
        var result = await _shikimoriApiCalls.UpdateAnime(5801, ShikimoriUpdate.UpdateStatus.watching, 6, 1);
        Assert.IsTrue(result);
        await Task.Delay(1000);
        result = await _shikimoriApiCalls.UpdateAnime(5801, ShikimoriUpdate.UpdateStatus.watching, 7);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task TestGetAnimeList() {
        var result = await _shikimoriApiCalls.GetUserAnimeList();
        
        Assert.IsNotNull(result);
    }
}