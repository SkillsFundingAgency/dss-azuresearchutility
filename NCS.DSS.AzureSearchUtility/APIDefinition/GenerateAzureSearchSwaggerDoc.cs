using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NCS.DSS.AzureSearchUtility.Annotations;
using Newtonsoft.Json;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace NCS.DSS.AzureSearchUtility.APIDefinition
{
    public static class GenerateAzureSearchSwaggerDoc
    {
        public const string APITitle = "Customer Search";
        public const string APIDefRoute = APITitle;
        public const string APIDescription = "National Careers Service " + APITitle + " Service";

        public static string GenerateSwaggerDoc(string hostName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            dynamic doc = new ExpandoObject();
            doc.swagger = "2.0";
            doc.info = new ExpandoObject();
            doc.info.title = APITitle;
            doc.info.version = "1.0.0";
            doc.info.description = APIDescription;
            doc.host = !string.IsNullOrEmpty(hostName) ? hostName + ".search.windows.net" : string.Empty;
            doc.basePath = "/";
            doc.schemes = new[] { "https" };
            if (doc.host.Contains("127.0.0.1") || doc.host.Contains("localhost"))
            {
                doc.schemes = new[] { "http" };
            }

            doc.definitions = new ExpandoObject();
            doc.paths = GeneratePaths(assembly, doc);
            doc.securityDefinitions = GenerateSecurityDefinitions();

            return JsonConvert.SerializeObject(doc);
        }

        private static dynamic GenerateSecurityDefinitions()
        {
            dynamic securityDefinitions = new ExpandoObject();
            securityDefinitions.apikeyQuery = new ExpandoObject();
            securityDefinitions.apikeyQuery.type = "apiKey";
            securityDefinitions.apikeyQuery.name = "code";
            securityDefinitions.apikeyQuery.@in = "query";
            return securityDefinitions;
        }

        private static dynamic GeneratePaths(Assembly assembly, dynamic doc)
        {
            dynamic paths = new ExpandoObject();
            
                var route = "/api/";

                dynamic path = new ExpandoObject();

                var verbs = new[] { "get" };
                foreach (string verb in verbs)
                {
                    dynamic operation = new ExpandoObject();
                    operation.operationId = ToTitleCase("Get");
                    operation.produces = new[] { "application/json" };
                    operation.consumes = new[] { "application/json" };
                    operation.parameters = GenerateFunctionParametersSignature(route, doc);

                    // Summary is title
                    operation.summary = "Customer Search";
                    // Verbose description
                    operation.description = "This is the description for Customer Search";

                    operation.responses = GenerateResponseParameterSignature(doc);
                    operation.tags = new[] { APITitle };

                    dynamic keyQuery = new ExpandoObject();
                    keyQuery.apikeyQuery = new string[0];
                    operation.security = new ExpandoObject[] { keyQuery };

                    AddToExpando(path, verb, operation);
                }
                AddToExpando(paths, route, path);
            
            return paths;
        }

        private static string GetFunctionDescription(string funcName)
        {
            return $"This function will run {funcName}";
        }

        /// <summary>
        /// Max 80 characters in summary/title
        /// </summary>
        private static string GetFunctionName(string funcName)
        {
            return $"Run {funcName}";
        }

        private static string GetPropertyDescription(PropertyInfo propertyInfo)
        {
            var displayAttr = (DisplayAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();

            return !string.IsNullOrWhiteSpace(displayAttr?.Description) ? displayAttr.Description : $"This returns {propertyInfo.PropertyType.Name}";
        }

        private static dynamic GenerateResponseParameterSignature(dynamic doc)
        {
            dynamic responses = new ExpandoObject();
            dynamic responseDef = new ExpandoObject();

            var returnType = typeof(Models.CustomerSearch);
              
            if (returnType != typeof(void))
            {
                responseDef.schema = new ExpandoObject();

                if (returnType.Namespace == "System")
                {
                    // Warning:
                    // Allthough valid, it's always better to wrap single values in an object
                    // Returning { Value = "foo" } is better than just "foo"
                    SetParameterType(returnType, responseDef.schema, null);
                }
                else
                {
                    string name = returnType.Name;
                    if (returnType.IsGenericType)
                    {
                        var realType = returnType.GetGenericArguments()[0];
                        if (realType.Namespace == "System")
                        {
                            dynamic inlineSchema = GetObjectSchemaDefinition(null, returnType);
                            responseDef.schema = inlineSchema;
                        }
                        else
                        {
                            AddToExpando(responseDef.schema, "$ref", "#/definitions/" + name);
                            AddParameterDefinition((IDictionary<string, object>)doc.definitions, returnType);
                        }
                    }
                    else
                    {
                        AddToExpando(responseDef.schema, "$ref", "#/definitions/" + name);
                        AddParameterDefinition((IDictionary<string, object>)doc.definitions, returnType);
                    }
                }
            }

            var httpStatusCodes = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.OK, "customer search results"},
                { HttpStatusCode.BadRequest, "Bad Request"},
                { HttpStatusCode.Forbidden, "Forbidden"},
                { (HttpStatusCode)429, "Too Many Request"},
                { HttpStatusCode.ServiceUnavailable, "Service Unavailable"},
            };

            foreach (var httpStatusCode in httpStatusCodes)
            {
                if (httpStatusCode.Key != HttpStatusCode.OK){
                    responseDef = new ExpandoObject();
                }

                var status = (int)httpStatusCode.Key;
                var description = httpStatusCode.Value;

                responseDef.description = description;
                AddToExpando(responses, status.ToString(), responseDef);
            }

            return responses;
        }

        private static List<object> GenerateFunctionParametersSignature(string route, dynamic doc)
        {
            var parameterSignatures = new List<object>();

            dynamic opApiKeyParam = new ExpandoObject();
            opApiKeyParam.@in = "header";
            opApiKeyParam.name = "api-key";
            SetParameterType(typeof(string), opApiKeyParam, null);
            opApiKeyParam.required = true;
            parameterSignatures.Add(opApiKeyParam);

            //dynamic eoCount = new ExpandoObject();
            //var opCountParam = (IDictionary<string, object>) eoCount;

            //opCountParam.Add("in", "query");
            //opCountParam.Add("name", "$count");
            //opCountParam.Add("required", false);
            //opCountParam.Add("description", "Specifies whether to fetch the total count of results.");
            //SetParameterType(typeof(Boolean), opCountParam, null);
            //parameterSignatures.Add(opCountParam);
            
            var eoTop = new ExpandoObject();
            var opTopParam = (IDictionary<string, object>) eoTop;
            opTopParam.Add("in", "query");
            opTopParam.Add("name", "$top");
            opTopParam.Add("required", false);
            opTopParam.Add("description", "The number of search results to retrieve.");
            opTopParam.Add("x-example", "$top=100");
            SetParameterType(typeof(int), opTopParam, null);
            parameterSignatures.Add(opTopParam);


            dynamic eoSearch = new ExpandoObject();
            var opSearchParam = (IDictionary<string, object>) eoSearch;
            opSearchParam.Add("in", "query");
            opSearchParam.Add("name", "search");
            opSearchParam.Add("required", false);
            opSearchParam.Add("description", "search=GivenName:John\n" +
                                        $"This search result will bring back all the customers which have a GivenName equal to 'John' \n" +
                                    "search = GivenName:Joh *" +
                                    $"this search result will bring back any names that start with 'Joh' \n");
            opSearchParam.Add("x-example", "search=*");
            SetParameterType(typeof(string), opSearchParam, null);
            parameterSignatures.Add(opSearchParam);
            

            dynamic eoFilter = new ExpandoObject();
            var opFilterParam = (IDictionary<string, object>) eoFilter;
            opFilterParam.Add("in", "query");
            opFilterParam.Add("name", "$filter");
            opFilterParam.Add("required", false);
            opFilterParam.Add("description", "To search DOB you need to preform a filter on the index.");
            opFilterParam.Add("x-example", "$filter=DateofBirth eq 2005-07-26");
            SetParameterType(typeof(string), opFilterParam, null);
            parameterSignatures.Add(opFilterParam);

            dynamic eoOrderBy = new ExpandoObject();
            var opOrderByParam = (IDictionary<string, object>) eoOrderBy;
            opOrderByParam.Add("in", "query");
            opOrderByParam.Add("name", "$orderby");
            opOrderByParam.Add("required", false);
            opOrderByParam.Add("description", "A list of comma-separated expressions to sort the results by.");
            opOrderByParam.Add("x-example", "$orderby=");
            SetParameterType(typeof(string), opOrderByParam, null);
            parameterSignatures.Add(opOrderByParam);

            dynamic eoSelect = new ExpandoObject();
            var opSelectParam = (IDictionary<string, object>) eoSelect;
            opSelectParam.Add("in", "query");
            opSelectParam.Add("name", "$select");
            opSelectParam.Add("required", false);
            opSelectParam.Add("description", "A list of comma-separated fields to include in the result set.");
            opSelectParam.Add("x-example", "$select=CustomerId, GivenName, FamilyName");
            SetParameterType(typeof(string), opSelectParam, null);
            parameterSignatures.Add(opSelectParam);

            dynamic eoSkip = new ExpandoObject();
            var opSkipParam = (IDictionary<string, object>) eoSkip;
            opSkipParam.Add("in", "query");
            opSkipParam.Add("name","$skip");
            opSkipParam.Add("required", false);
            opSkipParam.Add("description", "The number of search results to skip.");
            opSkipParam.Add("x-example", "$skip=100");
            SetParameterType(typeof(int), opSkipParam, null);
            parameterSignatures.Add(opSkipParam);

            //dynamic eoScoringProfile = new ExpandoObject();
            //var opScoringProfileParam = (IDictionary<string, object>) eoScoringProfile;
            //opScoringProfileParam.Add("in", "query");
            //opScoringProfileParam.Add("name", "scoringProfile");
            //opScoringProfileParam.Add("required", false);
            //opScoringProfileParam.Add("description", "The name of a scoring profile to evaluate match scores for matching documents in order to sort the results.");
            //opScoringProfileParam.Add("x-example", "scoringProfile=CustomerScoringProfile");
            //SetParameterType(typeof(string), opScoringProfileParam, null);
            //parameterSignatures.Add(opScoringProfileParam);

            return parameterSignatures;
        }

        private static void AddObjectProperties(Type t, string parentName, List<object> parameterSignatures, dynamic doc)
        {
            var publicProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in publicProperties)
            {
                if (!string.IsNullOrWhiteSpace(parentName))
                {
                    parentName += ".";
                }
                if (property.PropertyType.Namespace != "System")
                {
                    AddObjectProperties(property.PropertyType, parentName + property.Name, parameterSignatures, doc);
                }
                else
                {
                    dynamic opParam = new ExpandoObject();

                    opParam.name = parentName + property.Name;
                    opParam.@in = "query";
                    opParam.required = false;
                    opParam.description = GetPropertyDescription(property);
                    SetParameterType(property.PropertyType, opParam, doc.definitions);
                    parameterSignatures.Add(opParam);
                }
            }
        }

        private static void AddParameterDefinition(IDictionary<string, object> definitions, Type parameterType)
        {
            dynamic objDef;
            if (!definitions.TryGetValue(parameterType.Name, out objDef))
            {
                objDef = GetObjectSchemaDefinition(definitions, parameterType);
                definitions.Add(parameterType.Name, objDef);
            }
        }

        private static dynamic GetObjectSchemaDefinition(IDictionary<string, object> definitions, Type parameterType)
        {
            dynamic objDef = new ExpandoObject();
            objDef.type = "object";
            objDef.properties = new ExpandoObject();
            var publicProperties = parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> requiredProperties = new List<string>();
            foreach (PropertyInfo property in publicProperties)
            {
                dynamic propDef = new ExpandoObject();
                propDef.description = GetPropertyDescription(property);

                var exampleAttribute = (Example)property.GetCustomAttributes(typeof(Example), false).FirstOrDefault();
                if (exampleAttribute != null)
                    propDef.example = exampleAttribute.Description;

                SetParameterType(property.PropertyType, propDef, definitions);
                AddToExpando(objDef.properties, property.Name, propDef);
            }
            if (requiredProperties.Count > 0)
            {
                objDef.required = requiredProperties;
            }
            return objDef;
        }

        private static void SetParameterType(Type parameterType, dynamic opParam, dynamic definitions)
        {
            var inputType = parameterType;
            string paramType = parameterType.UnderlyingSystemType.ToString();

            var isEnum = parameterType.IsEnum;
            var isNullableEnum = Nullable.GetUnderlyingType(parameterType)?.IsEnum == true;

            var setObject = opParam;
            if ((inputType.IsArray || inputType.GetInterface(typeof(System.Collections.IEnumerable).Name, false) != null) && inputType != typeof(String))
            {
                opParam.type = "array";
                opParam.items = new ExpandoObject();
                setObject = opParam.items;

                if (inputType.IsArray)
                {
                    parameterType = parameterType.GetElementType();
                }
                else if (inputType.IsGenericType && inputType.GenericTypeArguments.Length == 1)
                {
                    parameterType = inputType.GetGenericArguments()[0];
                }

            }

            if (inputType.Namespace == "System" && !isNullableEnum && !isEnum || (inputType.IsGenericType && inputType.GetGenericArguments()[0].Namespace == "System"))
            {
                if (paramType.Contains("System.String"))
                {
                    setObject.type = "string";
                }
                else if (paramType.Contains("System.DateTime"))
                {
                    setObject.format = "date";
                    setObject.type = "string";
                }
                else if (paramType.Contains("System.Int32"))
                {
                    setObject.format = "int32";
                    setObject.type = "integer";
                }
                else if (paramType.Contains("System.Int64"))
                {
                    setObject.format = "int64";
                    setObject.type = "integer";
                }
                else if (paramType.Contains("System.Single"))
                {
                    setObject.format = "float";
                    setObject.type = "number";
                }
                else if (paramType.Contains("System.Boolean"))
                {
                    setObject.type = "boolean";
                }
                else
                {
                    setObject.type = "string";
                }

            }
            else if (isEnum || isNullableEnum)
            {
                opParam.type = "string";
                var enumValues = new List<string>();

                if (isEnum)
                {
                    foreach (var item in Enum.GetValues(inputType))
                    {
                        var enumName = inputType.GetEnumName(item);

                        if (enumName != null)
                        {
                            var memInfo = inputType.GetMember(enumName);
                            var descriptionAttributes =
                                memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                            var description = string.Empty;

                            if (descriptionAttributes.Length > 0)
                                description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                            if (string.IsNullOrEmpty(description))
                                description = item.ToString();

                            enumValues.Add(Convert.ToInt32(item) + " - " + description);
                        }
                    }
                }

                if (isNullableEnum)
                {
                    var enumType = Nullable.GetUnderlyingType(inputType);

                    if (enumType != null)
                    {
                        foreach (var item in Enum.GetValues(enumType))
                        {
                            var memInfo = Nullable.GetUnderlyingType(inputType)?.GetMember(item.ToString());
                            var descriptionAttributes =
                                memInfo?[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                            var description = string.Empty;

                            if (descriptionAttributes?.Length > 0)
                                description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                            if (string.IsNullOrEmpty(description))
                                description = item.ToString();

                            enumValues.Add(Convert.ToInt32(item) + " - " + description);
                        }

                    }
                }

                if (enumValues.Any())
                    opParam.@enum = enumValues.ToArray();

            }
            else if (definitions != null)
            {
                AddToExpando(setObject, "$ref", "#/definitions/" + parameterType.Name);
                AddParameterDefinition((IDictionary<string, object>)definitions, parameterType);
            }
        }

        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        public static void AddToExpando(ExpandoObject obj, string name, object value)
        {
            if (((IDictionary<string, object>)obj).ContainsKey(name))
            {
                // Fix for functions with same routes but different verbs
                var existing = (IDictionary<string, object>)((IDictionary<string, object>)obj)[name];
                var append = (IDictionary<string, object>)value;
                foreach (KeyValuePair<string, object> keyValuePair in append)
                {
                    existing.Add(keyValuePair);
                }
            }
            else
            {
                ((IDictionary<string, object>)obj).Add(name, value);
            }
        }
    }
}