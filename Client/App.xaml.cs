using Autofac;
using Client.Core.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClientOPC
{
	/// <summary>
	/// Logica di interazione per App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		protected override void OnStartup(StartupEventArgs e)
		{		
			//Start Logger
			log4net.Config.XmlConfigurator.Configure();
			Log.Info("Start Application");

			//Start Container AutoFac
			var builder = new ContainerBuilder();
			builder.Register(c => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)).As<ILog>();
			base.OnStartup(e);
		}
	}
}
