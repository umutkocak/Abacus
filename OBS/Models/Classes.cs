
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
    
public partial class Classes
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Classes()
    {

        this.ClassLessons = new HashSet<ClassLessons>();

        this.ClassStudents = new HashSet<ClassStudents>();

        this.Notes = new HashSet<Notes>();

        this.Announcements = new HashSet<Announcements>();

    }


    public int ID { get; set; }

    public string ClassName { get; set; }

    public Nullable<int> TeacherID { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ClassLessons> ClassLessons { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ClassStudents> ClassStudents { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Notes> Notes { get; set; }

    public virtual Teachers Teachers { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Announcements> Announcements { get; set; }

}

}