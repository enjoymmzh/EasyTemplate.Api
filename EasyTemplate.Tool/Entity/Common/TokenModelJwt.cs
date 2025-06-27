using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTemplate.Tool.Entity.Common
{
    // 令牌token
    public class TokenModelJwt
    {
        /// <summary>
        /// Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Grade { get; set; }

        public string Name { get; set; }
    }
}
