using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Moq;

namespace jellyfin_ani_sync_unit_tests; 

public class Helpers {
    private class DelegatingHandlerStub : DelegatingHandler {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) {
            _handlerFunc = handlerFunc;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return _handlerFunc(request, cancellationToken);
        }
    }
    
    public static void MockHttpCalls(HttpStatusCode responseCode, string responseContent, ref IHttpClientFactory _httpClientFactory) {
        var mockFactory = new Mock<IHttpClientFactory>();
        var clientHandlerStub = new DelegatingHandlerStub((request, _) => {
            request.SetConfiguration(new HttpConfiguration());
            return Task.FromResult(new HttpResponseMessage(responseCode) { RequestMessage = request, Content = new StringContent(responseContent) });
        });
        var client = new HttpClient(clientHandlerStub);
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        _httpClientFactory = mockFactory.Object;
    }
}