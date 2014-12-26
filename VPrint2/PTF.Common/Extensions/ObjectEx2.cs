/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;

namespace VPrinting
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// public class AOPInitialized
    /// {
    ///     public AOPInitialized()
    ///     {
    ///         this.ApplyDefaultValues();
    ///     }
        
    ///     #region TestInterface Members
        
    ///     [DefaultValue(3)]
    ///     public int IntProperty { get; set; }
        
    ///     [DefaultValue(3)]
    ///     public long LongProperty { get; set; }
        
    ///     [DefaultValue(true)]
    ///     public bool BoolProptrty { get; set; }
        
    ///     #endregion
    /// }
    /// </example>
    public static class ObjectEx3
    {
        // Dictionary to hold type initialization methods' cache 
        private static ConcurrentDictionary<Type, Action<object>> sm_typesInitializers = new ConcurrentDictionary<Type, Action<object>>();

        /// <summary>
        /// Implements precompiled setters with embedded constant values from DefaultValueAttributes
        /// </summary>
        public static void ApplyDefaultValues(this object _this)
        {
            if (_this == null)
                return;

            Action<Object> setter = null;

            // Attempt to get it from cache
            if (!sm_typesInitializers.TryGetValue(_this.GetType(), out setter))
            {
                // If no initializers are added do nothing
                setter = (o) => { };

                // Iterate through each property
                ParameterExpression objectTypeParam = Expression.Parameter(typeof(object), "this");

                foreach (PropertyInfo prop in _this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    // Skip read only properties
                    if (!prop.CanWrite)
                        continue;

                    // There are no more then one attribute of this type
                    DefaultValueAttribute[] attr = prop.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

                    // Skip properties with no DefaultValueAttribute
                    if ((null == attr) || (null == attr[0]))
                        continue;
                    
                    Expression dva;

                    // Build the Lambda expression
#if DEBUG
                    // Make sure types do match
                    try
                    {
                        dva = Expression.Convert(Expression.Constant(attr[0].Value), prop.PropertyType);
                    }
                    catch (InvalidOperationException e)
                    {
                        string error = string.Format("Type of DefaultValueAttribute({3}{0}{3}) does not match type of property {1}.{2}",
                            attr[0].Value.ToString(), _this.GetType().Name, prop.Name, ((typeof(string) == attr[0].Value.GetType()) ? "\"" : ""));

                        throw new InvalidOperationException(error, e);
                    }
#else
                    dva = Expression.Convert(Expression.Constant(attr[0].Value), prop.PropertyType);
#endif
                    Expression setExpression = Expression.Call(Expression.TypeAs(objectTypeParam, _this.GetType()), prop.GetSetMethod(), dva);

                    Expression<Action<Object>> setLambda = Expression.Lambda<Action<Object>>(setExpression, objectTypeParam);

                    // Add this action to multicast delegate
                    setter += setLambda.Compile();
                }

                // Save in the type cache
                sm_typesInitializers.TryAdd(_this.GetType(), setter);
            }

            // Initialize member properties
            setter(_this);
        }

        /// <summary>
        /// Implements cache of ResetValue delegates
        /// </summary>
        public static void ResetDefaultValues(this object _this)
        {
            if (_this == null)
                return;

            Action<object> setter = null;

            // Attempt to get it from cache
            if (!sm_typesInitializers.TryGetValue(_this.GetType(), out setter))
            {
                // Init delegate with empty body,
                // If no initializers are added do nothing
                setter = (o) => { };

                // Go throu each property and compile Reset delegates
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(_this))
                {
                    // Add only these which values can be reset
                    if (prop.CanResetValue(_this))
                        setter += prop.ResetValue;
                }

                // Save in the type cache
                sm_typesInitializers.TryAdd(_this.GetType(), setter);
            }

            // Initialize member properties
            setter(_this);
        }
    }
}