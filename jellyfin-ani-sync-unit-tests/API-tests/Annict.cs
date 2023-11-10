using System.Net.Http;
using System.Threading.Tasks;
using jellyfin_ani_sync.Api.Anilist;
using jellyfin_ani_sync.Api.Annict;
using jellyfin_ani_sync.Models;
using jellyfin_ani_sync.Models.Annict;
using Jellyfin.Extensions;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests; 

public class Annict {
    private AnnictApiCalls _annictApiCalls;

    [SetUp]
    public void Setup() {
        var mockFactory = new Mock<IHttpClientFactory>();
        var client = new HttpClient();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        IHttpClientFactory factory = mockFactory.Object;

        var mockLoggerFactory = new NullLoggerFactory();
        var mockServerApplicationHost = new Mock<IServerApplicationHost>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _annictApiCalls = new AnnictApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, GetUserConfig.ManuallyGetUserConfig());
    }

    [Test]
    public async Task TestGenericSearch() {
        var result = await _annictApiCalls.SearchAnime("けいおん");
        
        Assert.IsNotNull(result[0].Id);
    }

    [Test]
    public async Task TestGettingCurrentUser() {
        var result = await _annictApiCalls.GetCurrentUser();
        
        Assert.IsNotNull(result);
    }
    
    [Test]
    public async Task TestGettingUserAnimeList() {
        var result = await _annictApiCalls.GetAnimeList(AnnictSearch.AnnictMediaStatus.Watched);
        
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestUpdatingAnime() {
        var result = await _annictApiCalls.UpdateAnime("V29yay02Njg=", AnnictSearch.AnnictMediaStatus.Watched);
        
        Assert.IsTrue(result);
    }
}