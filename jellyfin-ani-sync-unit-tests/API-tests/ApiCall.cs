using System;
using System.Net.Http;
using System.Threading.Tasks;
using jellyfin_ani_sync.Api;
using jellyfin_ani_sync.Api.Anilist;
using jellyfin_ani_sync.Configuration;
using jellyfin_ani_sync.Helpers;
using jellyfin_ani_sync.Models;
using MediaBrowser.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace jellyfin_ani_sync_unit_tests.API_tests;

public class ApiCall {
    private ApiCallHelpers _aniListApiCallHelpers;
    private ApiCallHelpers _malApiCallHelpers;

    [SetUp]
    public void Setup() {
        var mockFactory = new Mock<IHttpClientFactory>();
        var client = new HttpClient();
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        IHttpClientFactory factory = mockFactory.Object;

        var mockLoggerFactory = new NullLoggerFactory();
        var mockServerApplicationHost = new Mock<IServerApplicationHost>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var malApiCalls = new MalApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, GetUserConfig.ManuallyGetUserConfig());
        _malApiCallHelpers = new ApiCallHelpers(malApiCalls: malApiCalls);

        var aniListApiCalls = new AniListApiCalls(factory, mockLoggerFactory, mockServerApplicationHost.Object, mockHttpContextAccessor.Object, GetUserConfig.ManuallyGetUserConfig());
        _aniListApiCallHelpers = new ApiCallHelpers(aniListApiCalls: aniListApiCalls);
    }

    [Test]
    public async Task TestMalGenericSearch() {
        var result = await _malApiCallHelpers.SearchAnime("K-ON!");

        Assert.IsNotEmpty(result);
    }

    [Test]
    public async Task TestAniListGenericSearch() {
        var result = await _aniListApiCallHelpers.SearchAnime("K-ON!");

        Assert.IsNotEmpty(result);
    }

    [Ignore("destructive")]
    [Test]
    public async Task TestMalUpdatingAnime() {
        var result = await _malApiCallHelpers.UpdateAnime(40834, 10, Status.Watching, false, 1, DateTime.Today - TimeSpan.FromDays(7));

        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestAniListUpdatingAnime() {
        var result = await _aniListApiCallHelpers.UpdateAnime(5680, 5, Status.Watching, false, 1, DateTime.Today - TimeSpan.FromDays(7));

        Assert.IsNotNull(result);
    }

    [Test]
    public async Task TestMalGetAnime() {
        var result = await _malApiCallHelpers.GetAnime(7791);

        Assert.IsNotNull(result.Title);
    }

    [Test]
    public async Task TestAniListGetAnime() {
        var result = await _aniListApiCallHelpers.GetAnime(5680);

        Assert.IsNotEmpty(result.Title);
    }
}