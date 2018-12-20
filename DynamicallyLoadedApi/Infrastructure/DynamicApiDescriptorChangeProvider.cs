using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace DynamicallyLoadedApi.Infrastructure {
  public class DynamicApiDescriptorChangeProvider : IActionDescriptorChangeProvider {

    public static DynamicApiDescriptorChangeProvider Instance { get; } = new DynamicApiDescriptorChangeProvider();

    public CancellationTokenSource TokenSource { get; private set; }

    public bool HasChanged { get; set; }

    public IChangeToken GetChangeToken() {
      TokenSource = new CancellationTokenSource();
      return new CancellationChangeToken(TokenSource.Token);
    }
  }
}