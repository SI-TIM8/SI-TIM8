using System;

namespace LABsistem.Api.Validators
{
    public interface IOpremaValidator
    {
        void ValidateCreate(string naziv, int kabinetId);
    }

    public class OpremaValidator : IOpremaValidator
    {
        public void ValidateCreate(string naziv, int kabinetId)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                throw new Exception("Naziv opreme je obavezan.");
            
            if (kabinetId <= 0)
                throw new Exception("Kabinet mora biti odabran.");
        }
    }
}
