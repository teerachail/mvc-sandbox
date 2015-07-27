using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BigModelBinding.Models
{
    public abstract class BaseEntity<TId>
    {
        public abstract TId Id { get; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateBy { get; set; }

        public virtual bool IsTransient()
        {
            return Id.Equals(default(TId));
        }
    }

    public class EnrollmentService : BaseEntity<int>
    {
        public override int Id => EnrollmentServiceId;
        public int EnrollmentServiceId { get; set; }
        public int EnrollmentId { get; set; }
        [Required(ErrorMessage = "Service is required.")]
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Provider is required.")]
        public int? ProviderId { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        public virtual Service Service { get; set; }
        public virtual Provider Provider { get; set; }
    }

    public class Enrollment : BaseEntity<int>
    {
        public override int Id => EnrollmentId;
        public int EnrollmentId { get; set; }
        public int ClientId { get; set; }
        public virtual User PrimaryCaseManager { get; set; }
        public virtual User SecondaryCaseManager { get; set; }
        public int ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<EnrollmentService> EnrollmentServices { get; set; }
        public virtual ICollection<Doc> Documents { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }

    public class Service : BaseEntity<int>
    {
        public override int Id => ServiceId;
        public int ServiceId { get; set; }
        [Required]
        public int ProgramId { get; set; }
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string SubName { get; set; }
        public ICollection<Provider> Providers { get; set; }
        public Program Program { get; set; }
    }

    public class Provider : BaseEntity<int>
    {
        public override int Id => ProviderId;
        public int ProviderId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }

    public class Client : BaseEntity<int>
    {
        public override int Id => ClientId;
        public int ClientId { get; set; }
        public virtual Address HomeAddress { get; set; }
        public virtual ICollection<Doc> Docs { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<ClientCareSetting> CareSettings { get; set; }
    }

    public class Program : BaseEntity<int>
    {
        public override int Id => ProgramId;
        public int ProgramId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }

    public class Note : BaseEntity<int>
    {
        public override int Id => NoteId;
        public int NoteId { get; set; }
        public string Content { get; set; }
    }

    public class Doc : BaseEntity<int>
    {
        public override int Id => DocId;
        public int DocId { get; set; }
        public string Filename { get; set; }
    }

    public class Contact : BaseEntity<int>
    {
        public override int Id => ContactId;
        public int ContactId { get; set; }
    }

    public class User : BaseEntity<int>
    {
        public override int Id => UserId;
        public int UserId { get; set; }
    }

    public class ClientCareSetting : BaseEntity<int>
    {
        public override int Id => ClientCareSettingId;
        public int ClientCareSettingId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Address
    {
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
