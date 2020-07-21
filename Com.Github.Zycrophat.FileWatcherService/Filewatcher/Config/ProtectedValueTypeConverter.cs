using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Config
{
    public class ProtectedValueTypeConverter : System.ComponentModel.TypeConverter
    {

        /// <summary>
        /// Allow only string conversions
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ProtectedValue))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }


        /// <summary>
        /// Allow only string conversions
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)

        {

            if (sourceType == typeof(string))
            {
                return true;
            }             

            return base.CanConvertFrom(context, sourceType);
        }





        /// <summary>
        /// Convert into a string resource with a comma delimited list
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)

        {
            var cipherText = (string) value;
            if (destinationType == typeof(ProtectedValue))
            {
                return new ProtectedValue(cipherText);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }



        /// <summary>
        /// Convert back into properties from a comma delimited list.
        /// Note the list is position specific.
        /// This code doesn't demonstrate proper error handling for
        /// manually invalidated values.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context,  System.Globalization.CultureInfo culture, object value)

        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            var cipherText = (string)value;
            return new ProtectedValue(cipherText);
        }

    }
}
