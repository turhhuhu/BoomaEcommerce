using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External.Supply.Requests;

namespace BoomaEcommerce.Services.External.Supply
{
    public class SupplyClient : ISupplyClient
    {
        private HttpClient _httpClient;

        public SupplyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> MakeOrder(SupplyDetailsDto supplyDetails)
        {
            var supplyItem = new SupplyRequest(
                "supply",
                supplyDetails.Name,
                supplyDetails.Address,
                supplyDetails.City,
                supplyDetails.Country,
                supplyDetails.Zip);
            var content = supplyItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString,out var transactionId);
            if (!successAction || !IsValidTransactionId(transactionId))
                return -1;
            return transactionId;
        }
        public async Task<int> CancelOrder(int transactionId)
        {
            var cancelSupplyItem = new CancelSupplyRequest(
                "cancel_supply",
                transactionId.ToString());
            var content = cancelSupplyItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString, out var responseAsInt);
            if (!successAction || responseAsInt == -1)
                return -1;
            return 1;
        }
        
        private async Task<bool> HandShake()
        {
            var handshakeRequest = new HandshakeRequest("handshake");
            var content = handshakeRequest.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString.Equals("OK");
        }
        
        private static bool IsValidTransactionId(int transactionId)
        {
            return transactionId is >= 10000 and <= 100000;
        }
    }
}