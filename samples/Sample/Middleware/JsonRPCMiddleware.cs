﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ServerStack;
using ServerStack.Protocols.Tcp;

namespace Sample.Middleware
{
    public class JsonRPCMiddleware
    {
        private readonly Func<TcpContext, Task> _next;

        public JsonRPCMiddleware(Func<TcpContext, Task> next)
        {
            _next = next;
        }

        public Task Invoke(TcpContext context)
        {
            return _next(context);
        }
    }

    public static class JsonRPCMiddlewareExtensions
    {
        public static IApplicationBuilder<TcpContext> UseJsonRPC<T>(this IApplicationBuilder<TcpContext> app) where T : class
        {
            return app.UseJsonRPC<T>(new JsonSerializerSettings());
        }

        public static IApplicationBuilder<TcpContext> UseJsonRPC<T>(this IApplicationBuilder<TcpContext> app, JsonSerializerSettings settings) where T : class
        {
            return app.Run(ctx =>
            {
                var channel = new ServerChannel(ctx.Body, settings, app.ApplicationServices);
                channel.Bind<T>();
                return channel.StartAsync();
            });
        }
    }
}