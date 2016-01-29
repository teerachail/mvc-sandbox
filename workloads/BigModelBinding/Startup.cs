﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using BigModelBinding.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace BigModelBinding
{
    public class Startup
    {
        public static int FieldCount;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }

        public static void Main(string[] args)
        {
            var application = new WebHostBuilder()
                .UseConfiguration(WebHostConfiguration.GetDefault(args))
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }

        public static StringBuilder Formify(EnrollmentService model)
        {
            var builder = new StringBuilder();
            var set = new HashSet<object>();

            Visit(builder, set, "", model);
            if (builder[builder.Length - 1] == '&')
            {
                builder.Length--;
            }

            return builder;
        }

        private static void Visit(StringBuilder builder, HashSet<object> set, string prefix, object model)
        {
            var typeInfo = model.GetType().GetTypeInfo();
            if (!typeInfo.IsValueType && typeInfo != typeof(string).GetTypeInfo())
            {
                if (!set.Add(model))
                {
                    // Already visited this model.
                    return;
                }
            }

            if (typeInfo.Assembly == typeof(EnrollmentService).GetTypeInfo().Assembly)
            {
                // This is another POCO.
                foreach (var property in typeInfo.AsType().GetRuntimeProperties())
                {
                    var value = property.GetValue(model);
                    if (value != null)
                    {
                        Visit(builder, set, ModelNames.CreatePropertyModelName(prefix, property.Name), value);
                    }
                }
            }
            else if (
                typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeInfo) &&
                typeInfo != typeof(string).GetTypeInfo())
            {
                var i = 0;
                foreach (var item in (IEnumerable)model)
                {
                    if (item != null)
                    {
                        Visit(builder, set, ModelNames.CreateIndexModelName(prefix, i++), item);
                    }
                }
            }
            else
            {
                FieldCount++;

                // This is a simple type.
                builder.Append(prefix);
                builder.Append("=");
                builder.Append(UrlEncoder.Default.Encode(Convert.ToString(model, CultureInfo.InvariantCulture)));
                builder.Append("&");
            }
        }
    }
}
