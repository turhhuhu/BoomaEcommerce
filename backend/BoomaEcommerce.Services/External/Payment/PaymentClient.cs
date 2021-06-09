using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External.Payment.Requests;

namespace BoomaEcommerce.Services.External.Payment
{
    public class PaymentClient : IPaymentClient
    {
        private HttpClient _httpClient;

        public PaymentClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> MakePayment(PaymentDetailsDto paymentDetails)
        {
            if (!await HandShake())
            {
                return -1;
            }
            var payRequest = new PayRequest(
                "pay",
                paymentDetails.CardNumber,
                paymentDetails.Month,
                paymentDetails.Year,
                paymentDetails.HolderName,
                paymentDetails.Ccv,
                paymentDetails.Id);
            var content = payRequest.ToFormData();
            var response = await _httpClient.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString, out var transactionId);
            if (!successAction || !IsValidTransactionId(transactionId))
                return -1;
            
            return transactionId;
        }

        public async Task<int> CancelPayment(int transactionId)
        {
            if (!await HandShake())
            {
                return -1;
            }
            var cancelPayRequest = new CancelPayRequest(
                "cancel_pay",
                transactionId.ToString());
            var content = cancelPayRequest.ToFormData();
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