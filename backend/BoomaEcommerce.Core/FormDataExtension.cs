using System.Net.Http;

namespace BoomaEcommerce.Core
{
    public static class FormDataExtension
    {
        public static FormUrlEncodedContent ToFormData(this object obj)
        {
            var formData = obj.ToKeyValue();
 
            return new FormUrlEncodedContent(formData);
        }
    }
}