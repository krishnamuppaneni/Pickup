namespace Pickup.Mobile.Services
{
    public interface IRestClient<T>
    {
        public T GetClient();
    }
}