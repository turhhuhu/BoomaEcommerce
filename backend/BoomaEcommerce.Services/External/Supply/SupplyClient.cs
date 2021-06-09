using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.ClientRequests;
using BoomaEcommerce.Services.ClientRequests.orderRequests;
using BoomaEcommerce.Services.ClientRequests.paymentRequests;

namespace BoomaEcommerce.Services.External
{
    public class SupplyClient : ISupplyClient
    {
        private HttpClient _httpClient;

        public SupplyClient()
        {
            _httpClient = new HttpClient();
        }
        public async Task<long> MakeOrder(Purchase purchase)
        {
            var supplyItem = new SupplyItem(
                "supply",
                "matan",
                "irus",
                "irus",
                "israel",
                "112");
            var content = supplyItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  long.TryParse(responseString,out long retVal);
            if (successAction)
                return retVal;
            return -123;
        }
        public async Task<int> CancelOrder(Guid purchaseGuid)
        {
            var cancelSupplyItem = new CancelSupplyItem(
                "cancel_supply",
                "1234567");
            var content = cancelSupplyItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  Int32.TryParse(responseString,out int retVal);
            if (successAction)
                return retVal;
            return -123;
        }
        
        public async Task<string> HandShake()
        {
            var handshakeItem = new HandshakeItem("handshake");
            var content = handshakeItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}