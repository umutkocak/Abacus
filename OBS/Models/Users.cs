
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace OBS.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Users
{

    public int ID { get; set; }

    public Nullable<int> TeacherId { get; set; }

    public Nullable<int> StudentId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }



    public virtual Students Students { get; set; }

    public virtual Teachers Teachers { get; set; }

}

}