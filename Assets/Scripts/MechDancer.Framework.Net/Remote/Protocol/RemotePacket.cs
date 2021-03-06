namespace MechDancer.Framework.Net.Protocol {
	public sealed class RemotePacket {
		public readonly byte   Command;
		public readonly byte[] Payload;
		public readonly string Sender;

		/// <summary>
		///     从结构化数据构造
		/// </summary>
		public RemotePacket(
			string sender,
			byte   command,
			byte[] payload
		) {
			Command = command;
			Sender  = sender;
			Payload = payload;
		}

		/// <summary>
		///     解构
		/// </summary>
		public void Deconstruct(
			out string sender,
			out byte   command,
			out byte[] payload
		) {
			sender  = Sender;
			command = Command;
			payload = Payload;
		}

		public override string ToString() => $"command: {Command}, sender: {Sender}, payload: byte[{Payload.Length}]";
	}
}