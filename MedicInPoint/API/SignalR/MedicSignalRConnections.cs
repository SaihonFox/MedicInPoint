using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace MedicInPoint.API.SignalR;

public class MedicSignalRConnections : IAsyncDisposable
{
	public readonly HubConnection AnalysisConnection;
	public readonly HubConnection AnalysisCategoryConnection;
	public readonly HubConnection MessageConnection;
	public readonly HubConnection MessagesMessageConnection;
	public readonly HubConnection PatientConnection;
	public readonly HubConnection RequestConnection;

	public MedicSignalRConnections()
	{
		AnalysisConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/analysis")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		AnalysisCategoryConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/analysis_category")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		MessageConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/message")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		MessagesMessageConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/messages_message")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		PatientConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/patient")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		RequestConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/request")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();

		Task.WhenAll([
			AnalysisConnection.StartAsync(),
			AnalysisCategoryConnection.StartAsync(),
			MessageConnection.StartAsync(),
			MessagesMessageConnection.StartAsync(),
			PatientConnection.StartAsync(),
			RequestConnection.StartAsync()
		]);
	}

	public async ValueTask DisposeAsync()
	{
		if (AnalysisConnection != null)
			await AnalysisConnection.DisposeAsync();
		if (AnalysisCategoryConnection != null)
			await AnalysisCategoryConnection.DisposeAsync();
		if (MessageConnection != null)
			await MessageConnection.DisposeAsync();
		if (MessagesMessageConnection != null)
			await MessagesMessageConnection.DisposeAsync();
		if (PatientConnection != null)
			await PatientConnection.DisposeAsync();
		if (RequestConnection != null)
			await RequestConnection.DisposeAsync();
		GC.SuppressFinalize(this);
	}
}