using SOLID_Prac1.Interface;

namespace SOLID_Prac1.Services
{
    public class ValidatedReportDecorator : IReport
    {
        private readonly IReport _inner;
        private readonly IEnumerable<IReportValidator> _validators;

        public ValidatedReportDecorator(IReport inner, IEnumerable<IReportValidator> validators)
        {
            _inner = inner;
            _validators = validators;
        }

        public string Generate()
        {
            var content = _inner.Generate();

            foreach (var validator in _validators)
            {
                validator.Validate(content);
            }

            return content;
        }
    }
}
