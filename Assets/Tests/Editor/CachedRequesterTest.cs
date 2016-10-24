using System;
using System.Net.Mime;
using NSubstitute;
using NUnit.Framework;
using Silphid.AsyncLoader;
using UniRx;

public class CachedRequesterTest
{
    private IRequester _innerRequester;
    private CachedRequester _fixture;

    private static readonly Uri TestUri1 = new Uri("http://test.com/data.json");
    private static readonly Uri TestUri2 = new Uri("http://test.com/data.xml");
    private static readonly byte[] Bytes1 = {0x12, 0x34};
    private static readonly byte[] Bytes2 = {0x56, 0x78};
    private static readonly ContentType TestContentType1 = new ContentType("application/json");
    private static readonly ContentType TestContentType2 = new ContentType("application/xml");

    [SetUp]
    public void SetUp()
    {
        _innerRequester = Substitute.For<IRequester>();
        SetupDownload(_innerRequester, TestUri1, Bytes1, TestContentType1);
        SetupDownload(_innerRequester, TestUri2, Bytes2, TestContentType2);

        _fixture = new CachedRequester(_innerRequester);
        _fixture.ClearCache();
    }

    private void SetupDownload(IRequester requester, Uri uri, byte[] bytes, ContentType contentType)
    {
        requester
            .Request(uri)
            .Returns(Observable.Return(
                new Response
                {
                    Bytes = bytes,
                    Headers = { [KnownHttpHeaders.ContentType] = contentType.ToString() }
                }));
    }

    private void AssertResponse(Response response, byte[] bytes, ContentType contentType)
    {
        Assert.That(response.Bytes.Length, Is.EqualTo(bytes.Length));
        Assert.That(response.Bytes[0], Is.EqualTo(bytes[0]));
        Assert.That(response.Bytes[1], Is.EqualTo(bytes[1]));
        Assert.That(response.ContentType.MediaType, Is.EqualTo(contentType.MediaType));
    }

	[Test]
	public void DelegatesToInnerDownloader()
	{
	    var response = _fixture.Request(TestUri1).Wait();

	    _innerRequester.Received(1).Request(TestUri1);
	    AssertResponse(response, Bytes1, TestContentType1);
	}

	[Test]
	public void DownloadsSameUriOnlyOnce()
	{
	    var response1 = _fixture.Request(TestUri1).Wait();
	    var response2 = _fixture.Request(TestUri2).Wait();
	    var response3 = _fixture.Request(TestUri1).Wait();

	    _innerRequester.Received(2).Request(Arg.Any<Uri>());
	    _innerRequester.Received(1).Request(TestUri1);
	    _innerRequester.Received(1).Request(TestUri2);
	    AssertResponse(response1, Bytes1, TestContentType1);
	    AssertResponse(response2, Bytes2, TestContentType2);
	    AssertResponse(response3, Bytes1, TestContentType1);
	}
}
