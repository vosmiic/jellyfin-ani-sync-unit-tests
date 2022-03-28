using System.Net.Http;
using System.Threading.Tasks;
using jellyfin_ani_sync.Api.Anilist;
using jellyfin_ani_sync.Api.Kitsu;
using jellyfin_ani_sync.Models.Kitsu;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests; 

public class Kitsu {
    private KitsuApiCalls _kitsuApiCalls;

    [SetUp]
    public void Setup() {
        var mockFactory = new Mock<IHttpClientFactory>();
        var client = new HttpClient();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        IHttpClientFactory factory = mockFactory.Object;

        var mockLoggerFactory = new NullLoggerFactory();
        var mockServerApplicationHost = new Mock<IServerApplicationHost>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _kitsuApiCalls = new KitsuApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, GetUserConfig.ManuallyGetUserConfig());
    }
    
    [Test]
    public async Task TestGenericSearch() {
        var result = await _kitsuApiCalls.SearchAnime("K-ON!");
        
        Assert.IsNotNull(result[0].Id);
    }

    [Test]
    public async Task TestGetUserInformation() {
        var result = await _kitsuApiCalls.GetUserInformation();
        
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestGetAnime() {
        var result = await _kitsuApiCalls.GetAnime(4240);
        
        Assert.IsNotNull(result.KitsuAnimeData.Id);
    }

    [Test]
    public async Task TestGetUserList() {
        var result = await _kitsuApiCalls.GetUserAnimeStatus(1289093, 11614);
        
        Assert.IsNotNull(result.Attributes.Progress);
    }

    [Test]
    public async Task TestUpdateAnimeStatus() {
        var result = await _kitsuApiCalls.UpdateAnimeStatus(4240, 6, KitsuUpdate.Status.current);
        
        Assert.IsTrue(result);
    }

    [Test]
    public async Task TestGetRelatedAnime() {
        var result = await _kitsuApiCalls.GetRelatedAnime(4240);
        
        Assert.IsNotNull(result);
    }
    
    [Test]
    public async Task TestGetRelatedAnimeCombinationWorking() {
        var result = await _kitsuApiCalls.GetAnime(4240);
        
        Assert.IsNotEmpty(result.KitsuAnimeData.RelatedAnime);
    }
}