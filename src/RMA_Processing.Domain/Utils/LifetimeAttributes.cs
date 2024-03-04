using System.ComponentModel.DataAnnotations;

namespace RMA_Processing.Domain.Utils
{
    public class LifetimeAttributes
    {
        public class TransientAttribute : Attribute
        {
            public TransientAttribute()
            {
            }
        }

        public class ScopedAttribute : Attribute
        {
            public ScopedAttribute()
            {
            }
        }

        public class SingletonAttribute : Attribute
        {
            public SingletonAttribute()
            {
            }
        }
    }
}