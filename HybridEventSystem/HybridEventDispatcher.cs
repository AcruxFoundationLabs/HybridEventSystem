namespace Acrux.EventSystems.Hybrid;

public class HybridEventDispatcher<TCorroborateArgs, TClaimArgs, TInvariantArgs>
{
	private List<HybridEventListener<TCorroborateArgs, TClaimArgs, TInvariantArgs>> Listeners { get; } = [];

	public void Invoke(ValueOrFunc<TCorroborateArgs> corroborateArgs, ValueOrFunc<TClaimArgs> claimArgs, ValueOrFunc<TInvariantArgs> invariantArgs)
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
			TInvariantArgs _invariantArgs = invariantArgs.IsValue ? invariantArgs.Value : invariantArgs.Func.Invoke();
			listener.InvarintBehaviour?.Invoke(_invariantArgs);
			Console.WriteLine("Invariant behaviour invoked.");

			// Ask if listener wants to execute its "calimed behaviour"
			// (Only if event isn't already claimed)
			if (wasEventClaimed) continue;
			Console.WriteLine("Asking if listener will claim this event dispatch...");
			TCorroborateArgs _corroborateArgs = corroborateArgs.IsValue ? corroborateArgs.Value : corroborateArgs.Func.Invoke();
			if (!(listener.CorroborateClaim?.Invoke(_corroborateArgs) ?? false))
			{
				Console.WriteLine("Listener rejected this event dispatch.");
				continue;
			}
			Console.WriteLine("Listener claimed this event dispatch.");

			Console.WriteLine("Invoking claimed behaviour...");
			TClaimArgs _claimArgs = claimArgs.IsValue ? claimArgs.Value : claimArgs.Func.Invoke();
			listener.ClaimedBehaviour?.Invoke(_claimArgs);
			Console.WriteLine("Claimed behaviour invoked.");
			wasEventClaimed = true;
		}

		Console.WriteLine("Dispatched.");
	}

	/// <summary>
	/// Attaches an <see cref="HybridEventListener{TArgs}"/> to this dispatcher.
	/// </summary>
	/// <param name="listener">The <see cref="HybridEventListener{TArgs}"/> to attach.</param>
	public void Add(HybridEventListener<TCorroborateArgs, TClaimArgs, TInvariantArgs> listener)
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
	public void Remove(HybridEventListener<TCorroborateArgs, TClaimArgs, TInvariantArgs> listener)
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

public class HybridEventDispatcher<TCorroborateArgs, TBehaviourArgs> : HybridEventDispatcher<TCorroborateArgs, TBehaviourArgs, TBehaviourArgs>
{
	public void Invoke(ValueOrFunc<TCorroborateArgs> corroborateArgs, ValueOrFunc<TBehaviourArgs> behaviourArgs)
	{
		Invoke(corroborateArgs, behaviourArgs, behaviourArgs);
	}
}

public class HybridEventDispatcher<TArgs> : HybridEventDispatcher<TArgs, TArgs>
{
	public void Invoke(ValueOrFunc<TArgs> args)
	{
		Invoke(args, args, args);
	}
}