namespace RentalApi.Domain.Interfaces
{
    public interface IMessageConsumer
    {
        void StartConsuming(string queueName);
    }
}