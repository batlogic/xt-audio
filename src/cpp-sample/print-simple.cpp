#include <xt-cpp.hpp>
#include <iostream>
#include <memory>

int PrintSimpleMain(int argc, char** argv) {
  Xt::Audio audio("", nullptr, nullptr, nullptr);
  for(int s = 0; s < Xt::Audio::GetServiceCount(); s++) {
    std::unique_ptr<Xt::Service> service = Xt::Audio::GetServiceByIndex(s);
    for(int d = 0; d < service->GetDeviceCount(); d++) 
      std::cout << service->GetName() << ": " << service->GetDeviceName(d) << "\n";
  }
  return 0;
}
