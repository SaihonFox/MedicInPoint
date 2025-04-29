using System.Net.Sockets;

using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

using Quartz;
using Quartz.Impl;

using static MedicInPoint.Services.AppStateService;

namespace MedicInPoint.Services;

public partial class AppStateService : ObservableObject, IAppStateService
{
	[ObservableProperty]
	public User? _currentUser = null;

	private CancellationTokenSource cts = new();

	public AppStateService()
	{
		StartScheduler();
	}

	async void StartScheduler()
	{
		var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

		var job = JobBuilder.Create<PortCheckJob>().WithIdentity("api-connection-job", "connection-checker").Build();
		
		var trigger = TriggerBuilder.Create()
			.WithIdentity("api-connection-trigger", "connection-checker")
			.StartNow()
			.WithSimpleSchedule(b =>
				b.WithInterval(TimeSpan.FromSeconds(0.2))
				 .RepeatForever()
			)
			.Build();

		await scheduler.ScheduleJob(job, trigger, cts.Token);
		await scheduler.Start(cts.Token);
	}

	public async void Dispose()
	{
		await cts.CancelAsync();
		GC.SuppressFinalize(this);
	}

	public class PortStateChangedEventArgs(bool isPortOpen, IJobExecutionContext context) : EventArgs
	{
		public bool IsPortOpen { get; } = isPortOpen;

		public IJobExecutionContext Context { get; } = context;
	}
}

public class PortCheckJob : IJob
{
	private static bool _previousState = false;
	public static event EventHandler<PortStateChangedEventArgs> PortStateChanged = null!;

	public async Task Execute(IJobExecutionContext context) =>
		await CheckPortAsync(context);

	private async Task CheckPortAsync(IJobExecutionContext context)
	{
		bool currentState = await IsPortOpenAsync(context);

		if (currentState != _previousState)
		{
			_previousState = currentState;
			PortStateChanged?.Invoke(this, new PortStateChangedEventArgs(currentState, context));
		}
	}

	private async Task<bool> IsPortOpenAsync(IJobExecutionContext context)
	{
		using var client = new TcpClient
		{
			ReceiveTimeout = 0,
			SendTimeout = 0,
			LingerState = new LingerOption(true, 0),
			NoDelay = true
		};
		client.Client.ReceiveTimeout = 0;
		client.Client.SendTimeout = 0;
		client.Client.LingerState = new LingerOption(true, 0);
		client.Client.NoDelay = true;
		try
		{
			var task = client.ConnectAsync("localhost", 5033);
			if (task.Wait(50) && !client.Connected)
				return false;
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}