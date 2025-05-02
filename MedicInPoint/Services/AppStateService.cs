using System.Net.NetworkInformation;
using System.Net.Sockets;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

using Microsoft.Extensions.DependencyInjection;

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
	private static bool _internetAvailable = false;
	public static EventHandler<PortStateChangedEventArgs> PortStateChanged = null!;

	private readonly TcpClient client = new TcpClient
	{
		ReceiveTimeout = 0,
		SendTimeout = 0,
		LingerState = new LingerOption(true, 0),
		NoDelay = true,
		Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
		{
			ReceiveTimeout = 0,
			SendTimeout = 0,
			LingerState = new LingerOption(true, 0),
			NoDelay = true
		}
	};

	public PortCheckJob()
	{
		NetworkChange.NetworkAvailabilityChanged += (s, e) => _internetAvailable = e.IsAvailable;
	}

	public async Task Execute(IJobExecutionContext context) =>
		await CheckPortAsync(context);

	private async Task CheckPortAsync(IJobExecutionContext context)
	{
		bool currentState = await IsPortOpenAsync(context);
		await File.AppendAllTextAsync(@"C:\Users\ILNAR\Desktop\ping.txt", $"1|time: {context.ScheduledFireTimeUtc:HH.mm.ss.ffffff} | available: {client.Connected}\n");

		if (currentState != _previousState)
		{
			_previousState = currentState;
			PortStateChanged?.Invoke(this, new PortStateChangedEventArgs(currentState, context));
		}
	}

	private async Task<bool> IsPortOpenAsync(IJobExecutionContext context)
	{
		try
		{
			await client.ConnectAsync("localhost", 5033).ConfigureAwait(false);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}