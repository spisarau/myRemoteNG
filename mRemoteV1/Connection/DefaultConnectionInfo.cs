﻿using System;
using System.ComponentModel;
using mRemoteNG.App;


namespace mRemoteNG.Connection
{
    public class DefaultConnectionInfo : ConnectionInfo
    {
        public static DefaultConnectionInfo Instance { get; } = new DefaultConnectionInfo();
        private readonly string[] _excludedProperties = { "Parent", "Name", "Panel", "Hostname", "Port", "Inheritance",
            "OpenConnections", "IsContainer", "IsDefault", "PositionID", "ConstantID", "TreeNode", "IsQuickConnect", "PleaseConnect" };

        private DefaultConnectionInfo()
        {
            IsDefault = true;
        }

        public void LoadFrom<TSource>(TSource sourceInstance, Func<string, string> propertyNameMutator = null)
        {
            if (propertyNameMutator == null) propertyNameMutator = a => a;
            var connectionProperties = GetProperties(_excludedProperties);
            foreach (var property in connectionProperties)
            {
                var propertyFromSource = typeof(TSource).GetProperty(propertyNameMutator(property.Name));
                if (propertyFromSource == null)
                {
                    Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                        $"DefaultConInfo-LoadFrom: Could not load {property.Name}", true);
                    continue;
                }
                var valueFromSource = propertyFromSource.GetValue(sourceInstance, null);
                
                var descriptor = TypeDescriptor.GetProperties(Instance)[property.Name];
                var converter = descriptor.Converter;
                if (converter != null && converter.CanConvertFrom(valueFromSource.GetType()))
                    property.SetValue(Instance, converter.ConvertFrom(valueFromSource), null);
                else
                    property.SetValue(Instance, valueFromSource, null);
            }
        }

        public void SaveTo<TDestination>(TDestination destinationInstance, Func<string, string> propertyNameMutator = null)
        {
            if (propertyNameMutator == null) propertyNameMutator = a => a;
            var inheritanceProperties = GetProperties(_excludedProperties);
            foreach (var property in inheritanceProperties)
            {
                try
                {
                    var propertyFromDestination = typeof(TDestination).GetProperty(propertyNameMutator(property.Name));
                    var localValue = property.GetValue(Instance, null);
                    if (propertyFromDestination == null)
                    {
                        Runtime.MessageCollector?.AddMessage(Messages.MessageClass.ErrorMsg,
                            $"DefaultConInfo-SaveTo: Could not load {property.Name}", true);
                        continue;
                    }
                    var convertedValue = Convert.ChangeType(localValue, propertyFromDestination.PropertyType);
                    propertyFromDestination.SetValue(destinationInstance, convertedValue, null);
                }
                catch (Exception ex)
                {
                    Runtime.MessageCollector?.AddExceptionStackTrace($"Error saving default connectioninfo property {property.Name}", ex);
                }
            }
        }
    }
}