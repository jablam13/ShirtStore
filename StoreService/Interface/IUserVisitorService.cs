using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Interface
{
    public interface IUserVisitorService
    {
        Task<Guid> GetVisitorUid();
        string GetIpValue();
    }
}
