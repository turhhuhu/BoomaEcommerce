using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External.Payment.Requests;

namespace BoomaEcommerce.Services.External.Payment
{
    public class PaymentClient : IPaymentClient
    {
        private HttpClient _httpClient;

        public PaymentClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("externalClient");
        }
        public async Task<int> MakePayment(PaymentDetailsDto paymentDetails)
        {
            if (!await HandShake())
            {
                throw new PaymentFailureException();
            }
            var payRequest = new PayRequest(
                "pay",
                paymentDetails.CardNumber.ToString(),
                paymentDetails.Month.ToString(),
                paymentDetails.Year.ToString(),
                paymentDetails.HolderName,
                paymentDetails.Ccv.ToString(),
                paymentDetails.Id.ToString());
            var content = payRequest.ToFormData();
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString, out var transactionId);
            if (!successAction || !IsValidTransactionId(transactionId))
                throw new PaymentFailureException();
            
            return transactionId;
        }

        public async Task<int> CancelPayment(int transactionId)
        {
            if (!await HandShake())
            {
                throw new PaymentFailureException();
            }
            var cancelPayRequest = new CancelPayRequest(
                "cancel_pay",
                transactionId.ToString());
            var content = cancelPayRequest.ToFormData();
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var successAction =  int.TryParse(responseString, out var responseAsInt);
            if (!successAction || responseAsInt == -1)
                throw new PaymentFailureException();
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