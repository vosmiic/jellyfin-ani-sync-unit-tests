using System.Net.Http;
using System.Threading.Tasks;
using jellyfin_ani_sync.Api.Anilist;
using jellyfin_ani_sync.Models;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests; 

public class AniList {
    private AniListApiCalls _aniListApiCalls;

    [SetUp]
    public void Setup() {
        var mockFactory = new Mock<IHttpClientFactory>();
        var client = new HttpClient();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        IHttpClientFactory factory = mockFactory.Object;

        var mockLoggerFactory = new NullLoggerFactory();
        var mockServerApplicationHost = new Mock<IServerApplicationHost>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _aniListApiCalls = new AniListApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, GetUserConfig.ManuallyGetUserConfig());
    }

    [Test]
    public async Task TestGenericSearch() {
        var result = await _aniListApiCalls.SearchAnime("K-ON!");
        
        Assert.IsNotNull(result[0].Id);
    }
    
    [Test]
    public async Task TestGenericLongTitleSearch() {
        var result = await _aniListApiCalls.SearchAnime("Kono Subarashii Sekai ni Shukufuku wo! 2: Kono Subarashii Geijutsu ni Shukufuku wo!");
        
        Assert.IsNotNull(result[0].Id);
    }

    [Test]
    public async Task TestGettingCurrentUser() {
        var result = await _aniListApiCalls.GetCurrentUser();
        
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestUpdatingAnime() {
        var result = await _aniListApiCalls.UpdateAnime(5680, AniListSearch.MediaListStatus.Current, 1);
        
        Assert.IsTrue(result);
    }

    [Test]
    public async Task TestGenericSearchPaging() {
        var result = await _aniListApiCalls.SearchAnime("life");
        
        Assert.IsTrue(result.Count > AniListApiCalls.PageSize);
    }

    [Test]
    public async Task TestGetAnime() {
        var result = await _aniListApiCalls.GetAnime(5680);

        Assert.IsNotNull(result.Id);
    }
}