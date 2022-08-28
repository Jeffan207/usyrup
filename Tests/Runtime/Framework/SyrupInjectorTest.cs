using System.Collections;
using NUnit.Framework;
using Syrup.Framework;
using Syrup.Framework.Exceptions;
using Tests.Framework.TestData;
using Tests.Framework.TestModules;
using UnityEngine.TestTools;

public class SyrupInjectorTest {

    SyrupInjectorOptions OPTIONS = new() {
        VerboseLogging = true
    };


    [Test]
    public void TestSyrupInjector_WithSingleProviderModule() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingleProviderModule());
        TastySyrup tastySyrup1 = syrupInjector.GetInstance<TastySyrup>();
        TastySyrup tastySyrup2 = syrupInjector.GetInstance<TastySyrup>();

        //Two unique instances should be created
        Assert.AreNotEqual(tastySyrup1.id, tastySyrup2.id);
    }

    [Test]
    public void TestSyrupInjector_WithTwoDependentProvidersModule() {
        SyrupInjector syrupInjector = new SyrupInjector(new TwoDependentProvidersModule());
        Pancake pancake1 = syrupInjector.GetInstance<Pancake>();
        Pancake pancake2 = syrupInjector.GetInstance<Pancake>();

        //Assert both pancakes are completely different
        Assert.AreNotEqual(pancake1.id, pancake2.id);
        Assert.AreNotEqual(pancake1.flour.id, pancake2.flour.id);

        //Assert you can still get some flour
        Flour flour = syrupInjector.GetInstance<Flour>();
        Assert.AreNotEqual(pancake1.flour.id, flour.id);
    }

    [Test]
    public void TestSyrupInjector_WithModuleThatsMissingADependencyThrowsException() {
        Assert.Throws<MissingDependencyException>(() => {
            new SyrupInjector(new ProviderWithMissingDependencyModule());
        });
    }

    [Test]
    public void TestSyrupInjector_WithSingleNamedProviderModule() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingleNamedProviderModule());
        TastySyrup mapleSyrup1 = syrupInjector.GetInstance<TastySyrup>("MapleSyrup");
        TastySyrup mapleSyrup2 = syrupInjector.GetInstance<TastySyrup>("MapleSyrup");
        Assert.AreNotEqual(mapleSyrup1.id, mapleSyrup2.id);

        //And the unnamed version of the dependency doesn't exist
        Assert.Throws<MissingDependencyException>(() => {
            syrupInjector.GetInstance<TastySyrup>();
        });
    }

    [Test]
    public void TestSyrupInjector_WithTwoNamedProvidersModule() {
        SyrupInjector syrupInjector = new SyrupInjector(new TwoNamedProvidersModule());
        Pancake pancake1 = syrupInjector.GetInstance<Pancake>("FluffyPancake");
        Pancake pancake2 = syrupInjector.GetInstance<Pancake>("FluffyPancake");

        //Assert both pancakes are completely different
        Assert.AreNotEqual(pancake1.id, pancake2.id);
        Assert.AreNotEqual(pancake1.flour.id, pancake2.flour.id);

        //Assert you can still get some flour
        Flour flour = syrupInjector.GetInstance<Flour>("WholeGrainFlour");
        Assert.AreNotEqual(pancake1.flour.id, flour.id);
    }

    [Test]
    public void TestSyrupInjector_WithDuplicateProvidersThrowsException() {
        Assert.Throws<DuplicateProviderException>(() => {
            new SyrupInjector(new DuplicateProvidersModule());
        });
    }

    [Test]
    public void TestSyrupInjector_WithDuplicateNamedProvidersThrowsException() {
        Assert.Throws<DuplicateProviderException>(() => {
            new SyrupInjector(new DuplicateNamedProvidersModule());
        });
    }

    [Test]
    public void TestSyrupInjector_WithProviderThatTakesTwoOfTheSameParameter() {
        SyrupInjector syrupInjector = new SyrupInjector(new DuplicateProviderParametersModule());
        Breakfast breakfast = syrupInjector.GetInstance<Breakfast>();
        Pancake pancake1 = breakfast.pancakes[0];
        Pancake pancake2 = breakfast.pancakes[1];

        //The two pancakes in our breakfast should be different pancakes!
        Assert.AreNotEqual(pancake1.id, pancake2.id);
    }

    [Test]
    public void TestSyrupInjector_WithEmptyModuleUsesConstructorInjection() {
        SyrupInjector syrupInjector = new SyrupInjector(new EmptyModule());
        Egg egg1 = syrupInjector.GetInstance<Egg>();
        Egg egg2 = syrupInjector.GetInstance<Egg>();

        //Two unique eggs should be injected
        Assert.AreNotEqual(egg1.id, egg2.id);
        Assert.AreEqual(Egg.SCRAMBLED_EGGS, egg1.style);
    }

    [Test]
    public void TestSyrupInjector_FavorsProviderOverConstructorInjection() {
        SyrupInjector syrupInjector = new SyrupInjector(new ProvidedEggModule());
        Egg egg = syrupInjector.GetInstance<Egg>();

        Assert.AreEqual(Egg.SUNNY_SIDE_UP_EGGS, egg.style);
    }

    [Test]
    public void TestSyrupInjector_FollowsConstructorInjectedHeirarchy() {
        SyrupInjector syrupInjector = new SyrupInjector(new EmptyModule());
        Omelette omelette1 = syrupInjector.GetInstance<Omelette>();
        Omelette omelette2 = syrupInjector.GetInstance<Omelette>();

        Assert.AreNotEqual(omelette1.id, omelette2.id);
        Assert.AreNotEqual(omelette1.egg.id, omelette2.egg.id);
    } 

    [Test]
    public void TestSyrupInjector_WhereAProvidedParameterIsPassedToAnInjectedConstructor() {
        SyrupInjector syrupInjector = new SyrupInjector(new ProvidedEggModule());
        Omelette omelette = syrupInjector.GetInstance<Omelette>();
        Assert.AreEqual(Egg.SUNNY_SIDE_UP_EGGS, omelette.egg.style);
    }

    [Test]
    public void TestSyrupInjector_InjectsTwoOfTheSameParameterToAnInjectedConstructor() {
        SyrupInjector syrupInjector = new SyrupInjector(new EmptyModule());
        TwoEggOmelette twoEggOmelette = syrupInjector.GetInstance<TwoEggOmelette>();

        //This omelette should be made with two different eggs!
        Assert.AreNotEqual(twoEggOmelette.egg1.id, twoEggOmelette.egg2.id);
    }

    [Test]
    public void TestSyrupInjector_CantGetANonInjectedObject() {
        SyrupInjector syrupInjector = new SyrupInjector(new EmptyModule());
        Assert.Throws<MissingDependencyException>(() => {
            syrupInjector.GetInstance<TastySyrup>();
        });
    }

    [Test]
    public void TestSyrupInjector_CantGetAMissingNamedDependencyEvenIfUnnamedVersionIsProvided() {
        SyrupInjector syrupInjector = new SyrupInjector(new ProvidedEggModule());
        Assert.Throws<MissingDependencyException>(() => {
            syrupInjector.GetInstance<Egg>("BrownEgg");
        });
    }

    [Test]
    public void TestSyrupInjector_ProvidesSameObjectWhenProviderIsMarkedAsSingleton() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingletonProviderModule());
        Egg egg1 = syrupInjector.GetInstance<Egg>();
        Egg egg2 = syrupInjector.GetInstance<Egg>();

        Assert.AreEqual(egg1.id, egg2.id);
    }

    [Test]
    public void TestSyrupInjector_ProvidesSameComponentObjectsWhenProviderIsMarkedSingleton() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingletonWithNonSingletonDependencyModule());
        Pancake pancake1 = syrupInjector.GetInstance<Pancake>();
        Pancake pancake2 = syrupInjector.GetInstance<Pancake>();

        Assert.AreEqual(pancake1.id, pancake2.id);
        Assert.AreEqual(pancake1.flour.id, pancake2.flour.id);
    }

    [Test]
    public void TestSyrupInjector_ProvidesUniqueObjectsWithSingletonDependencies_ThroughConstructor() {
        SyrupInjector syrupInjector = new SyrupInjector(new SingletonProviderModule());
        Omelette omelette1  = syrupInjector.GetInstance<Omelette>();
        Omelette omelette2  = syrupInjector.GetInstance<Omelette>();

        //Somehow two different omelettes are made with the same eggs!
        Assert.AreNotEqual(omelette1.id, omelette2.id);
        Assert.AreEqual(omelette1.egg.id, omelette2.egg.id);
    }

    [Test]
    public void TestSyrupInjector_ProvidesUniqueObjectsWithSingletonDependencies_ThroughProvider() {
        SyrupInjector syrupInjector = new SyrupInjector(new ProviderWithSingletonDependencyModule());
        Pancake pancake1  = syrupInjector.GetInstance<Pancake>();
        Pancake pancake2  = syrupInjector.GetInstance<Pancake>();

        Assert.AreNotEqual(pancake1.id, pancake2.id);
        Assert.AreEqual(pancake1.flour.id, pancake2.flour.id);
    }

    [Test]
    public void TestSyrupInjector_ProvidesSingletonObjectThroughConstructorInjectionOnly() {
        SyrupInjector syrupInjector = new SyrupInjector(new EmptyModule());
        Butter butter1 = syrupInjector.GetInstance<Butter>();
        Butter butter2 = syrupInjector.GetInstance<Butter>();

        Assert.AreEqual(butter1.id, butter2.id);
    }

    [Test]
    public void TestSyrupInjector_ProvidesDependenciesAcrossModules() {
        //This previously bad module is now made complete with the FlourModule!
        SyrupInjector syrupInjector = new SyrupInjector(new FlourModule(), new ProviderWithMissingDependencyModule());

        //We're able to build some pancakes
        Pancake pancake = syrupInjector.GetInstance<Pancake>();
        
        //And some flour separately
        Flour flour = syrupInjector.GetInstance<Flour>();

        Assert.AreNotEqual(pancake.flour.id, flour.id);

        Assert.Throws<MissingDependencyException>(() => {
            //No tasty syrup though
            syrupInjector.GetInstance<TastySyrup>();
        });
    }

    [Test]
    public void TestSyrupInjector_WithoutAnyModules() {
        SyrupInjector syrupInjector = new SyrupInjector();
        
        Egg egg = syrupInjector.GetInstance<Egg>();
        Assert.AreEqual(Egg.SCRAMBLED_EGGS, egg.style);

        //But again, no provided dependency, no tasty syrup
        Assert.Throws<MissingDependencyException>(() => {
            syrupInjector.GetInstance<TastySyrup>();
        });
    }

    [Test]
    public void TestSyrupInjector_WithoutFulfillingConstructorInjectionRequirements() {
        SyrupInjector syrupInjector = new SyrupInjector();

        //As the saying goes, "no flour, no waffles"
        Assert.Throws<MissingDependencyException>(() => {
            syrupInjector.GetInstance<Waffle>();
        });
    }

    [Test]
    public void TestSyrupInjector_TryingToFulfillCircularDependencyThrowsException() {
        Assert.Throws<MissingDependencyException>(() => {
            new SyrupInjector(new CircularDependencyModule());
        });
    }

    [Test]
    public void TestSyrupInjector_FulfillsMethodInjection() {
        SyrupInjector syrupInjector = new SyrupInjector(new FlourModule());
        Waffle waffle1 = syrupInjector.GetInstance<Waffle>();
        Waffle waffle2 = syrupInjector.GetInstance<Waffle>();

        Assert.AreNotEqual(waffle1.id, waffle2.id);
        Assert.NotNull(waffle1.butter);
        Assert.NotNull(waffle2.butter);
        Assert.AreEqual(waffle1.butter.id, waffle2.butter.id);
    }

    [Test]
    public void TestSyrupInjector_FulfillsMultipleInjectedMethodsOnAnObject() {
        SyrupInjector syrupInjector = new SyrupInjector(new TwoDependentProvidersModule(), new SingleProviderModule());

        Buffet buffet = syrupInjector.GetInstance<Buffet>();
        Assert.NotNull(buffet.pancake);
        Assert.NotNull(buffet.tastySyrup);
    }

    [Test]
    public void TestSyrupInjector_DoesNotFulfillMethodInjectionForProviders() {
        SyrupInjector syrupInjector = new SyrupInjector(new BuffetModule());

        Buffet buffet = syrupInjector.GetInstance<Buffet>();
        Assert.Null(buffet.pancake);
        Assert.Null(buffet.tastySyrup);
    }

    [Test]
    public void TestSyrupInjector_FulfillsInheritedMethodInjectionsInTheRightOrder() {
        SyrupInjector syrupInjector = new SyrupInjector(
            new TwoDependentProvidersModule(), 
            new SingleProviderModule(), 
            new ProvidedEggModule());

        AmericanBuffet americanBuffet = syrupInjector.GetInstance<AmericanBuffet>();
        Assert.NotNull(americanBuffet.pancake);
        Assert.NotNull(americanBuffet.tastySyrup);
        Assert.NotNull(americanBuffet.egg);
    }

    [Test]
    public void TestSyrupInjector_CanInjectOnDemand() {
        SyrupInjector syrupInjector = new SyrupInjector();

        EnglishMuffin englishMuffin = new();

        Assert.Null(englishMuffin.butter);

        syrupInjector.Inject(englishMuffin);

        Assert.NotNull(englishMuffin.butter);
    }

    [Test]
    public void TestSyrupInjector_OnDemandInjectionWithInheritedInjects() {
        SyrupInjector syrupInjector = new SyrupInjector(
            new TwoDependentProvidersModule(),
            new SingleProviderModule(),
            new ProvidedEggModule());

        AmericanBuffet americanBuffet = new();
        Assert.Null(americanBuffet.pancake);
        Assert.Null(americanBuffet.tastySyrup);
        Assert.Null(americanBuffet.egg);
        syrupInjector.Inject(americanBuffet);
        Assert.NotNull(americanBuffet.pancake);
        Assert.NotNull(americanBuffet.tastySyrup);
        Assert.NotNull(americanBuffet.egg);
    }

    [Test]
    public void TestSyrupInjector_CanInjectFields() {
        SyrupInjector syrupInjector = new SyrupInjector();

        OrangeJuice orangeJuice = syrupInjector.GetInstance<OrangeJuice>();

        Assert.NotNull(orangeJuice.orange);
    }

    [Test]
    public void TestSyrupInjector_CanInjectFieldsAndMethods() {
        SyrupInjector syrupInjector = new SyrupInjector(OPTIONS);

        LightBreakfast lightBreakfast = syrupInjector.GetInstance<LightBreakfast>();

        Assert.NotNull(lightBreakfast.orangeJuice);
        Assert.NotNull(lightBreakfast.egg);
    }

    [Test]
    public void TestSyrupInjector_CanInjectNamedFields() {
        Assert.Throws<MissingDependencyException>(() => {
            // Show we need the named dependency in order to complete the injection
            new SyrupInjector().GetInstance<CanadianSyrup>();
        });

        SyrupInjector syrupInjector = new SyrupInjector(new SingleNamedProviderModule());

        CanadianSyrup canadianSyrup = syrupInjector.GetInstance<CanadianSyrup>();
        Assert.NotNull(canadianSyrup.tastySyrup);
    }

    [Test]
    public void TestSyrupInjector_CanInjectInheritedFieldsAndMethods() {
        SyrupInjector syrupInjector = new SyrupInjector(new ProviderWithSingletonDependencyModule());

        NewJerseyBrunch newJerseyBrunch = syrupInjector.GetInstance<NewJerseyBrunch>();
        Assert.NotNull(newJerseyBrunch.butter);
        Assert.NotNull(newJerseyBrunch.egg);
        Assert.NotNull(newJerseyBrunch.orangeJuice);
        Assert.NotNull(newJerseyBrunch.pancake);

        StateBrunch stateBrunch = syrupInjector.GetInstance<StateBrunch>();
        Assert.NotNull(stateBrunch.butter);
        Assert.NotNull(stateBrunch.egg);
    }

    [Test]
    public void TestSyrupInjector_CanInjectFieldsOnDemand() {
        SyrupInjector syrupInjector = new SyrupInjector();

        OrangeJuice orangeJuice = new();

        Assert.Null(orangeJuice.orange);

        syrupInjector.Inject(orangeJuice);

        Assert.NotNull(orangeJuice.orange);
    }

    [Test]
    public void TestSyrupInjector_CanInjectFieldsAndMethodsOnDemand() {
        SyrupInjector syrupInjector = new SyrupInjector();

        LightBreakfast lightBreakfast = new();

        Assert.Null(lightBreakfast.orangeJuice);
        Assert.Null(lightBreakfast.egg);

        syrupInjector.Inject(lightBreakfast);

        Assert.NotNull(lightBreakfast.orangeJuice);
        Assert.NotNull(lightBreakfast.egg);
    }

    [Test]
    public void TestSyrupInjector_CanInjectPrivateFieldsAndMethods() {
        SyrupInjector syrupInjector = new SyrupInjector();

        PrivateBreakfast privateBreakfast = syrupInjector.GetInstance<PrivateBreakfast>();

        Assert.NotNull(privateBreakfast.GetOrangeJuice());
        Assert.NotNull(privateBreakfast.GetEgg());
    }
}
