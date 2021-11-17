using ASP_Trello_Identity.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_Trello_Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UserBoards = new HashSet<UserBoard>();
            UserWorkspaces = new HashSet<UserWorkspace>();
        }

        [PersonalData]
        [Column(TypeName = "nvarchar(127)")]
        public string FullName { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(max)")]
        public string Info { get; set; }

        
        public virtual ICollection<UserBoard> UserBoards { get; set; }
        
        public virtual ICollection<UserWorkspace> UserWorkspaces { get; set; }
    }
}
