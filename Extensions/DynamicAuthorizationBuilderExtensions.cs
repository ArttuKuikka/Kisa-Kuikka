﻿using Kisa_Kuikka.Models.DynamicAuth;
using Kisa_Kuikka.Services;

namespace Kisa_Kuikka.Extensions
{
    public static class DynamicAuthorizationBuilderExtensions
    {
        public static IDynamicAuthorizationBuilder AddSqlServerStore(this IDynamicAuthorizationBuilder builder, Action<SqlOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var sqlOptions = new SqlOptions();
            options.Invoke(sqlOptions);

            var serviceProvider = builder.Services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<SqlTableCreator>>();

            var tableCreator = new SqlTableCreator(sqlOptions, logger);
            tableCreator.CreateTable();

            scope.Dispose();
            serviceProvider.Dispose();

            builder.Services.AddSingleton(sqlOptions);
            builder.Services.AddScoped<IRoleAccessStore, RoleAccessStore>();

            return builder;
        }
    }
}
