namespace HES.Tests;

[TestClass]
public class EventDispatcherTests
{
	class CommandSentData
	{
		public string[] Keywords { get; init; }
	}

	[TestMethod]
	public void Test()
	{
		// Dispatcher creation
		HybridEventDispatcher<CommandSentData> onCommandSent = new();

		// Listener 1
		HybridEventListener<CommandSentData> printListener = new();
		printListener.Priority = 0;
		printListener.CorroborateClaim = (CommandSentData data) =>
		{
			if (data.Keywords[0] == "print") return true;
			return false;
		};
		printListener.ClaimedBehaviour = (CommandSentData data) =>
		{
			Console.WriteLine(data.Keywords[1]);
		};
		printListener.InvarintBehaviour = (CommandSentData data) =>
		{
			Console.WriteLine("[print] event is invoked!");
		};
		onCommandSent.Add(printListener);

		// Listener 2
		HybridEventListener<CommandSentData> sumListener = new();
		sumListener.Priority = 0;
		sumListener.CorroborateClaim = (CommandSentData data) =>
		{
			if (data.Keywords[0] == "party") return true;
			return false;
		};
		sumListener.ClaimedBehaviour = (CommandSentData data) =>
		{
			Console.WriteLine("ITS TIME TO PARTY!!!");
		};
		sumListener.InvarintBehaviour = (CommandSentData data) =>
		{
			Console.WriteLine("[party] event is invoked!");
		};
		onCommandSent.Add(sumListener);

		// Disptcher invokation
		onCommandSent.Invoke( new(){Keywords = ["print", "Hi"]} );
		onCommandSent.Invoke(new() { Keywords = ["party"] });
		onCommandSent.Invoke(new() { Keywords = ["invalid"] });
	}
}