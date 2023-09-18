using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace AService.Middleware;

public class HttpLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<HttpLoggingMiddleware> logger;

	public HttpLoggingMiddleware(
		RequestDelegate next, 
		ILogger<HttpLoggingMiddleware> logger)
	{
		_next = next;
		this.logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		context.Request.EnableBuffering();

		Stream originalBody = context.Response.Body;

		using (var memStream = new MemoryStream())
		{
			context.Response.Body = memStream;

			this.logger?.LogDebug("> Get Request Info:{INFO}", await GetRequestInfo(context));

			await _next(context);

			this.logger?.LogDebug("> Get Response Info:{INFO}", await GetResponseInfo(context, memStream));

			memStream.Position = 0;
			await memStream.CopyToAsync(originalBody);
		}
		context.Response.Body = originalBody;
	}

	private static async Task<string> GetRequestInfo(HttpContext context)
	{
		var request = context.Request;
		string strInfoBody = string.Empty;
		bool infoBody = request.ContentLength > 0;
		if (infoBody)
		{
			request.EnableBuffering();
			request.Body.Position = 0;
			string tmp = await ReadContent(request.BodyReader);
			request.Body.Position = 0;

			strInfoBody = string.Concat(Environment.NewLine, "Body: ", tmp);
		}

		return string.Concat(Environment.NewLine, '[', request.Method, "]: ", request.Path, '/', request.QueryString, (infoBody ? strInfoBody : string.Empty));
	}

	private static async Task<string> GetResponseInfo(HttpContext context, MemoryStream body) 
	{
		var response = context.Response;
		string strInfoBody = string.Empty;
		bool infoBody = body.Length > 0;
		if (infoBody)
		{
			body.Position = 0;
			string tmp = await new StreamReader(body).ReadToEndAsync();

			strInfoBody = string.Concat(Environment.NewLine, "Body: ", tmp);
		}

		return string.Concat(Environment.NewLine, '[', response.StatusCode, "]: ", (infoBody ? strInfoBody : string.Empty));		
	}

	private static async Task<string> ReadContent(PipeReader reader)
	{
		StringBuilder result = new();

		while (true)
		{
			ReadResult readResult = await reader.ReadAsync();
			var buffer = readResult.Buffer;

			result.Append(GetString(readResult.Buffer));

			reader.AdvanceTo(buffer.Start, buffer.End);
						
			if (readResult.IsCompleted)
			{
				break;
			}
		}

		return result.ToString();
	}

	private static string GetString(in ReadOnlySequence<byte> readOnlySequence)
	{
		// Separate method because Span/ReadOnlySpan cannot be used in async methods
		ReadOnlySpan<byte> span = readOnlySequence.IsSingleSegment ? readOnlySequence.First.Span : readOnlySequence.ToArray().AsSpan();
		return Encoding.UTF8.GetString(span);
	}
}


