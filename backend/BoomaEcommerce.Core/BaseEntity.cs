using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core
{
    public class BaseEntity : IBaseEntity
    {
        public Guid Guid { get; set; }

        protected BaseEntity(Guid guid)
        {
            Guid = guid;
        }

        protected BaseEntity()
        {
            Guid = Guid.NewGuid();
        }

    }

    public interface IBaseEntity
    {
        public Guid Guid { get; set; }
    }
}
