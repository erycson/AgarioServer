#include <iostream>
#include <locale>

int main()
{
	std::locale::global(std::locale(""));

	int t;
	std::cout << "Ol�" << std::endl;
	std::cin >> t;
	return 0;
}