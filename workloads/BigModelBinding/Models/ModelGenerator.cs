using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BigModelBinding.Models
{
    public static class ModelGenerator
    {
        public static EnrollmentService GenerateModel()
        {
            var random = new Random(5757);

            var typeFactors = new Dictionary<Type, int>()
            {
                { typeof(EnrollmentService), 5 },
                { typeof(Service), 1 },
                { typeof(Provider), 1 },
                { typeof(Client), 50000 },
                { typeof(Program), 20 },
                { typeof(Note), 1000 },
                { typeof(Doc), 1000 },
                { typeof(Contact), 500 },
                { typeof(User), 1200 },
                { typeof(ClientCareSetting), 1000 }
            };

            var entities = new Dictionary<Type, List<object>>()
            {
                { typeof(EnrollmentService), new List<object>() },
                { typeof(Enrollment), new List<object>() },
                { typeof(Service), new List<object>() },
                { typeof(Provider), new List<object>() },
                { typeof(Client), new List<object>() },
                { typeof(Program), new List<object>() },
                { typeof(Note), new List<object>() },
                { typeof(Doc), new List<object>() },
                { typeof(Contact), new List<object>() },
                { typeof(User), new List<object>() },
                { typeof(ClientCareSetting), new List<object>() }
            };

            foreach (var kvp in typeFactors)
            {
                var properties = kvp.Key.GetRuntimeProperties();
                for (var i = 0; i < kvp.Value; i++)
                {
                    var entity = Activator.CreateInstance(kvp.Key);
                    entities[kvp.Key].Add(entity);
                }
            }

            foreach (var kvp in entities)
            {
                var properties = kvp.Key.GetRuntimeProperties();
                foreach (var entity in kvp.Value)
                {
                    foreach (var property in properties)
                    {
                        if (typeFactors.ContainsKey(property.PropertyType))
                        {
                            var relatedEntities = entities[property.PropertyType];
                            property.SetValue(entity, relatedEntities[random.Next(0, relatedEntities.Count)]);
                        }
                        else if (property.PropertyType == typeof(ICollection<EnrollmentService>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(ICollection<Doc>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(ICollection<Note>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(ICollection<Provider>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(ICollection<Service>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(ICollection<Contact>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(ICollection<ClientCareSetting>))
                        {
                            property.SetValue(entity, Select(property.PropertyType, random, 5, entities));
                        }
                        else if (property.PropertyType == typeof(Address))
                        {
                            var address = new Address()
                            {
                                City = "Seattle",
                                State = "WA",
                                Street1 = Guid.NewGuid().ToString(),
                                Zip = 98004,
                            };

                            property.SetValue(entity, address);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            // Do nothing
                        }
                        else if (property.PropertyType == typeof(int?))
                        {
                            property.SetValue(entity, 0);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(entity, Guid.NewGuid().ToString());
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            // Do nothing
                        }
                    }
                }
            }

            foreach (Client client in entities[typeof(Client)])
            {
                client.Enrollments = new List<Enrollment>();
                var enrollmentCount = random.Next(0, 5);
                for (var i = 0; i < enrollmentCount; i++)
                {
                    var program = (Program)entities[typeof(Program)][random.Next(0, typeFactors[typeof(Program)])];
                    if (program.Enrollments == null)
                    {
                        program.Enrollments = new List<Enrollment>();
                    }

                    var enrollment = new Enrollment()
                    {
                        Client = client,
                        Documents = (List<Doc>)Select(typeof(ICollection<Doc>), random, 5, entities),
                        EnrollmentServices = (List<EnrollmentService>)Select(typeof(ICollection<EnrollmentService>), random, 5, entities),
                        Notes = (List<Note>)Select(typeof(ICollection<Note>), random, 5, entities),
                        PrimaryCaseManager = (User)entities[typeof(User)][random.Next(0, typeFactors[typeof(User)])],
                        SecondaryCaseManager = (User)entities[typeof(User)][random.Next(0, typeFactors[typeof(User)])],
                        Program = program,
                    };

                    client.Enrollments.Add(enrollment);
                    program.Enrollments.Add(enrollment);

                    entities[typeof(Enrollment)].Add(enrollment);
                }
            }

            return (EnrollmentService)entities[typeof(EnrollmentService)][random.Next(0, typeFactors[typeof(EnrollmentService)])];
        }

        private static object Select(Type propertyType, Random random, int count, Dictionary<Type, List<object>> entities)
        {
            var elementType = propertyType.GetGenericArguments()[0];
            var type = typeof(List<>).MakeGenericType(elementType);

            var bleh = entities[elementType];
            var list = (IList)Activator.CreateInstance(type);
            for (var i = 0; i < Math.Min(count, bleh.Count); i++)
            {
                list.Add(bleh[random.Next(0, bleh.Count)]);
            }
            return list;
        }
    }
}
