using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework;
using IdentityServer4.Stores;
using IdentityServer4.EntityFramework.Stores;
using Microsoft.Extensions.Hosting;
using IdentityServer4.EntityFramework.DbContexts;

namespace Authzilla
{
    public static class Extensions
    {
        public static IIdentityServerBuilder AddOperationalStoreV2<TContext>(this IIdentityServerBuilder builder,Action<OperationalStoreOptions> storeOptionsAction = null)
          where TContext : PersistedGrantDbContext, IPersistedGrantDbContext
        {
            builder.Services.AddOperationalDbContextV2<TContext>(storeOptionsAction);

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            builder.Services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();

            return builder;
        }

        public static IServiceCollection AddOperationalDbContextV2<TContext>(this IServiceCollection services,
             Action<OperationalStoreOptions> storeOptionsAction = null)
             where TContext : PersistedGrantDbContext, IPersistedGrantDbContext
        {
            var storeOptions = new OperationalStoreOptions();
            services.AddSingleton(storeOptions);
            storeOptionsAction?.Invoke(storeOptions);

            if (storeOptions.ResolveDbContextOptions != null)
            {
                services.AddDbContext<TContext>(storeOptions.ResolveDbContextOptions);
            }
            else
            {
                services.AddDbContext<TContext>(dbCtxBuilder =>
                {
                    storeOptions.ConfigureDbContext?.Invoke(dbCtxBuilder);
                });
            }

            services.AddScoped<IPersistedGrantDbContext, TContext>();
            services.AddTransient<TokenCleanupService>();

            return services;
        }
    }

}
