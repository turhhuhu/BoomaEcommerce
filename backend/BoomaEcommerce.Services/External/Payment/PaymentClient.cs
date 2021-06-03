using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.ClientRequests.paymentRequests;

namespace BoomaEcommerce.Services.External
{
    public class PaymentClient : IPaymentClient
    {
        private HttpClient _httpClient;

        public PaymentClient()
        {
            _httpClient = new HttpClient();
        }
        public async Task<long> MakePayment(Purchase purchase)
        {
            var payItem = new PayItem(
                "pay",
                "1111",
                "1",
                "2021",
                "matan",
                "112",
                "23323322");
            var content = payItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return 1;
        }

        public async Task<int> CancelPayment(Guid purchaseGuid)
        {
            var cancelPayItem = new CancelPayItem(
                "cancel_pay",
                "1234567");
            var content = cancelPayItem.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return 1;
        }
    }
}