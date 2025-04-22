using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IPatient
{
	[Get("/patients")]
	Task<IApiResponse<IList<Patient>>> GetPatients();

	[Get("/patients/{id}")]
	Task<IApiResponse<Patient>> GetPatient(int id, CancellationToken? token = null);

	[Post("/patients")]
	Task<IApiResponse<Patient>> AddPatient(Patient analysis, CancellationToken? token = null);
}