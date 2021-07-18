using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Model.Models
{
    public class AuthToken
    {
        public string AuthTokenId { get; set; }

        public string Value { get; set; }

        public string UserTokenId { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
