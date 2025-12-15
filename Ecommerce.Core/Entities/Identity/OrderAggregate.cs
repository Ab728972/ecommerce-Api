using System.Runtime.Serialization;

namespace Ecommerce.Core.Entities.OrderAggregate
{
	public enum OrderStatus
	{
		[EnumMember(Value = "Pending")]
		Pending,
		[EnumMember(Value = "Payment Received")]
		PaymentReceived,
		[EnumMember(Value = "Payment Failed")]
		PaymentFailed
	}
}