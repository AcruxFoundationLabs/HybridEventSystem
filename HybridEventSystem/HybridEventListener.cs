namespace Acrux.EventSystems.Hybrid;

/// <summary>
/// Manages the behaviour taken in the invokation of an event from a <see cref="HybridEventDispatcher{TArgs}"/>.<br></br>
/// </summary>
public class HybridEventListener<TCorroborateArgs, TClaimArgs, TInvariantArgs>
{
	/// <summary>
	/// Defines the signature for the <see cref="CorroborateClaim"/> property.
	/// </summary>
	/// <param name="args">The arguments of the raised dispatcher.</param>
	/// <returns>A <see cref="bool"/> indicating if this listener "claims" the event.</returns>
	public delegate bool CorroborateDelegate(TCorroborateArgs args);

	/// <summary>
	/// Defines the signature for the <see cref="ClaimedBehaviour"/> property.
	/// </summary>
	/// <param name="args">The arguments of the raised dispatcher.</param>
	public delegate void ClaimedBehaviourDelegate(TClaimArgs args);

	/// <summary>
	/// Defines the signature for the <see cref="InvarintBehaviour"/> property.
	/// </summary>
	/// <param name="args">The arguments of the raised dispatcher.</param>
	public delegate void InvariantBehaviourDelegate(TInvariantArgs args);

	/// <summary>
	/// Used to determinate if this listener will "claim" the event invokation to realize
	/// its defined "claiming behaviour", or if it "rejects" it.
	/// </summary>
	public CorroborateDelegate? CorroborateClaim { get; set; }

	/// <summary>
	/// The behaviour of this listener that will be executed if the event invokation is "claimed"
	/// by this <see cref="HybridEventListener{TArgs}"/>.
	/// </summary>
	public ClaimedBehaviourDelegate? ClaimedBehaviour { get; set; }

	/// <summary>
	/// The behaviour of this listener that will be executed
	/// even if the event wasn't "claimed" by this <see cref="HybridEventListener{TArgs}"/>.
	/// </summary>
	public InvariantBehaviourDelegate? InvarintBehaviour { get; set; }

	/// <summary>
	/// Used to define the order of corroborance in a <see cref="HybridEventDispatcher{TArgs}"/>.
	/// The lower the value, the more priority.
	/// </summary>
	public byte Priority
	{
		get => _priority;
		set
		{
			_priority = value;
			foreach(var dispatcher in Dispatchers)
			{
				dispatcher?.ReorderListeners();
			}
		}
	}
	private byte _priority;

	internal List<HybridEventDispatcher<TCorroborateArgs, TClaimArgs, TInvariantArgs>> Dispatchers { get; } = [];
}

/// <summary>
/// Manages the behaviour taken in the invokation of an event from a <see cref="HybridEventDispatcher{TArgs}"/>.<br></br>
/// </summary>
public class HybridEventListener<TCorroborateArgs, TBehaviourArgs> : HybridEventListener<TCorroborateArgs, TBehaviourArgs, TBehaviourArgs>
{

}

/// <summary>
/// Manages the behaviour taken in the invokation of an event from a <see cref="HybridEventDispatcher{TArgs}"/>.<br></br>
/// </summary>
public class HybridEventListener<TArgs> : HybridEventListener<TArgs, TArgs>
{

}