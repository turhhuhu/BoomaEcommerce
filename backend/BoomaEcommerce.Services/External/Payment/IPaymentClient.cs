using System;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.External.Payment
{
    public interface IPaymentClient
    {
        
        public Task<int> MakePayment(PaymentDetailsDto paymentDetails);

        public Task<int> CancelPayment(int transactionId);
    }
}