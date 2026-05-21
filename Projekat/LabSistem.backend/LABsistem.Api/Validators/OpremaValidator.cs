using System;

namespace LABsistem.Api.Validators
{
    public interface IOpremaValidator
    {
        void ValidateSave(string naziv, string kategorija, int kabinetId);
    }

    public class OpremaValidator : IOpremaValidator
    {
        public void ValidateSave(string naziv, string kategorija, int kabinetId)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                throw new Exception("Naziv opreme je obavezan.");

            if (naziv.Trim().Length > 30)
                throw new Exception("Naziv opreme može imati najviše 30 karaktera.");

            if (string.IsNullOrWhiteSpace(kategorija))
                throw new Exception("Kategorija opreme je obavezna.");

            if (kategorija.Trim().Length > 40)
                throw new Exception("Kategorija opreme može imati najviše 40 karaktera.");

            if (kabinetId <= 0)
                throw new Exception("Kabinet mora biti odabran.");
        }
    }
}
