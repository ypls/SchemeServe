namespace SchemeServe.Models
{
    public class Cqc
    {
        public string providerId { get; set; }
        public string[] locationIds { get; set; }
        public string organisationType { get; set; }
        public string ownershipType { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string brandId { get; set; }
        public string brandName { get; set; }
        public string registrationStatus { get; set; }
        public string registrationDate { get; set; }
        public string companiesHouseNumber { get; set; }
        public string charityNumber { get; set; }
        public string website { get; set; }
        public string postalAddressLine1 { get; set; }
        public string postalAddressLine2 { get; set; }
        public string postalAddressTownCity { get; set; }
        public string postalAddressCounty { get; set; }
        public string region { get; set; }
        public string postalCode { get; set; }
        public string uprn { get; set; }
        public double onspdLatitude { get; set; }
        public double onspdLongitude { get; set; }
        public string mainPhoneNumber { get; set; }
        public string inspectionDirectorate { get; set; }
        public string constituency { get; set; }
        public string localAuthority { get; set; }
        public Date lastInspection { get; set; }
    }
        public class Date
    {
        public string date { get; set; }
    }
}
