using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Identity.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreatorId { get; set; } = "";

        /// <summary>
        /// 否已删除
        /// </summary>
        public bool Deleted { get; set; }


        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 所属部门Id
        /// </summary>
        public string DepartmentId { get; set; } = "";

        public string OtherData { get; set; } = "";

        // 用户角色 用户权限 用户信息 用户登录tokens  重新绑定与父类的关系 命名必须和父类一致
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }

    public enum Sex
    {
        [Description("男")]
        Man = 1,

        [Description("女")]
        Woman = 0
    }
}
