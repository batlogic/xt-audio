project (xt-audio)
cmake_minimum_required (VERSION 2.8)

# Core library (C API), both static and dynamic linking supported.
set (CORE_DIR "../src/core")
file (GLOB CORE_SRC "${CORE_DIR}/*.*")
add_library (xt-core ${CORE_SRC})
if (BUILD_SHARED_LIBS)
  target_compile_options (xt-core PRIVATE -DXT_SHARED=1)
endif (BUILD_SHARED_LIBS)

if (NOT BUILD_SHARED_LIBS)
# C++ library (C++ API), only static linking supported.
  set (CPP_DIR "../src/cpp")
  file (GLOB CPP_SRC "${CPP_DIR}/*.*")
  add_library(xt-cpp ${CPP_SRC})
  target_include_directories (xt-cpp PRIVATE ${CORE_DIR})
  target_link_libraries (xt-cpp xt-core)

# C++ driver program.
  set (CPP_DRIVER_DIR "../src/cpp-driver")
  file (GLOB CPP_DRIVER_SRC "${CPP_DRIVER_DIR}/*.*")
  add_executable (xt-cpp-driver ${CPP_DRIVER_SRC})
  target_include_directories (xt-cpp-driver PRIVATE ${CPP_DIR})
  target_link_libraries (xt-cpp-driver xt-cpp)

# C++ sample program.
  set (CPP_SAMPLE_DIR "../src/cpp-sample")
  file (GLOB CPP_SAMPLE_SRC "${CPP_SAMPLE_DIR}/*.*")
  add_executable (xt-cpp-sample ${CPP_SAMPLE_SRC})
  target_include_directories (xt-cpp-sample PRIVATE ${CPP_DIR})
  target_link_libraries (xt-cpp-sample xt-cpp)
endif (NOT BUILD_SHARED_LIBS)

if (WIN32)
# Need asmjit for ASIO backend on windows.
  file (GLOB_RECURSE ASMJIT_SRC "${XT_ASMJIT_DIR}/*.*")
  add_library (asmjit STATIC ${ASMJIT_SRC})
  target_compile_options (asmjit PUBLIC -DASMJIT_STATIC=1)
  target_include_directories (xt-core PRIVATE ${XT_ASMJIT_DIR})
  target_link_libraries (xt-core asmjit)

# Need ASIO SDK for ASIO backend on windows.
  file (GLOB ASIOSDK_SRC "${XT_ASIOSDK_DIR}/host/asiodrivers.cpp"
                         "${XT_ASIOSDK_DIR}/host/pc/asiolist.cpp")
  add_library (asiosdk STATIC ${ASIOSDK_SRC})
  target_include_directories (asiosdk PRIVATE ${XT_ASIOSDK_DIR}/common)
  target_include_directories (asiosdk PRIVATE ${XT_ASIOSDK_DIR}/host/pc)
  target_include_directories (xt-core PRIVATE ${XT_ASIOSDK_DIR})
  target_link_libraries (xt-core asiosdk)
endif (WIN32)

# Pull in platform specific dependencies and force g++ in c++11 mode.
if (WIN32)
  target_link_libraries (xt-core dsound dxguid avrt winmm)
else (WIN32)
  set (CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
  target_link_libraries (xt-core pthread asound jack pulse-simple pulse)
endif (WIN32)
