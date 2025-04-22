using MedicInPoint.API.AIMLAPI.Models;

using Refit;

namespace MedicInPoint.API.AIMLAPI.Placeholders;

public interface IRequest
{
	[Post("chat/completions")]
	Task<IApiResponse<AIResponse>> PostCompletion([Authorize] string bearerToken, [Body] Completion completion);
}