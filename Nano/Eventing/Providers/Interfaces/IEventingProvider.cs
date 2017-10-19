using System;

namespace Nano.Eventing.Providers.Interfaces
{
    /// <summary>
    /// Eventing Provider.
    /// Defines a provider for eventing in the application.
    /// </summary>
    public interface IEventingProvider : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="body"></param>
        /// <param name="persist"></param>
        void Fanout<TBody>(TBody body, bool persist = true)
            where TBody : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="callback"></param>
        void Consume<TBody>(Action<byte[]> callback)
            where TBody : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="callback"></param>
        void Receive<TBody>(Action<TBody> callback)
            where TBody : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="callback"></param>
        /// <param name="routingKey"></param>
        /// <param name="isTemporary"></param>
        void Consume<TBody>(Action<TBody> callback, string routingKey = "", bool isTemporary = false)
            where TBody : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="callback"></param>
        void Respond<TRequest, TResponse>(Func<TRequest, TResponse> callback)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        TResponse Request<TRequest, TResponse>(TRequest request) 
            where TRequest : class 
            where TResponse : class;
    }
}