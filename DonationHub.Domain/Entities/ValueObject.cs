
namespace DonationHub.Domain.Entities
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract IEnumerable<object> GetAtomicValues(); 
        public bool Equals(ValueObject? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;
            var other = (ValueObject)obj; //. casting

            //. calling the values and making a equality process for these two objects one by one
            return GetAtomicValues()
                .SequenceEqual(other.GetAtomicValues());
        }

        //. Making sure that the object is of tyepe of object or another type like value object
        public bool Equals(object? obj)
        {
            //. Dealing with any object and validating if the object is value object or not before calling another
            return obj is ValueObject other && Equals(other);
        }

        //. Getting the hash values 
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var item in GetAtomicValues())
                hash.Add(item);
            return hash.ToHashCode();
        }

        
    }
}
