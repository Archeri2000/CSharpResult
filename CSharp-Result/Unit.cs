using System;

namespace CSharp_Result
{
    public struct Unit
    {
        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override string ToString()
        {
            return "null";
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public static class UnitExtensions
    {
        public static Func<T1, Unit> Unit<T1>(this Action<T1> func)
        {
            return t1 =>
            {
                func(t1);
                return new Unit();
            };
        }
        public static Func<T1, T2, Unit> Unit<T1, T2>(Action<T1, T2> func)
        {
            return (t1, t2) =>
            {
                func(t1, t2);
                return new Unit();
            };
        }
        public static Func<T1, T2, T3, Unit> Unit<T1, T2, T3>(Action<T1, T2, T3> func)
        {
            return (t1, t2, t3) =>
            {
                func(t1, t2, t3);
                return new Unit();
            };
        }
        public static Func<T1, T2, T3, T4, Unit> Unit<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func)
        {
            return (t1,t2,t3,t4) =>
            {
                func(t1,t2,t3,t4);
                return new Unit();
            };
        }

        public static Func<T1, T2, T3, T4, T5, Unit> Unit<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func)
        {
            return (t1, t2, t3, t4, t5) =>
            {
                func(t1, t2, t3, t4, t5);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, Unit> Unit<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func)
        {
            return (t1, t2, t3, t4, t5, t6) =>
            {
                func(t1, t2, t3, t4, t5, t6);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> Unit<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
                return new Unit();
            };
        }   
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
                return new Unit();
            };
        }
                
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
                return new Unit();
            };
        }
                
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
                return new Unit();
            };
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Unit> Unit<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) =>
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
                return new Unit();
            };
        }
    }
}