using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IPatient
{
	[Get("/patients")]
	Task<IList<Patient>> GetPatients();

	[Get("/patients/{id}")]
	Task<Patient?> GetPatient(int id);

	[Post("/patients")]
	Task AddPatient(Patient analysis);
}