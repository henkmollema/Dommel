using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dommel.Tests
{
    public class DommelMapperTests
    {
        //[Fact]
        //public void SetKeyPropertyResolver()
        //{
        //    try
        //    {
        //        DommelMapper.SetKeyPropertyResolver(new MyKeyPropertyResolver());
        //        Assert.IsType<MyKeyPropertyResolver>(DommelMapper.KeyPropertyResolver);
        //    }
        //    finally
        //    {
        //        DommelMapper.SetKeyPropertyResolver(new DefaultKeyPropertyResolver());
        //    }
        //}

        //[Fact]
        //public void SetPropertyResolver()
        //{
        //    try
        //    {
        //        DommelMapper.SetPropertyResolver(new MyPropertyResolver());
        //        Assert.IsType<MyPropertyResolver>(DommelMapper.PropertyResolver);
        //    }
        //    finally
        //    {
        //        DommelMapper.SetPropertyResolver(new DefaultPropertyResolver());
        //    }
        //}

        private class MyKeyPropertyResolver : IKeyPropertyResolver
        {
            public KeyPropertyInfo[] ResolveKeyProperties(Type type) => throw new NotImplementedException();
        }

        private class MyPropertyResolver : IPropertyResolver
        {
            public IEnumerable<PropertyInfo> ResolveProperties(Type type) => throw new NotImplementedException();
        }
    }
}
