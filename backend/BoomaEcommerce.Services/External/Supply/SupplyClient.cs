using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External.Supply.Requests;

namespace BoomaEcommerce.Services.External.Supply
{
    public class SupplyClient : ISupplyClient
    {
        private HttpClient _httpClient;

        public SupplyClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("externalClient");
        }
        public async Task<int> MakeOrder(SupplyDetailsDto supplyDetails)
        {
            if (!await HandShake())
            {
                throw new SupplyFailureException();
            }
            var supplyItem = new SupplyRequest(
                "supply",
                supplyDetails.Name,
                supplyDetails.Address,
                supplyDetails.City,
                supplyDetails.Country,
                supplyDetails.Zip.ToString());
            var content = supplyItem.ToFormData();
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString,out var transactionId);
            if (!successAction || !IsValidTransactionId(transactionId))
                throw new SupplyFailureException();
            return transactionId;
        }
        public async Task<int> CancelOrder(int transactionId)
        {
            if (!await HandShake())
            {
                throw new SupplyFailureException();
            }
            var cancelSupplyItem = new CancelSupplyRequest(
                "cancel_supply",
                transactionId.ToString());
            var content = cancelSupplyItem.ToFormData();
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString, out var responseAsInt);
            if (!successAction || responseAsInt == -1)
                throw new SupplyFailureException();
            return 1;
        }
        
        private async Task<bool> HandShake()
        {
            var handshakeRequest = new HandshakeRequest("handshake");
            var content = handshakeRequest.ToFormData();
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString.Equals("OK");
        }
        
        private static bool IsValidTransactionId(int transactionId)
        {
            return transactionId is >= 10000 and <= 100000;
        }
    }
}