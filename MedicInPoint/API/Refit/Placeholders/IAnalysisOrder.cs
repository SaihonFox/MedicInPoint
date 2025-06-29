﻿using MedicInPoint.Models;

using Refit;

namespace Medic.API.Refit.Placeholders;

public interface IAnalysisOrder
{
	[Get("/analysis_orders")]
	Task<IApiResponse<List<AnalysisOrder>>> GetAnalysisOrders();

	[Get("/analysis_orders/user")]
	Task<IApiResponse<List<AnalysisOrder>>> GetAnalysisOrders4User([Query] int id);

	[Post("/analysis_orders")]
	Task<IApiResponse<User>> Post([Body] AnalysisOrder analysisOrder);

	[Put("/analysis_orders")]
	Task<IApiResponse<User>> Update([Body] AnalysisOrder analysisOrder);

	[Delete("/analysis_orders/{id}")]
	Task<IApiResponse<User>> Delete(int id);
}