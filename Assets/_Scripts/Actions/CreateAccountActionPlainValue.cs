using Libplanet;
using Libplanet.Store;

namespace Flawless.Actions
{
    public class CreateAccountActionPlainValue : DataModel
    {
        public Address Address { get; private set; }
        public string Name { get; private set; }

        public CreateAccountActionPlainValue(Address address, string name)
            : base()
        {
            Address = address;
            Name = name;
        }

        // Used for deserializing stored action.
        public CreateAccountActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }
}
