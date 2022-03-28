using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using jellyfin_ani_sync;
using jellyfin_ani_sync.Api;
using jellyfin_ani_sync.Configuration;
using jellyfin_ani_sync.Models;
using jellyfin_ani_sync.Models.Mal;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Contrib.HttpClient;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests;

public class Mal {
    private MalApiCalls _malApiCalls;

    [SetUp]
    public void Setup() {
        var mockFactory = new Mock<IHttpClientFactory>();
        var client = new HttpClient();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        IHttpClientFactory factory = mockFactory.Object;

        var mockLoggerFactory = new NullLoggerFactory();
        var mockServerApplicationHost = new Mock<IServerApplicationHost>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _malApiCalls = new MalApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, GetUserConfig.ManuallyGetUserConfig());
    }

    [Test]
    public async Task TestGenericGetUserInformation() {
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