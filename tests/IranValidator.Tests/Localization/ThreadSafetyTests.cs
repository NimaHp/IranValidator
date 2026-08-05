using System.Collections.Concurrent;
using System.Globalization;
using FluentAssertions;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.Localization;

/// <summary>
/// Gate 6 — thread-safety of <see cref="ValidationMessageOptions"/>: concurrent
/// resolution must be stable, and concurrent registration + resolution must
/// never corrupt the resolver table (backed by a ConcurrentDictionary).
/// </summary>
public sealed class ValidationMessageOptionsThreadSafetyTests
{
    [Fact]
    public void GetResolver_ConcurrentReads_AlwaysReturnsRegisteredResolver()
    {
        var options = new ValidationMessageOptions();
        var faResolver = new PersianMessageResolver();
        var faCulture = CultureInfo.GetCultureInfo("fa-IR");
        options.AddResolver(faCulture, faResolver);

        var mismatches = new ConcurrentBag<string>();
        Parallel.For(0, 8, _ =>
        {
            for (int i = 0; i < 2000; i++)
            {
                IValidationMessageResolver r = options.GetResolver(faCulture);
                if (!ReferenceEquals(r, faResolver))
                    mismatches.Add("GetResolver(fa-IR) returned an unexpected resolver");
            }
        });

        mismatches.Should().BeEmpty();
    }

    [Fact]
    public void AddResolver_ConcurrentWithGetResolver_NeverThrows_AndSettlesCorrectly()
    {
        var options = new ValidationMessageOptions();
        var faResolver = new PersianMessageResolver();
        var faCulture = CultureInfo.GetCultureInfo("fa-IR");
        var frResolver = new EnglishMessageResolver();
        var frCulture = CultureInfo.GetCultureInfo("fr-FR");
        var deResolver = new EnglishMessageResolver();
        var deCulture = CultureInfo.GetCultureInfo("de-DE");
        options.AddResolver(faCulture, faResolver);

        // Snapshot of the fallback before the race: nothing registered for fr/de
        // yet, so the built-in English singleton is returned.
        var fallback = options.GetResolver(frCulture);

        var seen = new ConcurrentBag<IValidationMessageResolver>();
        Parallel.Invoke(
            // Readers hammering an already-registered culture
            () =>
            {
                for (int i = 0; i < 2000; i++)
                    seen.Add(options.GetResolver(faCulture));
            },
            // Readers hammering cultures that are being registered concurrently
            () =>
            {
                for (int i = 0; i < 2000; i++)
                    seen.Add(options.GetResolver(frCulture));
            },
            // Concurrent registrations
            () =>
            {
                for (int i = 0; i < 500; i++)
                    options.AddResolver(frCulture, frResolver);
            },
            () =>
            {
                for (int i = 0; i < 500; i++)
                    options.AddResolver(deCulture, deResolver);
            });

        // Every result observed during the race must be one of the registered
        // resolvers or the pre-race fallback — never garbage, never an exception.
        foreach (IValidationMessageResolver r in seen)
        {
            bool allowed = ReferenceEquals(r, faResolver)
                || ReferenceEquals(r, frResolver)
                || ReferenceEquals(r, deResolver)
                || ReferenceEquals(r, fallback);
            allowed.Should().BeTrue("resolver observed during concurrent registration/resolution must be registered or fallback");
        }

        // Settled state: registrations are all visible afterwards.
        options.GetResolver(frCulture).Should().BeSameAs(frResolver);
        options.GetResolver(deCulture).Should().BeSameAs(deResolver);
        options.GetResolver(faCulture).Should().BeSameAs(faResolver);
    }

    [Fact]
    public void GetResolver_ConcurrentDistinctCultures_MatchesSingleThreadedBaseline()
    {
        var options = new ValidationMessageOptions();
        var faResolver = new PersianMessageResolver();
        var enResolver = new EnglishMessageResolver();
        options.AddResolver(CultureInfo.GetCultureInfo("fa-IR"), faResolver);
        options.AddResolver(CultureInfo.GetCultureInfo("en-US"), enResolver);

        // Baseline per culture.
        var cultures = new[] { CultureInfo.GetCultureInfo("fa-IR"), CultureInfo.GetCultureInfo("en-US"), CultureInfo.GetCultureInfo("de-DE"), CultureInfo.InvariantCulture };
        var baseline = cultures.Select(c => options.GetResolver(c)).ToArray();

        var mismatches = new ConcurrentBag<string>();
        Parallel.For(0, 8, _ =>
        {
            for (int i = 0; i < 1000; i++)
            {
                for (int c = 0; c < cultures.Length; c++)
                {
                    if (!ReferenceEquals(options.GetResolver(cultures[c]), baseline[c]))
                        mismatches.Add($"resolver for {cultures[c].Name} differed from baseline");
                }
            }
        });

        mismatches.Should().BeEmpty();
    }
}
