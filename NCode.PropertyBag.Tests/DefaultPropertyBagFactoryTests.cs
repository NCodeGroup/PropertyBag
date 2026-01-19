namespace NCode.PropertyBag.Tests;

public class DefaultPropertyBagFactoryTests
{
    #region Singleton Property Tests

    [Fact]
    public void Singleton_ReturnsNonNullInstance()
    {
        var singleton = DefaultPropertyBagFactory.Singleton;

        Assert.NotNull(singleton);
    }

    [Fact]
    public void Singleton_ReturnsIPropertyBagFactory()
    {
        var singleton = DefaultPropertyBagFactory.Singleton;

        Assert.IsAssignableFrom<IPropertyBagFactory>(singleton);
    }

    [Fact]
    public void Singleton_ReturnsSameInstanceOnMultipleCalls()
    {
        var singleton1 = DefaultPropertyBagFactory.Singleton;
        var singleton2 = DefaultPropertyBagFactory.Singleton;

        Assert.Same(singleton1, singleton2);
    }

    [Fact]
    public void Singleton_CanBeReplaced()
    {
        var originalSingleton = DefaultPropertyBagFactory.Singleton;

        try
        {
            var customFactory = new DefaultPropertyBagFactory();
            DefaultPropertyBagFactory.Singleton = customFactory;

            Assert.Same(customFactory, DefaultPropertyBagFactory.Singleton);
        }
        finally
        {
            DefaultPropertyBagFactory.Singleton = originalSingleton;
        }
    }

    #endregion

    #region Create Method Tests

    [Fact]
    public void Create_ReturnsNonNullInstance()
    {
        var factory = new DefaultPropertyBagFactory();

        var propertyBag = factory.Create();

        Assert.NotNull(propertyBag);
    }

    [Fact]
    public void Create_ReturnsIPropertyBag()
    {
        var factory = new DefaultPropertyBagFactory();

        var propertyBag = factory.Create();

        Assert.IsAssignableFrom<IPropertyBag>(propertyBag);
    }

    [Fact]
    public void Create_ReturnsDefaultPropertyBag()
    {
        var factory = new DefaultPropertyBagFactory();

        var propertyBag = factory.Create();

        Assert.IsType<DefaultPropertyBag>(propertyBag);
    }

    [Fact]
    public void Create_ReturnsEmptyPropertyBag()
    {
        var factory = new DefaultPropertyBagFactory();
        var key = new PropertyBagKey<string>("TestKey");

        var propertyBag = factory.Create();

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Create_ReturnsNewInstanceEachCall()
    {
        var factory = new DefaultPropertyBagFactory();

        var propertyBag1 = factory.Create();
        var propertyBag2 = factory.Create();

        Assert.NotSame(propertyBag1, propertyBag2);
    }

    [Fact]
    public void Create_ReturnedInstancesAreIndependent()
    {
        var factory = new DefaultPropertyBagFactory();
        var key = new PropertyBagKey<string>("TestKey");

        var propertyBag1 = factory.Create();
        var propertyBag2 = factory.Create();

        propertyBag1.Set(key, "Value1");

        Assert.True(propertyBag1.TryGetValue(key, out var value1));
        Assert.Equal("Value1", value1);
        Assert.False(propertyBag2.TryGetValue(key, out _));
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void Factory_ImplementsIPropertyBagFactory()
    {
        var factory = new DefaultPropertyBagFactory();

        Assert.IsAssignableFrom<IPropertyBagFactory>(factory);
    }

    [Fact]
    public void Create_ViaInterfaceReference_Works()
    {
        IPropertyBagFactory factory = new DefaultPropertyBagFactory();

        var propertyBag = factory.Create();

        Assert.NotNull(propertyBag);
        Assert.IsType<DefaultPropertyBag>(propertyBag);
    }

    #endregion

    #region Singleton Create Tests

    [Fact]
    public void Singleton_Create_ReturnsValidPropertyBag()
    {
        var propertyBag = DefaultPropertyBagFactory.Singleton.Create();

        Assert.NotNull(propertyBag);
        Assert.IsAssignableFrom<IPropertyBag>(propertyBag);
    }

    [Fact]
    public void Singleton_Create_ReturnsNewInstanceEachCall()
    {
        var propertyBag1 = DefaultPropertyBagFactory.Singleton.Create();
        var propertyBag2 = DefaultPropertyBagFactory.Singleton.Create();

        Assert.NotSame(propertyBag1, propertyBag2);
    }

    #endregion
}
