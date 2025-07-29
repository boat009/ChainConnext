using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChainConnext.Shared.Authen
{
    public class AuthenADRequest
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }
    public class AuthenADData
    {
        public string? message { get; set; }
        public string? status { get; set; }
        public List<EmpData>? data { get; set; }
        public RawADData? raw { get; set; }
    }
    public class EmpData
    {
        public string? displayname { get; set; }
        public string? userprincipalname { get; set; }
        public string? ad_name { get; set; }
        public string? emp_id { get; set; }
        public string? user_validation { get; set; }
        public string? empid { get; set; }
        public string? position_id { get; set; }
        public string? position_name { get; set; }
        public string? division_id { get; set; }
        public string? division_name { get; set; }
        public string? depart_id { get; set; }
        public string? departname { get; set; }
        public string? company_id { get; set; }
        public string? company_name { get; set; }
        public string? emp_section { get; set; }
        public string? user_id { get; set; }
        public string? branch_id { get; set; }
        public string? section_id { get; set; }
        public string? section_name { get; set; }
        public string? abs_position_name { get; set; }
        public string? fname { get; set; }
        public string? lname { get; set; }
        public string? telno_1 { get; set; }
        public string? telno_2 { get; set; }
        public string? Status { get; set; }
        public string? birthDate { get; set; }
    }
    public class RawADData
    {
        public string? dn { get; set; }
        public string? distinguishedName { get; set; }
        public string? userPrincipalName { get; set; }
        public string? sAMAccountName { get; set; }
        public string? mail { get; set; }
        public string? lockoutTime { get; set; }
        public string? whenCreated { get; set; }
        public string? pwdLastSet { get; set; }
        public string? userAccountControl { get; set; }
        public string? employeeID { get; set; }
        public string? sn { get; set; }
        public string? givenName { get; set; }
        public string? cn { get; set; }
        public string? displayName { get; set; }
        public string? description { get; set; }
        public string? typeAD { get; set; }
        public string? comment { get; set; }
        public string? initials { get; set; }
        public string? employeeType { get; set; }
    }
}
