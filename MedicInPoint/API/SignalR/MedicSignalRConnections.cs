using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace MedicInPoint.API.SignalR;

public class MedicSignalRConnections : IAsyncDisposable
{
	private readonly HubConnection _analysisConnection;
	private readonly HubConnection _analysisCategoryConnection;
	private readonly HubConnection _messageConnection;
	private readonly HubConnection _messagesMessageConnection;
	private readonly HubConnection _requestConnection;

	public MedicSignalRConnections()
	{
		_analysisConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/analysis")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		_analysisCategoryConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/analysis_category")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		_messageConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/message")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		_messagesMessageConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/messages_message")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
		_requestConnection = new HubConnectionBuilder()
			.WithUrl(MedicConfiguration.URL + "hub/request")
			.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Trace))
			.Build();
	}

	public async ValueTask DisposeAsync()
	{
		if(_analysisConnection != null)
			await _analysisConnection.DisposeAsync();
		if (_analysisCategoryConnection != null)
			await _analysisCategoryConnection.DisposeAsync();
		if (_messageConnection != null)
			await _messageConnection.DisposeAsync();
		if (_messagesMessageConnection != null)
			await _messagesMessageConnection.DisposeAsync();
		if (_requestConnection != null)
			await _requestConnection.DisposeAsync();
	}
}