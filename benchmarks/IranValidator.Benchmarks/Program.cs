using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

// Persian.Plus ships non-optimized binaries; the optimizations validator
// would block every run, so it is disabled (standard for third-party deps).
var config = DefaultConfig.Instance
    .WithOptions(ConfigOptions.DisableOptimizationsValidator);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
