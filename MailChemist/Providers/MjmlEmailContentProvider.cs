using MailChemist.Core.Entities;
using MailChemist.Core.Interfaces;
using MailChemist.Entities;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace MailChemist.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class MjmlEmailContentProvider : IEmailContentProvider
    {
        private readonly RestClient _client;
        private readonly string _mjmlApiApplicationId;
        private readonly string _mjmlApiSecretKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="mjmlApiApplicationId"></param>
        /// <param name="mjmlApiSecretKey"></param>
        public MjmlEmailContentProvider(Uri baseUri,
                                        string mjmlApiApplicationId,
                                        string mjmlApiSecretKey)
        {
            if (baseUri is null)
                throw new ArgumentNullException(nameof(baseUri));

            _mjmlApiApplicationId = mjmlApiApplicationId;
            _mjmlApiSecretKey = mjmlApiSecretKey;

            _client = new RestClient(baseUri);
            _client.Authenticator = new HttpBasicAuthenticator(_mjmlApiApplicationId, _mjmlApiSecretKey);
            _client.AddDefaultHeader("Content-type", "application/json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public EmailContentData GenerateEmailContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new EmailContentData();

            // LR: Prepare the request
            var mjmlRequest = new MjmlRenderRequest
            {
                mjml = content
            };

            // LR: Convert to json payload
            var renderMjmlPayload = JsonConvert.SerializeObject(mjmlRequest);

            var request = new RestRequest("render", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(renderMjmlPayload);

            var restResponse = _client.ExecuteAsync(request).GetAwaiter().GetResult();

            var mjmlRenderResponse = JsonConvert.DeserializeObject<MjmlRenderResponse>(restResponse.Content);

            var retVal = new EmailContentData();
            //var errors = new List<string>();
            string error;

            if (restResponse.IsSuccessful == false)
            {
                //errors.Add("MJML Render request failed");

                // if (restResponse.ErrorMessage != null)
                //    errors.Add(restResponse.ErrorMessage);

                // if (restResponse.StatusDescription != null)
                //    errors.Add(restResponse.StatusDescription);
                retVal.Error = "Failed";

                return retVal;
            }

            //if (mjmlRenderResponse.Errors != null)
            //    errors.AddRange(mjmlRenderResponse.Errors.Select(s => s.Message));

            retVal.Content = mjmlRenderResponse.Html;
            //retVal.Error = error;

            return retVal;
        }
    }
}