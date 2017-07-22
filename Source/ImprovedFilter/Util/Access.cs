using System;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace ImprovedFilter.Util {
    public static class Access {

        /// <summary>
        /// Get's a <em>fast</em> accessor to a an <see cref="BindingFlags.NonPublic"/> <see cref="BindingFlags.Instance"/> property value.
        /// </summary>
        /// <typeparam name="T">Type of the owning instance</typeparam>
        /// <typeparam name="P">Type of the property</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <remarks>This access if orders of magnitude faster than using Reclection.</remarks>
        public static Func<T, P> GetPropertyGetter<T, P>([NotNull] string propertyName) {
            MethodInfo mi = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true);
            DynamicMethod dm = new DynamicMethod(String.Empty, typeof(P), new[] { typeof(T) }, typeof(T));
            var ilGen = dm.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Callvirt, mi);
            ilGen.Emit(OpCodes.Ret);

            return (Func<T, P>)dm.CreateDelegate(typeof(Func<T, P>));
        }

        /// <summary>
        /// Get's a <em>fast</em> accessor to a an <see cref="BindingFlags.NonPublic"/> <see cref="BindingFlags.Static"/> property value.
        /// </summary>
        /// <typeparam name="P">Type of the property</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="ownerType">Owning Type</param>
        /// <remarks>This access if orders of magnitude faster than using Reclection.</remarks>
        public static Func<P> GetPropertyGetter<P>([NotNull] string propertyName, [NotNull] Type ownerType) {
            MethodInfo mi = ownerType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic).GetGetMethod(true);
            DynamicMethod dm = new DynamicMethod(String.Empty, typeof(P), Type.EmptyTypes, ownerType);
            var ilGen = dm.GetILGenerator();
            ilGen.Emit(OpCodes.Callvirt, mi);
            ilGen.Emit(OpCodes.Ret);

            return (Func<P>)dm.CreateDelegate(typeof(Func<P>));
        }
    }
}
