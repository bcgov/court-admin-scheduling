﻿using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CAS.API.infrastructure
{
    public static class CustomBuilderExtensions
    {
        public static IDataProtectionBuilder UseXmlEncryptor(
            this IDataProtectionBuilder builder,
            Func<IServiceProvider, IXmlEncryptor> factory)
        {
            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
            {
                var instance = factory(serviceProvider);
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlEncryptor = instance;
                });
            });

            return builder;
        }
    }
}
