using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SquaresApp.Common.DTOs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace SquaresApp.Common.SwaggerUtils
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        private readonly Random _rand = new Random();

        /// <summary>
        /// swagger ISchemaFilter for request and response example in swagger UI
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = context.Type.Name switch
            { 
                nameof(PointDTO) => new OpenApiObject
                {  
                    [nameof(PointDTO.X)] = new OpenApiLong(_rand.Next(10)),
                    [nameof(PointDTO.Y)] = new OpenApiLong(_rand.Next(10)),
                },
                nameof(GetPointDTO) => new OpenApiObject
                { 
                    [nameof(GetPointDTO.Id)] = new OpenApiLong(_rand.Next(5)),
                    [nameof(GetPointDTO.X)] = new OpenApiLong(_rand.Next(10)),
                    [nameof(GetPointDTO.Y)] = new OpenApiLong(_rand.Next(10)),
                },
                nameof(UserDTO) => new OpenApiObject
                {
                    [nameof(UserDTO.Username)] = new OpenApiString("Admin"),
                    [nameof(UserDTO.Password)] = new OpenApiString("Admin"), 
                },
                nameof(GetUserDTO) => new OpenApiObject
                {
                    [nameof(GetUserDTO.Username)] = new OpenApiString("Admin"),
                    [nameof(GetUserDTO.Password)] = new OpenApiString("Admin"),
                    [nameof(GetUserDTO.Id)] = new OpenApiLong(_rand.Next(5)),
                },
                nameof(SquareDTO) => new OpenApiObject
                {
                    [nameof(SquareDTO.A)] = new OpenApiObject
                    {
                        [nameof(GetPointDTO.Id)] = new OpenApiLong(_rand.Next(5)),
                        [nameof(GetPointDTO.X)] = new OpenApiLong(_rand.Next(10)),
                        [nameof(GetPointDTO.Y)] = new OpenApiLong(_rand.Next(10)),
                    },
                    [nameof(SquareDTO.B)] = new OpenApiObject
                    {
                        [nameof(GetPointDTO.Id)] = new OpenApiLong(_rand.Next(5)),
                        [nameof(GetPointDTO.X)] = new OpenApiLong(_rand.Next(10)),
                        [nameof(GetPointDTO.Y)] = new OpenApiLong(_rand.Next(10)),
                    },
                    [nameof(SquareDTO.C)] = new OpenApiObject
                    {
                        [nameof(GetPointDTO.Id)] = new OpenApiLong(_rand.Next(5)),
                        [nameof(GetPointDTO.X)] = new OpenApiLong(_rand.Next(10)),
                        [nameof(GetPointDTO.Y)] = new OpenApiLong(_rand.Next(10)),
                    },
                    [nameof(SquareDTO.D)] = new OpenApiObject
                    {
                        [nameof(GetPointDTO.Id)] = new OpenApiLong(_rand.Next(5)),
                        [nameof(GetPointDTO.X)] = new OpenApiLong(_rand.Next(10)),
                        [nameof(GetPointDTO.Y)] = new OpenApiLong(_rand.Next(10)),
                    },
                }, 
                _ => default
            };


        }
    }
}

