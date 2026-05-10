using System;

namespace LABsistem.Api.Validators
{
    public interface ITerminValidator
    {
        void ValidateCreate(DateTime datum, TimeSpan pocetak, TimeSpan kraj);
    }

    public class TerminValidator : ITerminValidator
    {
        public void ValidateCreate(DateTime datum, TimeSpan pocetak, TimeSpan kraj)
        {
            if (datum.Date < DateTime.Now.Date)
                throw new Exception("Datum ne može biti u prošlosti.");
            
            if (pocetak >= kraj)
                throw new Exception("Vrijeme početka mora biti prije vremena kraja.");
        }
    }
}
