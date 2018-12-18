#region imports

using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

#endregion

namespace DynamicallyLoadedApi.Infrastructure {
  public class DynamicControllerChangeProvider : IActionDescriptorChangeProvider {

    public static DynamicControllerChangeProvider Instance = new DynamicControllerChangeProvider();

    public CancellationTokenSource TokenSource { get; private set; }

    public bool HasChanged { get; set; }

    public IChangeToken GetChangeToken() {
      TokenSource = new CancellationTokenSource();
      return new CancellationChangeToken(TokenSource.Token);
    }
  }
}