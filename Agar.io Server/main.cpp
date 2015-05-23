#include <iostream>
#include <locale>

int main()
{
	std::locale::global(std::locale(""));

	int t;
	std::cout << "Olá" << std::endl;
	std::cin >> t;
	return 0;
}