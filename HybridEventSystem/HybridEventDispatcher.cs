namespace HES;

/// <summary>
/// Manages the invokation of an event, where different <see cref="HybridEventListener{TArgs}"/> are attached to in.<br></br>
/// When invoked the handler asks the listeners one by one in order if they want to "claim" the event or "reject" it.<br></br>
/// After a listener "claims" an event, its functionality is invoked and the dispatch finishes.
/// </summary>
/// <typeparam name="TArgs"></typeparam>
public class HybridEventDispatcher<TArgs>
{
	private List<HybridEventListener<TArgs>> Listeners { get; } = [];

	/// <summary>
	/// Notifies all <see cref="Listeners"/> in order to "claim" the event and
	/// execute the claimer behaviour.
	/// </summary>
	/// <param name="args">The arguments of the event raise.</param>
	public void Invoke(TArgs args)
	{
		Console.WriteLine("\nDispatching...");
		if (Listeners.Count == 0)
		{
			Console.WriteLine("Dispatch canceled, no listeners attached.");
			return;
		}

		bool wasEventClaimed = false;
		foreach(var listener in Listeners)
		{
			//Execute the listener invariant behaviour.
			Console.WriteLine("Invoking invariant behaviour...");
			listener.InvarintBehaviour?.Invoke(args);
			Console.WriteLine("Invariant behaviour invoked.");

			// Ask if listener wants to execute its "calimed behaviour"
			// (Only if event isn't already claimed)
			if (wasEventClaimed) continue;
			Console.WriteLine("Asking if listener will claim this event dispatch...");
			if (!(listener.CorroborateClaim?.Invoke(args) ?? false))
			{
				Console.WriteLine("Listener rejected this event dispatch.");
				continue;
			}
			Console.WriteLine("Listener claimed this event dispatch.");

			Console.WriteLine("Invoking claimed behaviour...");
			listener.ClaimedBehaviour?.Invoke(args);
			Console.WriteLine("Claimed behaviour invoked.");
			wasEventClaimed = true;
		}

		Console.WriteLine("Dispatched.");
	}

	/// <summary>
	/// Attaches an <see cref="HybridEventListener{TArgs}"/> to this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="HybridEventListener{TArgs}"/> to attach.</param>
	public void Add(HybridEventListener<TArgs> listener)
	{
		if (Listeners.Contains(listener)) return;

		Listeners.Add(listener);
		listener.Dispatchers.Add(this);
		ReorderListeners();
	}

	/// <summary>
	/// Detaches an <see cref="HybridEventListener{TArgs}"/> from this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="HybridEventListener{TArgs}"/> to attach.</param>
	public void Remove(HybridEventListener<TArgs> listener)
	{
		if (!Listeners.Contains(listener)) return;

		Listeners.Remove(listener);
		listener.Dispatchers.Remove(this);
	}

	internal void ReorderListeners()
	{
		Listeners.OrderBy(x => x.Priority);
	}
}
