using System.ComponentModel.DataAnnotations;

namespace RosterApp.Domain.Entities
{
    public class Currency : BaseEntity
    {
        [MaxLength(3)]
        public string Code { get; private set; }
        public string Symbol { get; private set; }
        public string Name { get; private set; }

        private Currency() { }

        public Currency(string code, string symbol, string name)
        {
            Code = code.ToUpper();
            Symbol = symbol;
            Name = name;

            Validate();
        }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code) || Code.Length != 3)
            { 
                throw new ArgumentException("Currency code must be 3 characters", nameof(Code)); 
            }

            if (string.IsNullOrWhiteSpace(Symbol))
            {  
                throw new ArgumentException("Symbol cannot be empty", nameof(Symbol)); 
            }

            if (string.IsNullOrWhiteSpace(Name))
            { 
                throw new ArgumentException("Name cannot be empty", nameof(Name)); 
            }
        }
    }
}
