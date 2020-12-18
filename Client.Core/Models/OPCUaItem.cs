using Client.Core.Interfaces;
using Client.Core.Services.Enums;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Models
{
	public class OPCUaItem : IItem
	{
		public event UpdatedEventHandler Updated;

		public string ItemID { get; private set; }

		private object _value;

		public object Value  {
			get { return _value; }
			private set{
					if(value != _value)
					{
					_value = value;
					OnUpdated(_value);
					}
				}
		}

		public DateTime LastUpdate { get; private set; }

		public AccessRights AccessRights { get; private set; }

		public Type DataType { get; private set; }

		public ItemType ItemType { get; private set; }

		public MonitoredItem MonitorItem { get; private set; }

		public OPCUaItem()
		{

		}

		public OPCUaItem(string nodeId)
		{
			ItemID = nodeId;
			ItemType = ItemType.UA;
			MonitorItem = new MonitoredItem() { StartNodeId = nodeId };
			MonitorItem.SamplingInterval = 1;
			MonitorItem.QueueSize = 1;
			MonitorItem.DiscardOldest = true;			
			MonitorItem.Notification += OnNotification;
		}

		private void OnNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
		{
			if(DataType == null)
			{
				VariableNode variableNode= MonitorItem.Subscription.Session.NodeCache.Find(ItemID) as VariableNode;
				if(variableNode != null)
					DataType = TypeInfo.GetSystemType(variableNode.DataType, EncodeableFactory.GlobalFactory);
			}
			foreach (DataValue value in monitoredItem.DequeueValues())
			{
				//_quality = value.StatusCode.ToString();
				Value = (Object)value.Value;
			}
		}

		public void OnUpdated(object newValue)
		{
			this.LastUpdate = DateTime.Now;
			this.Value = newValue;
			this.Updated?.Invoke(this, new ItemEventArgs { oldValue = this.Value, newValue = newValue });
		}

		public void Read()
		{
			if(MonitorItem!=null && MonitorItem.Subscription !=null && MonitorItem.Subscription.Session!=null)
				MonitorItem.Subscription.Session.ReadValue(MonitorItem.ResolvedNodeId);
		}

		public async Task ReadAsync()
		{
			await Task.Run(() => { this.Read(); });
		}

		public void Write(object value)
		{
			if (value == null || DataType == null)
				return;
			var valueToWrite = new WriteValue();
			valueToWrite.NodeId = ItemID;
			valueToWrite.AttributeId = Attributes.Value;
			valueToWrite.Value.Value = Convert.ChangeType(value, DataType);
			valueToWrite.Value.StatusCode = StatusCodes.Good;
			valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
			valueToWrite.Value.SourceTimestamp = DateTime.MinValue;

			var valuesToWrite = new WriteValueCollection();
			valuesToWrite.Add(valueToWrite);

			MonitorItem.Subscription.Session.BeginWrite(
				 requestHeader: null,
				 nodesToWrite: valuesToWrite,
				 callback: asyncResult =>
				 {
					 var response = MonitorItem.Subscription.Session.EndWrite(
							  result: asyncResult,
							  results: out StatusCodeCollection results,
							  diagnosticInfos: out DiagnosticInfoCollection diag);
					 Console.WriteLine(response.ServiceResult.Code);
					 Console.WriteLine(results[0]);

				 },
				 asyncState: null);
		}

		public async Task WriteAsync(object value)
		{
			await Task.Run(() => { this.Write(value); });
		}
	}
}
