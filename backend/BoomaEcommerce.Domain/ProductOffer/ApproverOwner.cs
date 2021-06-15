using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.ProductOffer
{
    public class ApproverOwner : BaseEntity
    {
        public StoreOwnership Approver { get; set; }

    }
}
