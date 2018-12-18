using System;
using System.Threading.Tasks;

namespace DynamicallyLoadedApi.Interfaces {
  public interface IBootstrap {
    Task Run(IServiceProvider services);
  }
}